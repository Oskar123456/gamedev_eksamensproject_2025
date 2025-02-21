/*
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 * */

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject hit_effect_prefab;
    public GameObject death_effect_prefab;

    Rigidbody rb;
    NavMeshAgent nma;
    GameObject player;
    Transform player_trf;
    EnemyStats stats;

    /* effects */
    GameObject hit_effect;
    float hit_effect_delete_t = 0.3f;
    float hit_effect_delete_t_left;

    GameObject death_effect;
    float death_effect_delete_t = 0.6f;

    float nav_update_t = 0.5f;
    float nav_update_t_left;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        nma = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        player_trf = player.GetComponent<Transform>();
        stats = GetComponent<EnemyStats>();
    }

    void Update()
    {
        if (stats.hp < 1) {
            death_effect = Instantiate(death_effect_prefab, new Vector3(transform.position.x,
                        transform.position.y + (transform.localScale.y / 2.0f), transform.position.z), Quaternion.identity, transform);
            Destroy(death_effect, death_effect_delete_t);
            Destroy(gameObject);
            return;
        }

        if (hit_effect_delete_t_left > 0) {
            hit_effect_delete_t_left -= Time.deltaTime;
            if (hit_effect_delete_t_left <= 0) {
                Destroy(hit_effect);
            }
        }

        if (nma.enabled && nav_update_t_left <= 0 && player_trf.hasChanged) {
            nav_update_t_left = nav_update_t;
            nma.SetDestination(player_trf.position);
        }
        nav_update_t_left -= Time.deltaTime;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.gameObject.CompareTag("Attack"))
            return;

        AttackStats attack_stats = collider.gameObject.GetComponent<AttackScript>().GetStats();
        stats.hp -= attack_stats.damage;
        Debug.Log("enemy attacked at " + transform.position.ToString() + " for " + attack_stats.damage + " damage (" +  stats.hp + "/" + stats.hp_max + ")");

        hit_effect_delete_t_left = hit_effect_delete_t;
        Destroy(hit_effect);
        hit_effect = Instantiate(hit_effect_prefab, new Vector3(transform.position.x,
                    transform.position.y + (transform.localScale.y / 2.0f), transform.position.z), Quaternion.identity, transform);
    }

    void OnCollisionEnter(Collision collision)
    {
    }

    void OnHit(AttackInfo ai)
    {
    }
}

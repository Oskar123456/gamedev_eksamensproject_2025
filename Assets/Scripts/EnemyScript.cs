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
    public GameObject healthbar_prefab;

    Rigidbody rb;
    NavMeshAgent nma;
    GameObject player;
    Transform player_trf;
    EnemyStats stats;
    GameObject healthbar;
    Slider healthbar_slider;
    Transform player_cam_trf;

    /* effects */
    GameObject hit_effect;
    float hit_effect_delete_t = 0.3f;
    float hit_effect_delete_t_left;

    GameObject death_effect;
    float death_effect_delete_t = 0.6f;

    float nav_update_t = 0.1f;
    float nav_update_t_left;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        nma = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        player_trf = player.GetComponent<Transform>();
        player_cam_trf = GameObject.Find("MainCamera").GetComponent<Transform>();
        stats = GetComponent<EnemyStats>();
        healthbar = Instantiate(healthbar_prefab, transform.position + Vector3.up * (transform.lossyScale.y + 1.0f), Quaternion.identity, transform);
        healthbar_slider = healthbar.transform.GetChild(0).gameObject.GetComponent<Slider>();
        healthbar.SetActive(false);
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

        if (player_trf.hasChanged) {
            if (stats.hp < stats.hp_max) {
                Vector3 p1 = player_cam_trf.position;
                Vector3 p2 = healthbar.transform.position;
                p1.y = 0; p2.y = 0;
                healthbar.transform.rotation = Quaternion.FromToRotation(Vector3.forward, p1 - p2);
            }
        }

        // if (nma.enabled && nav_update_t_left <= 0) {
            // nav_update_t_left = nav_update_t; // + (Utils.rng.Next() % 4) / 3.0f;

            // Debug.DrawRay(transform.position + Vector3.up * (transform.lossyScale.y / 2), (player_trf.position - transform.position) * 100, Color.red, 20);

        // NOTE: might want to calc euclidean distance first to save performance.
        // NOTE: hits effect objects in front of player.
        // NOTE: use mask to fix.
        RaycastHit hit_info;
        if (Physics.Raycast(transform.position + Vector3.up * (transform.lossyScale.y / 2), player_trf.position - transform.position, out hit_info, 200)) {
            if (hit_info.collider.gameObject.CompareTag("Player")) {
                nma.SetDestination(player_trf.position);
            }
        }
        // }
        // nav_update_t_left -= Time.deltaTime;
    }

    void TakeDamage(int damage)
    {
        stats.hp -= damage;
        healthbar_slider.value = (float)stats.hp / stats.hp_max;
        healthbar.SetActive(true);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.gameObject.CompareTag("Attack"))
            return;

        AttackScript attack_script = collider.GetComponent<AttackScript>();
        if (attack_script.GetAttacker() == gameObject)
            return;
        AttackStats attack_stats = attack_script.GetStats();
        TakeDamage(attack_stats.damage);

        hit_effect_delete_t_left = hit_effect_delete_t;
        Destroy(hit_effect);
        hit_effect = Instantiate(hit_effect_prefab, new Vector3(transform.position.x,
                    transform.position.y + (transform.localScale.y / 2.0f), transform.position.z), Quaternion.identity, transform);

        nma.ResetPath();
        nma.enabled = false;
        transform.position = transform.position + (Vector3.Normalize(transform.position - collider.transform.position) * 0.4f);
        nma.enabled = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        // if (!collision.gameObject.CompareTag("Player"))
        //     return;
        // collision.gameObject.SendMessage("OnEnemyCollision", stats);
    }

    void OnHit(AttackInfo ai)
    {
    }
}

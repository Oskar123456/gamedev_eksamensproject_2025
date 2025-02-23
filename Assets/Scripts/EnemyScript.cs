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
    public GameObject basic_attack;
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
    GameObject death_effect;
    float death_effect_delete_t = 0.6f;

    float nav_update_t = 0.1f;
    float nav_update_t_left;
    /* combat */
    Vector3 halfway_up_vec;
    float attack_time_left;
    bool is_attacking;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        nma = GetComponent<NavMeshAgent>();
        player = GameObject.Find("Player");
        player_trf = player.GetComponent<Transform>();
        player_cam_trf = GameObject.Find("MainCamera").GetComponent<Transform>();
        stats = GetComponent<EnemyStats>();
        stats.attack_stats = AttackStats.Ghost();
        healthbar = Instantiate(healthbar_prefab, transform.position + Vector3.up * (transform.lossyScale.y + 1.0f), Quaternion.identity, transform);
        healthbar_slider = healthbar.transform.GetChild(0).gameObject.GetComponent<Slider>();
        healthbar.SetActive(false);
        halfway_up_vec = Vector3.up * transform.localScale.y / 2.0f;
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

        if (player_trf.hasChanged) {
            if (stats.hp < stats.hp_max) {
                Vector3 p1 = player_cam_trf.position;
                Vector3 p2 = healthbar.transform.position;
                p1.y = 0; p2.y = 0;
                healthbar.transform.rotation = Quaternion.FromToRotation(Vector3.forward, p1 - p2);
            }
        }

        float dist_to_player = Vector3.Distance(transform.position, player_trf.position);

        if (dist_to_player > 25)
            return;

        if (attack_time_left > 0) {
            is_attacking = true;
            nma.enabled = false;
            attack_time_left -= Time.deltaTime;
        } else {
            nma.enabled = true;
            is_attacking = false;
        }

        if (!nma.enabled)
            return;

        RaycastHit hit_info = new RaycastHit();
        if (Physics.Raycast(transform.position + halfway_up_vec, player_trf.position - transform.position, out hit_info, 200)) {
            if (hit_info.collider.gameObject.CompareTag("Player") && dist_to_player <= stats.attack_stats.range) {
                nma.ResetPath();
                Attack();
            } else {
                float dest_to_player_dist = Vector3.Distance(nma.destination, player_trf.position);
                if (dest_to_player_dist > stats.attack_stats.range / 2.0f)
                    nma.SetDestination(player_trf.position);
            }
        }
    }

    void TakeDamage(int damage)
    {
        stats.hp -= damage;
        healthbar_slider.value = (float)stats.hp / stats.hp_max;
        healthbar.SetActive(true);
    }

    void OnTriggerEnter(Collider collider) { }
    void OnCollisionEnter(Collision collision) { }

    void OnHit(AttackHitInfo hit_info)
    {
        if (hit_info.stats.entity_type != EntityType.Player)
            return;

        TakeDamage(hit_info.stats.damage);

        /* TODO: refactor as "pushback()" or something */
        if (nma.enabled)
            nma.ResetPath();

        nma.enabled = false;
        transform.position = transform.position + (hit_info.normal * 0.4f);
        nma.enabled = true;
    }

    void Attack()
    {
        if (!is_attacking) {
            nma.enabled = false;

            attack_time_left = stats.attack_stats.cooldown;

            Vector3 normal = Vector3.Normalize(player_trf.position - transform.position);
            normal.y = 0;
            transform.rotation = transform.rotation * Quaternion.FromToRotation(transform.forward, normal);

            GameObject attack_obj = Instantiate(basic_attack, transform.position + halfway_up_vec, transform.rotation);

            AttackScript ascr = attack_obj.GetComponent<AttackScript>();
            ascr.SetStats(stats.attack_stats);
            ascr.SetAttacker(gameObject);
        }
    }
}

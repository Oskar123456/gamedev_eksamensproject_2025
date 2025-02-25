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

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyScript : MonoBehaviour
{
    public GameObject basic_attack;
    public GameObject death_effect_prefab;
    public GameObject healthbar_prefab;
    public EnemyType enemy_type;

    Rigidbody rb;
    NavMeshAgent nma;
    GameObject player;
    Transform player_trf;
    GameObject healthbar;
    Slider healthbar_slider;
    Transform player_cam_trf;

    /* effects */
    GameObject death_effect;
    public float death_effect_delete_t = 1f;
    public float death_effect_scale = 1f;

    float nav_update_t = 0.1f;
    float nav_update_t_left;
    /* combat */
    EnemyStats stats;
    AttackStats attack_stats;
    AttackBaseStats attack_base_stats;

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

        healthbar = Instantiate(healthbar_prefab, transform.position + Vector3.up * (transform.lossyScale.y + 1.0f), Quaternion.identity, transform);
        healthbar_slider = healthbar.transform.GetChild(0).gameObject.GetComponent<Slider>();
        healthbar.SetActive(false);
        halfway_up_vec = Vector3.up * transform.localScale.y / 2.0f;

        stats = GetComponent<EnemyStats>();
        attack_stats = GetComponent<AttackStats>();
        attack_base_stats = basic_attack.GetComponent<AttackBaseStats>();

        ScaleStatsToLevel();
    }

    void Update()
    {
        float dist_to_player = Vector3.Distance(transform.position, player_trf.position);

        if (dist_to_player > 20)
            return;

        if (player_trf.hasChanged) {
            if (stats.hp < stats.hp_max) {
                FixHealthBar();
            }
        }

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
            if (hit_info.collider.gameObject.CompareTag("Player") && dist_to_player <= attack_base_stats.range) {
                nma.ResetPath();
                Attack();
            } else {
                float dest_to_player_dist = Vector3.Distance(nma.destination, player_trf.position);
                if (dest_to_player_dist > attack_base_stats.range / 2.0f)
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

    void FixHealthBar()
    {
        Vector3 p1 = player_cam_trf.position;
        Vector3 p2 = healthbar.transform.position;
        p1.y = 0; p2.y = 0;
        healthbar.transform.rotation = Quaternion.FromToRotation(Vector3.forward, p1 - p2);
    }

    void OnTriggerEnter(Collider collider) { }
    void OnCollisionEnter(Collision collision) { }

    void OnHit(AttackHitInfo hit_info)
    {
        if (hit_info.entity_type != EntityType.Player)
            return;

        TakeDamage(hit_info.stats.damage);

        /* TODO: refactor as "pushback()" or something */
        if (nma.enabled)
            nma.ResetPath();

        if (stats.hp < 1) {
            player.SendMessage("AddXp", GameState.level);
            death_effect = Instantiate(death_effect_prefab, transform.position + halfway_up_vec, Quaternion.identity);
            death_effect.transform.localScale = death_effect.transform.localScale * death_effect_scale;
            Destroy(death_effect, death_effect_delete_t);
            Destroy(gameObject);
            return;
        }

        nma.enabled = false;
        transform.position = transform.position + (hit_info.normal * MathF.Min(0.1f * MathF.Sqrt(hit_info.stats.damage), 1.0f));
        nma.enabled = true;
    }

    void Attack()
    {
        if (!is_attacking) {
            nma.enabled = false;

            attack_time_left = attack_base_stats.duration / attack_stats.speed;

            Vector3 normal = Vector3.Normalize(player_trf.position - transform.position);
            normal.y = 0;
            transform.rotation = transform.rotation * Quaternion.FromToRotation(transform.forward, normal);

            GameObject attack_obj = Instantiate(basic_attack, transform.position + halfway_up_vec, transform.rotation, transform);

            AttackScript ascr = attack_obj.GetComponent<AttackScript>();
            ascr.SetStats(attack_stats);
            ascr.SetAttacker(gameObject);
            ascr.SetAttackerEntityType(EntityType.Enemy);
        }
    }

    void ScaleStatsToLevel()
    {
        stats.hp += GameState.level * 5;
        stats.hp_max += GameState.level * 5;
        attack_stats.damage += GameState.level;
        attack_stats.speed += GameState.level * 0.05f;
        attack_stats.scale += GameState.level * 0.05f;
    }
}

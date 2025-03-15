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
using Attacks;
using Loot;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

namespace AI
{
    // [RequireComponent(typeof(Rigidbody))]
    // [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyScript : MonoBehaviour
    {
        public GameObject text_prefab;
        public GameObject bad_text_prefab;
        public GameObject basic_attack;
        public GameObject death_effect_prefab;
        public GameObject healthbar_prefab;
        public float drop_chance = 0.5f;

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

        // float nav_update_t = 0.1f;
        // float nav_update_t_left;
        /* combat */
        GameObject attack_go;

        EnemyStats stats;

        Vector3 halfway_up_vec;
        float attack_time_left;
        bool is_attacking;
        float hit_time_left;

        void Start()
        {
            nma = GetComponent<NavMeshAgent>();

            player_cam_trf = GameObject.Find("Main Camera").GetComponent<Transform>();

            healthbar = Instantiate(healthbar_prefab, transform.position + Vector3.up * (transform.lossyScale.y + 1.0f), Quaternion.identity, transform);
            healthbar_slider = healthbar.transform.GetChild(0).gameObject.GetComponent<Slider>();
            healthbar.SetActive(false);
            halfway_up_vec = Vector3.up * transform.localScale.y / 2.0f;

            stats = GetComponent<EnemyStats>();
            stats.active_attack.ScaleWithEnemyStats(stats);
        }

        void Update()
        {
            if (player == null || player_trf == null) {
                player = GameObject.Find("Player");
                player_trf = player.GetComponent<Transform>();
            }

            if (hit_time_left >= 0) {
                hit_time_left -= Time.deltaTime;
                return;
            }

            float dist_to_player = Vector3.Distance(transform.position, player_trf.position);

            if (dist_to_player > 30)
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

            if (!nma.enabled) {
                return;
            }

            Vector3 normal = Vector3.Normalize(player_trf.position - transform.position);
            RaycastHit hit_info = new RaycastHit();
            if (Physics.Raycast(transform.position + halfway_up_vec - normal * 0.5f, player_trf.position - transform.position, out hit_info, 200)) {
                if (hit_info.collider.gameObject.CompareTag("Player") && dist_to_player <= stats.base_attack_range) {
                    nma.ResetPath();
                    Attack();
                } else {
                    float dest_to_player_dist = Vector3.Distance(nma.destination, player_trf.position);
                    if (dest_to_player_dist > stats.base_attack_range / 2.0f)
                        nma.SetDestination(player_trf.position);
                }
            }
        }

        void TakeDamage(int damage)
        {
            Destroy(attack_go);
            stats.hp -= damage;
            healthbar_slider.value = (float)stats.hp / stats.hp_max;
            healthbar.SetActive(true);
            float hit_time_left = stats.stun_lock;

            GameObject txt = Instantiate(bad_text_prefab, healthbar.transform.position, healthbar.transform.rotation, healthbar.transform);
            txt.transform.localScale = new Vector3(healthbar.transform.localScale.y / healthbar.transform.localScale.x * txt.transform.localScale.x,
                    txt.transform.localScale.y, txt.transform.localScale.z) * 5;
            txt.GetComponent<TextMeshProUGUI>().text = string.Format("-{0} hp", damage);
        }

        void FixHealthBar()
        {
            Vector3 p1 = player_cam_trf.position;
            Vector3 p2 = healthbar.transform.position;
            p1.y = 0; p2.y = 0;
            healthbar.transform.rotation = Quaternion.FromToRotation(Vector3.forward, p2 - p1);
        }

        void OnTriggerEnter(Collider collider) { }
        void OnCollisionEnter(Collision collision) { }

        void OnPush(Vector3 normal)
        {
            if (nma.enabled)
                nma.ResetPath();

            nma.enabled = false;
            transform.position = transform.position + normal;
            nma.enabled = true;
        }

        void OnHit(HitInfo hit_info)
        {
            if (hit_info.parent.tag != "Player")
                return;

            TakeDamage(hit_info.damage);

            /* TODO: refactor as "pushback()" or something */
            if (nma.enabled)
                nma.ResetPath();

            if (stats.hp < 1) {
                if (GameState.rng.Next(101) / 100f < drop_chance) {
                    Item item_dropped = GameData.GenerateLoot();
                    item_dropped.Drop(transform.position);
                }

                hit_info.parent.SendMessage("OnRecXp", stats.xp, SendMessageOptions.DontRequireReceiver);
                death_effect = Instantiate(death_effect_prefab, transform.position + halfway_up_vec, Quaternion.identity);
                death_effect.transform.localScale = death_effect.transform.localScale * death_effect_scale;
                Destroy(death_effect, death_effect_delete_t);
                Destroy(gameObject);
                return;
            }

            nma.enabled = false;
            transform.position = transform.position + (hit_info.normal * MathF.Min(0.1f * MathF.Sqrt(hit_info.damage), 1.0f));
            nma.enabled = true;
        }

        void Attack()
        {
            if (!is_attacking) {
                nma.enabled = false;

                attack_time_left = stats.attack_cooldown;

                Vector3 normal = Vector3.Normalize(player_trf.position - transform.position);
                normal.y = 0;
                transform.rotation = transform.rotation * Quaternion.FromToRotation(transform.forward, normal);

                stats.active_attack.Use(transform);
            }
        }
    }
}

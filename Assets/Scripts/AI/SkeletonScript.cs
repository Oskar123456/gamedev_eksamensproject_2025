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
    [RequireComponent(typeof(NavMeshAgent))]
    public class SkeletonScript : MonoBehaviour
    {
        public GameObject text_prefab;
        public GameObject bad_text_prefab;
        public GameObject death_effect_prefab;
        public GameObject healthbar_prefab;
        public GameObject hit_effect_prefab;
        public GameObject audio_hit_dummy;
        public GameObject audio_bone_hit_dummy;

        public float drop_chance = 0.5f;
        public float move_speed = 3.5f;
        public float move_speed_normal = 3.5f;
        public float move_speed_sprint = 15f;
        public float move_speed_damage_threshold = 10f;

        NavMeshAgent nma;
        GameObject player;
        Transform player_trf;
        GameObject healthbar;
        Slider healthbar_slider;
        Transform player_cam_trf;
        Animator animator;

        /* effects */
        AudioSource audio_source;
        FootStepsScript footsteps;
        float footstep_cooldown = 0.1f;
        float footstep_cooldown_left;

        GameObject death_effect;
        public float death_effect_delete_t = 1f;
        public float death_effect_scale = 1f;

        float nav_update_t = 0.5f;
        float nav_update_t_left;
        /* combat */
        EnemyStats stats;
        Vector3 halfway_up_vec;

        bool is_attacking;
        bool is_fleeing;
        float hit_time_left;
        float fall_time = 1.5f;
        float fall_time_left;

        void Start()
        {
            nma = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            animator.SetFloat("fall_speed", 5);

            player_cam_trf = GameObject.Find("Main Camera").GetComponent<Transform>();

            healthbar = Instantiate(healthbar_prefab, transform.position + Vector3.up * (transform.lossyScale.y + 1.0f), Quaternion.identity, transform);
            healthbar_slider = healthbar.transform.GetChild(0).gameObject.GetComponent<Slider>();
            healthbar.SetActive(false);
            halfway_up_vec = Vector3.up * transform.localScale.y / 2.0f;

            stats = GetComponent<EnemyStats>();
            stats.active_attack.ScaleWithEnemyStats(stats);

            foreach (Transform t in transform) {
                if (t.gameObject.name == "AudioDummyPlayerFootSteps") {
                    footsteps = t.GetComponent<FootStepsScript>();
                }
            }
            if (footsteps == null) {
                Debug.LogError("Skeleton: no footsteps audio source");
            }

            audio_source = GetComponent<AudioSource>();
        }

        void Update()
        {
            if (player == null || player_trf == null) {
                player = GameObject.Find("Player");
                player_trf = player.GetComponent<Transform>();
            }

            if (player_trf.hasChanged) {
                if (stats.hp < stats.hp_max) {
                    FixHealthBar();
                }
            }

            if (hit_time_left >= 0 || fall_time_left >= 0) {
                hit_time_left -= Time.deltaTime;
                fall_time_left -= Time.deltaTime;
            }

            if (hit_time_left <= 0 && fall_time_left <= 0) {
                float dist_to_player = Vector3.Distance(transform.position, player_trf.position);

                if (dist_to_player > stats.attack_range * 2) {
                    is_attacking = false;
                    return;
                }

                if (is_attacking) {
                    nav_update_t_left -= Time.deltaTime;
                    if (nav_update_t_left <= 0) {
                        nav_update_t_left = nav_update_t;
                        nma.SetDestination(player_trf.position);
                        is_fleeing = false;
                    }
                } else {
                    RaycastHit hit_info = new RaycastHit();
                    if (Physics.Raycast(transform.position + halfway_up_vec, player_trf.position - transform.position, out hit_info, 200, 1 << 7)) {
                        if (hit_info.collider.gameObject.CompareTag("Player") && dist_to_player <= stats.attack_range) {
                            Attack();
                        } else {
                            nma.SetDestination(player_trf.position);
                        }
                    }
                }
            }

            move_speed = (is_attacking) ? move_speed_sprint : move_speed_normal;
            nma.speed = move_speed;

            ChooseAnimation();
        }

        void TakeDamage(int damage)
        {
            stats.hp -= damage;
            healthbar_slider.value = (float)stats.hp / stats.hp_max;
            healthbar.SetActive(true);
            float hit_time_left = stats.stun_lock;

            GameObject txt = Instantiate(bad_text_prefab, healthbar.transform.position, healthbar.transform.rotation, healthbar.transform);
            txt.transform.localScale = new Vector3(healthbar.transform.localScale.y / healthbar.transform.localScale.x * txt.transform.localScale.x,
                    txt.transform.localScale.y, txt.transform.localScale.z) * 5;
            txt.GetComponent<TextMeshProUGUI>().text = string.Format("-{0} hp", damage);
            txt.GetComponent<TextMeshProUGUI>().fontSize = 60;
        }

        void FixHealthBar()
        {
            Vector3 p1 = player_cam_trf.position;
            Vector3 p2 = healthbar.transform.position;
            p1.y = 0; p2.y = 0;
            healthbar.transform.rotation = Quaternion.FromToRotation(Vector3.forward, p2 - p1);
        }

        void OnTriggerStay(Collider collider)
        {
            GameObject other = collider.gameObject;

            if (!is_attacking || is_fleeing) {
                return;
            }

            if (other.tag != "Player" || fall_time_left > 0)  {
                return;
            }

            Vector3 halfway_up_vec = Vector3.up * other.transform.lossyScale.y / 2.0f;
            Vector3 normal = Vector3.Normalize(collider.transform.position - transform.position);
            normal.y = 0;

            if (nma.velocity.magnitude < move_speed_damage_threshold) {
                Debug.Log("not doing damage at " + nma.velocity.magnitude.ToString() + " speed");
                float angle = GameState.rng.Next(360);
                float dist = stats.attack_range;
                int c = 0;
                while (true) {
                    c++;
                    if (c > 1000) {
                        Debug.Log("PANICKING INFINITE LOOP IN SKELETONSCRIPT");
                        break;
                    }
                    float x = MathF.Cos(angle) * dist + player_trf.position.x;
                    float z = MathF.Sin(angle) * dist + player_trf.position.z;
                    Vector3 potential_target = new Vector3(x, 100, z);

                    RaycastHit hit_info;
                    Physics.Raycast(potential_target, Vector3.down, out hit_info, 200);

                    if (hit_info.collider == null || hit_info.collider.gameObject.layer != 6) {
                        angle += 20f;
                        dist += (float)((int)angle / 360);
                        continue;
                    }

                    Debug.Log("new destination: " + hit_info.point);

                    is_fleeing = true;

                    NavMeshPath nmp = new NavMeshPath();
                    if (!nma.CalculatePath(hit_info.point, nmp)) {
                        float nmp_dist = 0;
                        for (int i = 0; i < nmp.corners.Length - 1; i++) {
                            nmp_dist += Vector3.Distance(nmp.corners[i], nmp.corners[i + 1]);
                        }
                        nav_update_t_left = (nmp_dist * 2) / (move_speed);
                        continue;
                    }

                    nma.SetDestination(hit_info.point);
                    break;
                }
                return;
            }

            fall_time_left = fall_time;

            GameObject hit_effect_sound = Instantiate(audio_hit_dummy, other.transform.position + halfway_up_vec, Quaternion.identity);
            GameObject hit_effect = Instantiate(hit_effect_prefab, other.transform.position + halfway_up_vec, Quaternion.identity);
            Destroy(hit_effect, 0.4f);
            Destroy(hit_effect_sound, 1);

            nma.enabled = false;
            transform.position -= normal * 0.15f;
            nma.enabled = true;

            is_attacking = false;

            other.SendMessage("OnHit", new HitInfo(normal, gameObject, stats.collision_damage, DamageType.Normal), SendMessageOptions.DontRequireReceiver);

            audio_source.Play();
        }

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
            if (hit_info.parent.tag != "Player") {
                return;
            }

            TakeDamage(hit_info.damage);

            fall_time_left = fall_time;

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
                Instantiate(audio_bone_hit_dummy, transform.position, Quaternion.identity);
                Destroy(death_effect, death_effect_delete_t);
                Destroy(gameObject);
                return;
            }

            nma.enabled = false;
            transform.position = transform.position + (hit_info.normal * MathF.Min(0.1f * MathF.Sqrt(hit_info.damage), 1.0f));
            nma.enabled = true;

            audio_source.Play();
        }

        void Attack()
        {
            if (!is_attacking) {
                nma.ResetPath();
                nma.speed = move_speed_sprint;
                Vector3 normal = Vector3.Normalize(player_trf.position - transform.position);
                nma.SetDestination(player_trf.position + normal * 2);
                is_attacking = true;
            }
        }

        void ChooseAnimation()
        {
            AnimatorStateInfo anim_state_info = animator.GetCurrentAnimatorStateInfo(0);
            footstep_cooldown_left -= Time.deltaTime;

            if (fall_time_left > 0) {
                animator.Play("Mini Simple Characters Armature|Loose");
            } else if (hit_time_left > 0) {
                animator.Play("Mini Simple Characters Armature|Idle");
            } else if (is_attacking) {
                animator.Play("Mini Simple Characters Armature|Run");
                animator.SetFloat("sprint_speed", nma.velocity.magnitude / 5f);
                if (footstep_cooldown_left <= 0) {
                    float anim_time_normed = anim_state_info.normalizedTime % 1.0f;
                    if (anim_time_normed > 0.21f && anim_time_normed < 0.27f) {
                        footsteps.PlayRandom();
                        footstep_cooldown_left = footstep_cooldown;
                    }
                    if (anim_time_normed > 0.71f || anim_time_normed < 0.77f) {
                        footsteps.PlayRandom();
                        footstep_cooldown_left = footstep_cooldown;
                    }
                }
            } else if (nma.velocity.magnitude > 0) {
                animator.Play("Mini Simple Characters Armature|Walk");
                animator.SetFloat("walk_speed", nma.velocity.magnitude / 2f);
                if (footstep_cooldown_left <= 0) {
                    float anim_time_normed = anim_state_info.normalizedTime % 1.0f;
                    if (anim_time_normed > 0.47f && anim_time_normed < 0.53f) {
                        footsteps.PlayRandom();
                        footstep_cooldown_left = footstep_cooldown;
                    }
                    if (anim_time_normed > 0.97f || anim_time_normed < 0.03f) {
                        footsteps.PlayRandom();
                        footstep_cooldown_left = footstep_cooldown;
                    }
                }
            } else {
                animator.Play("Mini Simple Characters Armature|Idle");
            }
        }
    }
}

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
using System.Collections.Generic;
using Attacks;
using Spells;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Player
{
    public class PlayerScript : MonoBehaviour
    {
        public GameObject active_attack;
        public GameObject active_spell;

        AttackInfo active_attack_info;
        SpellInfo active_spell_info;

        GameObject ui_active_attack;
        TextMeshProUGUI ui_active_attack_text;

        GameObject ui_active_spell;
        TextMeshProUGUI ui_active_spell_text;

        AttackBaseStats active_attack_stats;
        SpellBaseStats active_spell_stats;


        GameObject skill_tree_plus_button;

        PlayerStats stats;
        AttackerStats attack_stats;
        CasterStats caster_stats;

        GameObject game_controller;
        CharacterController char_ctrl;
        Animator animator;
        Transform trf;

        public float camera_dist = 0.5f;
        public float scroll_speed = 0.5f;

        // float yaw, pitch;
        Transform cam_trf;
        float cam_trf_angle_x, cam_trf_angle_y;
        Vector3 cam_trf_dist = new Vector3(-10, 16.85f, -10);

        public List<AudioClip> sounds;
        AudioSource audio_source;

        public GameObject bad_text_prefab;
        public GameObject level_up_audio_prefab;
        public GameObject level_up_text_prefab;
        public GameObject level_up_prefab;
        public float level_up_anim_t;
        public float level_up_anim_scale;
        GameObject level_up_effect;

        /* state flags */
        bool did_move = false;
        bool did_jump = false;
        bool did_sprint = false;
        bool did_attack = false;
        bool did_cast = false;
        bool is_attacking = false;
        bool is_casting = false;
        bool is_falling = false;
        /* state parameters */

        // public float fall_init = -0.5f;
        // public float fall_gravity = 30f;
        // public float fall_max = 1f;
        float fall_begin_t;

        float attack_time_left = 0;
        float cast_time_t = 2.33f;
        float cast_time_left = 0;
        float cast_cooldown_left = 0;
        float hit_time_left = 0;
        /* movement */
        float move_speed = 0.2f;
        public float move_speed_normal = 0.2f;
        public float move_speed_sprint_multi = 2.2f;

        float v_horizontal_0, v_horizontal;
        float v_vertical_0, v_vertical;
        float d_xz, d_y;
        public float v_jump = 0.5f;
        public float a_horizontal = 100, a_horizontal_sprint = 200, a_minus_horizontal = 300, a_vertical = -200f;
        public float a_minus_horizontal_fall_factor = 0.01f;
        float fall_time;
        float pitch_0, pitch;
        /* animator parameters */
        public float anim_mul_move_speed = 13;
        float[] anim_attack_time = { 1.0f, 1.333f };
        /* effects */
        Renderer wizard_renderer;
        Color color_original;
        Vector3 halfway_up_vec;
        float effect_blink_t = 0.5f;
        float effect_blink_left_t;

        void Awake()
        {
            stats = GetComponent<PlayerStats>();
            attack_stats = GetComponent<AttackerStats>();
            caster_stats = GetComponent<CasterStats>();
            /* UI */
            skill_tree_plus_button = GameObject.Find("SkillTreePlusButton");
        }

        void Start()
        {
            wizard_renderer = GameObject.Find("WizardBody").GetComponent<Renderer>();
            color_original = wizard_renderer.material.color;

            trf = GetComponent<Transform>();
            char_ctrl = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            cam_trf = GameObject.Find("Main Camera").GetComponent<Transform>();
            cam_trf_angle_x = cam_trf.eulerAngles.x;
            cam_trf_angle_y = cam_trf.eulerAngles.y;

            trf.eulerAngles = new Vector3(0, pitch, 0);
            UpdateCam();
            halfway_up_vec = Vector3.up * transform.localScale.y / 2.0f;

            game_controller = GameObject.Find("GameController");

            audio_source = GetComponent<AudioSource>();
            /* UI */
            ui_active_attack = GameObject.Find("ActiveAttack");
            if (ui_active_attack != null)
                ui_active_attack_text = GameObject.Find("ActiveAttackText").GetComponent<TextMeshProUGUI>();
            ui_active_spell = GameObject.Find("ActiveSpell");
            if (ui_active_spell != null)
                ui_active_spell_text = GameObject.Find("ActiveSpellText").GetComponent<TextMeshProUGUI>();
            ChangeActiveAttack(stats.active_attack);
            ChangeActiveSpell(stats.active_spell);
        }

        void Update()
        {
            if (stats.hp < 1) {
                game_controller.SendMessage("OnDeath");
                return;
            }

            if (effect_blink_left_t > 0) {
                effect_blink_left_t -= Time.deltaTime;
                wizard_renderer.material.color = new Color(color_original.r + MathF.Abs(MathF.Sin(effect_blink_left_t * 20)), color_original.g, color_original.b, color_original.a);
            }

            if (hit_time_left <= 0) {
                PollKeys();
                PollMouse();
                PollMovement();
                PollAttack();
                PollSpell();
                PollMisc();
            } else {
                hit_time_left -= Time.deltaTime;
                attack_time_left -= Time.deltaTime;
                cast_time_left -= Time.deltaTime;
                cast_cooldown_left -= Time.deltaTime;
                if (cast_time_left > 0) {
                    is_casting = true;
                } else {
                    is_casting = false;
                }
                if (attack_time_left > 0) {
                    is_attacking = true;
                } else {
                    is_attacking = false;
                }
            }

            UpdateVelocity();

            if (did_jump) { // fix for stuck on floor when using charactor controller
                char_ctrl.Move(trf.up * 0.001f * Time.deltaTime);
            }

            char_ctrl.Move(transform.forward * d_xz + Vector3.up * d_y);

            ChooseAnimation();

            if (trf.hasChanged) {
                UpdateCam();
            }

            ResetState();
        }

        void PollKeys()
        {
            if (Input.GetKey(KeyCode.LeftShift)) {
                if (Input.GetKeyDown(KeyCode.Alpha1)) {
                    ChangeActiveSpell(0);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2)) {
                    ChangeActiveSpell(1);
                }
            } else {
                if (Input.GetKeyDown(KeyCode.Alpha1)) {
                    ChangeActiveAttack(0);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2)) {
                    ChangeActiveAttack(1);
                }
            }
        }

        void PollMouse()
        {
            float mwheel = Input.GetAxis("Mouse ScrollWheel");
            if (mwheel != 0) {
                camera_dist -= scroll_speed * mwheel;
                UpdateCam();
            }
        }

        void PollAttack()
        {
            if (!is_attacking && !is_casting) {
                // if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(0)) {
                if (Input.GetMouseButton(0)) {
                    attack_time_left = active_attack_stats.duration / attack_stats.speed;
                    animator.SetFloat("attack_speed", 1 / (active_attack_stats.duration / attack_stats.speed));
                    Attack();
                    did_attack = true;
                    is_attacking = true;
                    // Debug.Log(string.Format("attacking: {0}, {1}, {2}, {3}", active_attack_stats.duration, attack_stats.speed,
                    //             (1 / (active_attack_stats.duration / attack_stats.speed)), (active_attack_stats.duration / attack_stats.speed)));
                    return;
                } else {
                    return;
                }
            }

            attack_time_left -= Time.deltaTime;
            if (attack_time_left > 0) {
                is_attacking = true;
            } else {
                is_attacking = false;
            }
        }

        void PollSpell()
        {
            if (!is_casting && !is_attacking && cast_cooldown_left <= 0) {
                // if (!EventSystem.current.IsPointerOverGameObject() && Input.GetMouseButton(1)) {
                if (Input.GetMouseButton(1)) {
                    cast_time_left = cast_time_t / caster_stats.speed;
                    cast_cooldown_left = active_spell_stats.cooldown;
                    animator.SetFloat("cast_speed", caster_stats.speed);
                    did_cast = true;
                    is_casting = true;

                    CastSpell();

                    return;
                } else {
                    return;
                }
            }

            cast_time_left -= Time.deltaTime;
            cast_cooldown_left -= Time.deltaTime;
            if (cast_time_left > 0) {
                is_casting = true;
            } else {
                is_casting = false;
            }
        }

        void PollMisc() {}

        void PollMovement()
        {
            if (is_falling || is_attacking || is_casting) {
                return;
            }

            did_move = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A));

            if (Input.GetKey(KeyCode.LeftShift)) {
                did_sprint = true;
                move_speed = move_speed_normal * move_speed_sprint_multi;
            } else {
                did_sprint = false;
                move_speed = move_speed_normal;
            }

            if (Input.GetKey(KeyCode.W)) {
                pitch = 0;
                if (Input.GetKey(KeyCode.D))
                    pitch += 45f;
                if (Input.GetKey(KeyCode.A))
                    pitch -= 45f;
            }
            else if (Input.GetKey(KeyCode.D)) {
                pitch = 90;
                if (Input.GetKey(KeyCode.S))
                    pitch += 45f;
                if (Input.GetKey(KeyCode.W))
                    pitch -= 45f;
            }
            else if (Input.GetKey(KeyCode.S)) {
                pitch = 180;
                if (Input.GetKey(KeyCode.A))
                    pitch += 45f;
                if (Input.GetKey(KeyCode.D))
                    pitch -= 45f;
            }
            else if (Input.GetKey(KeyCode.A)) {
                pitch = 270;
                if (Input.GetKey(KeyCode.W))
                    pitch += 45f;
                if (Input.GetKey(KeyCode.S))
                    pitch -= 45f;
            }

            if (Input.GetKey(KeyCode.Space)) {
                did_jump = true;
                audio_source.clip = sounds[1];
                audio_source.Play();
            }

            if (did_move) {
                pitch += 45f;
                trf.eulerAngles = new Vector3(0, pitch, 0);
            }
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            // Vector3 normal = hit.normal;
            // if (normal.y >= 0.9 && is_falling) {
                // did_land = true;
                // is_falling = false;
            // }
        }

        void ChooseAnimation()
        {
            if (hit_time_left > 0) {
                animator.Play("GetHit");
            }

            else if (is_attacking) {
                animator.Play("Attack04");
            }

            else if (is_casting) {
                animator.Play("VictoryStart");
            }

            else if (is_falling) {
                if (d_y < 0 && fall_time > 0.1f) { animator.Play("JumpAir"); }
                else if (d_y > 0 && fall_time < 0.2f) { animator.Play("JumpStart"); }
                else if (d_y > 0) { animator.Play("JumpUp"); }
            }

            else if (d_xz > 0) {
                if (did_sprint) {
                    animator.SetFloat("move_speed", anim_mul_move_speed * d_xz / 2);
                    animator.Play("BattleRunForward");
                } else {
                    animator.SetFloat("move_speed", anim_mul_move_speed * d_xz);
                    animator.Play("WalkForward");
                }
            }

            else {
                animator.Play("Idle02");
            }
        }

        void ResetState()
        {
            did_cast = false;
            did_jump = false;
            did_move = false;
            did_attack = false;
            did_sprint = false;
        }

        void UpdateCam()
        {
            cam_trf.position = trf.position + cam_trf_dist * camera_dist;
            cam_trf.eulerAngles = new Vector3(cam_trf_angle_x, cam_trf_angle_y, 0);
        }

        void OnHit(AttackHitInfo hit_info)
        {
            TakeDamage(hit_info.damage);
            char_ctrl.Move(hit_info.normal * MathF.Min(0.1f * MathF.Sqrt(hit_info.damage), 1.0f));
        }

        void OnEnemyCollision(EnemyStats enemy_stats)
        {
            TakeDamage(enemy_stats.collision_damage);
        }

        void TakeDamage(int damage)
        {
            if (stats.invulnerable)
                return;
            audio_source.clip = sounds[3];
            audio_source.Play();
            stats.hp -= damage;
            effect_blink_left_t = effect_blink_t;
            hit_time_left = stats.stun_lock;

            GameObject txt = Instantiate(bad_text_prefab, Vector3.zero, Quaternion.identity, GameObject.Find("Overlay").GetComponent<Transform>());
            txt.transform.localScale = txt.transform.localScale;
            txt.GetComponent<TextMeshProUGUI>().text = string.Format("-{0} hp", damage);
        }

        void OnTriggerEnter(Collider collider)
        {
        }

        void AddXp(int n)
        {
            GameObject xp_effect = Instantiate(level_up_text_prefab, Vector3.zero, Quaternion.identity, GameObject.Find("Overlay").GetComponent<Transform>());
            xp_effect.transform.localScale = xp_effect.transform.localScale;
            xp_effect.GetComponent<TextMeshProUGUI>().text = string.Format("+{0}xp", n);
            if (stats.AddXp(n)) {
                OnLevelUp();
            }
        }

        void OnLevelUp()
        {
            Instantiate(level_up_audio_prefab, transform.position + halfway_up_vec, Quaternion.identity);
            level_up_effect = Instantiate(level_up_prefab, transform.position + halfway_up_vec, Quaternion.identity);
            level_up_effect.transform.localScale = level_up_effect.transform.localScale * level_up_anim_scale;
            Destroy(level_up_effect, level_up_anim_t);
            Instantiate(level_up_text_prefab, Vector3.zero, Quaternion.identity, GameObject.Find("Overlay").GetComponent<Transform>());

            attack_stats.damage += 1;
            attack_stats.speed += 0.15f;
            attack_stats.scale += 0.15f;

            ChangeActiveAttack(stats.active_attack);
            ChangeActiveSpell(stats.active_spell);

            skill_tree_plus_button.SetActive(true);
        }

        void Attack()
        {
            GameObject attack_obj = Instantiate(active_attack, transform.position, transform.rotation);
            AttackerStats ats = attack_obj.GetComponent<AttackerStats>();
            ats.attacker = gameObject;
            ats.entity_type = EntityType.Player;
            ats.attacker_tag = "Player";
            ats.damage = attack_stats.damage;
            ats.speed = attack_stats.speed;
            ats.scale = attack_stats.scale;
        }

        void CastSpell()
        {
            GameObject spell_obj = Instantiate(active_spell, transform.position, transform.rotation);
            spell_obj.transform.localScale = spell_obj.transform.localScale * active_spell_stats.GetScale(caster_stats);
            CasterStats css = spell_obj.GetComponent<CasterStats>();
            css.caster = gameObject;
            css.entity_type = EntityType.Player;
            css.caster_tag = "Player";
            css.damage = caster_stats.damage;
            css.speed = caster_stats.speed;
            css.scale = caster_stats.scale;
            css.spell_level = caster_stats.spell_level;
        }

        void ChangeActiveAttack(int i)
        {
            if (i >= GameData.attack_list.Count || !stats.learned_attacks.Contains(i)) {
                return;
            }

            active_attack = GameData.attack_list[i];
            active_attack_info = active_attack.GetComponent<AttackInfo>();
            active_attack_stats = active_attack.GetComponent<AttackBaseStats>();
            stats.active_attack = i;

            if (ui_active_attack != null) {
                Image img_icon = ui_active_attack.GetComponent<Image>();
                img_icon.sprite = active_attack.GetComponent<AttackInfo>().icon;
                ui_active_attack_text.text = string.Format("{0}{1}Dmg: {2}",
                        active_attack_info.name, Environment.NewLine, active_attack_stats.damage + attack_stats.damage);
            }

            // Debug.Log("ChangeActiveAttack to " + active_attack_info.name);
        }

        void ChangeActiveSpell(int i)
        {
            if (i >= GameData.spell_list.Count || !stats.learned_spells.Contains(i)) {
                return;
            }

            active_spell = GameData.spell_list[i];
            active_spell_info = active_spell.GetComponent<SpellInfo>();
            active_spell_stats = active_spell.GetComponent<SpellBaseStats>();
            caster_stats.spell_level = stats.spell_levels[i];
            stats.active_spell = i;

            if (ui_active_spell != null) {
                Image img_icon = ui_active_spell.GetComponent<Image>();
                img_icon.sprite = active_spell.GetComponent<SpellInfo>().icon;
                ui_active_spell_text.text = string.Format("{0}{1}Level: {2}{3}{4}Dmg: {5}{6}Scale:{7}{8}Duration: {9}", active_spell_info.name,
                        Environment.NewLine, caster_stats.spell_level, Environment.NewLine,
                        Environment.NewLine, active_spell_stats.GetDamage(caster_stats),
                        Environment.NewLine, active_spell_stats.GetScale(caster_stats),
                        Environment.NewLine, active_spell_stats.GetDuration(caster_stats));
            }

            // Debug.Log("ChangeActiveSpell to " + active_spell_info.name);
        }

        void UpdateVelocity()
        {
            char_ctrl.Move(Vector3.up * -0.01f);
            is_falling = !char_ctrl.isGrounded;
            if (is_falling) {
                char_ctrl.Move(Vector3.up * 0.01f);
            }

            /* velocity */
            if (!is_falling) {
                v_vertical_0 = 0;
                fall_time = 0;
                if (did_attack || did_cast) {
                    v_horizontal_0 = 0;
                }
            } else {
                fall_time += Time.deltaTime;
            }

            /* vertical velocity */
            float dt = Time.deltaTime;
            float diff_pitch = MathF.Abs(((pitch - pitch_0 + 180) % 360) - 180);
            float diff_pitch_penalty = (1 - diff_pitch / 180.0f);

            float a = (did_move ? 1 : 0) * (did_sprint ? a_horizontal_sprint : a_horizontal);
            float a_minus = (is_falling ? a_minus_horizontal_fall_factor : 1) * -a_minus_horizontal;
            float a_minus_penalty = did_move ? 0.10f : 1;
            v_horizontal = (a + a_minus * a_minus_penalty) * dt + (v_horizontal_0 * diff_pitch_penalty);
            v_horizontal = MathF.Max(0, v_horizontal);

            v_vertical_0 = (did_jump) ? v_jump : v_vertical_0;
            v_vertical = (a_vertical) * dt + v_vertical_0;

            d_xz = (dt * v_horizontal_0) + 0.5f * v_horizontal * dt * dt;
            d_y = (dt * v_vertical_0) + 0.5f * (a_vertical) * dt * dt;
            d_xz = MathF.Max(0, d_xz);
            d_xz = MathF.Min(move_speed, d_xz);

            // Debug.Log("is_falling: " + is_falling + " ms: " + move_speed + " d_xz: " + d_xz + " d_y : " + d_y);

            pitch_0 = pitch;
            v_vertical_0 = v_vertical;
            v_horizontal_0 = v_horizontal;

        }

        void OnLevelUpSpell(int i)
        {
            stats.spell_levels[i]++;
            ChangeActiveSpell(i);
            if (stats.skill_points < 1) {
                skill_tree_plus_button.SetActive(false);
            }
        }
    }
}

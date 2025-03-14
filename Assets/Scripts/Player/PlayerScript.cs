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
using UI;
using Loot;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Player
{
    public class PlayerScript : MonoBehaviour
    {
        PlayerStats stats;

        /* UI */
        GameObject ui_active_attack;
        CoolDownScript ui_active_attack_cooldown;
        TextMeshProUGUI ui_active_attack_text;
        GameObject ui_active_spell;
        CoolDownScript ui_active_spell_cooldown;
        TextMeshProUGUI ui_active_spell_text;
        GameObject skill_tree_plus_button;
        UITest ui_test;
        PotionSlotScript ui_potion_slot;

        UIScript ui_script;
        GameObject game_controller;
        CharacterController char_ctrl;
        Animator animator;
        Transform trf;

        public float camera_dist = 0.5f;
        public float scroll_speed = 0.5f;

        Transform cam_trf;
        float cam_trf_angle_x, cam_trf_angle_y;
        Vector3 cam_trf_dist = new Vector3(-10, 16.85f, -10);

        /* state flags */
        bool did_move = false;
        bool did_jump = false;
        bool did_sprint = false;
        bool did_attack = false;
        bool did_cast = false;
        bool is_attacking = false;
        bool is_casting = false;
        bool is_falling = false;
        bool is_mouse_hover_ui = false;
        /* state parameters */
        float attack_time_left = 0;
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
        public float attack_anim_time = 1.333f;
        public float cast_anim_speed = 2.333f;
        /* effects */
        public GameObject pickup_audio_dummy;
        public List<AudioClip> sounds;
        AudioSource audio_source;
        public GameObject bad_text_prefab;
        public GameObject level_up_audio_prefab;
        public GameObject level_up_text_prefab;
        public GameObject level_up_prefab;
        GameObject level_up_effect;
        Renderer wizard_renderer;

        Vector3 halfway_up_vec;
        public float level_up_anim_t;
        public float level_up_anim_scale;
        Color color_original;
        float effect_blink_t = 0.5f;
        float effect_blink_left_t;

        void Awake()
        {
        }

        void Start()
        {
            stats = GetComponent<PlayerStats>();

            skill_tree_plus_button = GameObject.Find("SkillTreePlusButton");
            if (skill_tree_plus_button != null)
                skill_tree_plus_button.SetActive(false);

            ui_test = GameObject.Find("UI").GetComponent<UITest>();
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

            GameObject UI = GameObject.Find("UI");
            if (UI != null) {
                ui_script = UI.GetComponent<UIScript>();
            }
            game_controller = GameObject.Find("GameController");

            audio_source = GetComponent<AudioSource>();
            /* UI */
            GameObject ui_potion_slot_go = GameObject.Find("PotionSlot");
            if (ui_potion_slot_go != null) {
                ui_potion_slot = ui_potion_slot_go.GetComponent<PotionSlotScript>();
            }
            ui_active_attack = GameObject.Find("ActiveAttack");
            GameObject ui_active_attack_cooldown_go = GameObject.Find("ActiveAttackCoolDown");
            if (ui_active_attack_cooldown_go != null) {
                ui_active_attack_cooldown = ui_active_attack_cooldown_go.GetComponent<CoolDownScript>();
            }
            if (ui_active_attack != null) {
                ui_active_attack_text = GameObject.Find("ActiveAttackText").GetComponent<TextMeshProUGUI>();
            }
            ui_active_spell = GameObject.Find("ActiveSpell");
            GameObject ui_active_spell_cooldown_go = GameObject.Find("ActiveSpellCoolDown");
            if (ui_active_spell_cooldown_go != null) {
                ui_active_spell_cooldown = ui_active_spell_cooldown_go.GetComponent<CoolDownScript>();
            }
            if (ui_active_spell != null) {
                ui_active_spell_text = GameObject.Find("ActiveSpellText").GetComponent<TextMeshProUGUI>();
            }

            SyncStats();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.PageUp)) {
                stats.attack_scale += 0.25f;
                SyncStats();
            }

            if (stats.hp < 1) {
                game_controller.SendMessage("OnDeath");
                return;
            }

            if (effect_blink_left_t > 0) {
                effect_blink_left_t -= Time.deltaTime;
                if (wizard_renderer != null) {
                    if (effect_blink_left_t > 0) {
                        wizard_renderer.material.color = new Color(color_original.r + MathF.Abs(MathF.Sin(effect_blink_left_t * 20)), color_original.g, color_original.b, color_original.a);
                    } else {
                        wizard_renderer.material.color = color_original;
                    }
                }
            }

            if (hit_time_left <= 0) {
                is_mouse_hover_ui = ui_test.IsPointerOverUIElement();
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
        }

        void LateUpdate()
        {
            if (trf.hasChanged) {
                UpdateCam();
            }
            ResetState();
        }

        void PollKeys()
        {
            if (Input.GetKey(KeyCode.LeftShift)) {
                if (Input.GetKeyDown(KeyCode.Alpha1)) {
                    ChangeActiveAttack(0);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2)) {
                    ChangeActiveAttack(1);
                }
                if (Input.GetKeyDown(KeyCode.Alpha3)) {
                    ChangeActiveAttack(2);
                }
            } else {
            }

            if (Input.GetKeyDown(KeyCode.F)) {
                if (stats.currently_held_item != null) {
                    RaycastHit hit_info;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (!Physics.Raycast(ray, out hit_info, 500, 1 << 6)) {
                        return;
                    }

                    stats.currently_held_item.Drop(hit_info.point);
                    stats.currently_held_item = null;

                    ui_script.Sync();
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
                if (!is_mouse_hover_ui && Input.GetMouseButton(0)) {
                    attack_time_left = attack_anim_time / stats.attack_speed;
                    ui_active_attack_cooldown.SetCoolDown(attack_time_left);
                    animator.SetFloat("attack_speed", attack_anim_time / attack_time_left);
                    stats.active_attack.Use(transform);
                    did_attack = true;
                    is_attacking = true;
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
                if (!is_mouse_hover_ui && Input.GetMouseButton(1)) {
                    cast_time_left = cast_anim_speed / stats.spell_speed;
                    cast_cooldown_left = stats.active_spell.cooldown;
                    ui_active_spell_cooldown.SetCoolDown(cast_cooldown_left);
                    animator.SetFloat("cast_speed", stats.spell_speed);
                    did_cast = true;
                    is_casting = true;
                    stats.active_spell.Use(transform);
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
                move_speed = move_speed_normal * move_speed_sprint_multi * (1 + stats.move_speed_bonus);
            } else {
                did_sprint = false;
                move_speed = move_speed_normal * (1 + stats.move_speed_bonus);
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
            is_mouse_hover_ui = false;
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

        void OnHit(HitInfo hit_info)
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

        void OnRecXp(int n)
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

            stats.hp_max += 5;
            stats.hp += 5;
            stats.attack_speed += 0.05f;
            stats.attack_scale += 0.05f;

            SyncStats();

            skill_tree_plus_button.SetActive(true);
        }

        void ChangeActiveAttack(int i)
        {
            if (i >= stats.learned_attacks.Count) {
                return;
            }

            stats.active_attack = stats.learned_attacks[i];
            SyncStats();
        }

        void ChangeActiveSpell(int i)
        {
            if (i >= stats.learned_spells.Count) {
                return;
            }

            stats.active_spell = stats.learned_spells[i];
            SyncStats();
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
            stats.skill_points--;
            stats.learned_spells[i].level++;
            SyncStats();
        }

        void OnLevelUpAttack(int i)
        {
            stats.skill_points--;
            stats.learned_attacks[i].level++;
            SyncStats();
        }

        void OnPickUp(Item item)
        {
            Instantiate(pickup_audio_dummy, transform.position + halfway_up_vec, Quaternion.identity, transform);

            if (item.is_consumed_on_pickup) {
                GameObject txt = Instantiate(level_up_text_prefab, Vector3.zero, Quaternion.identity, GameObject.Find("Overlay").GetComponent<Transform>());
                txt.transform.localScale = txt.transform.localScale;
                txt.GetComponent<TextMeshProUGUI>().text = item.EffectString();
                item.Consume(stats);
                SyncStats();
            }

            else if (item is HealthPotion) {
                if (stats.potions == null) {
                    stats.potions = (HealthPotion)item;
                } else {
                    stats.potions.amount += ((HealthPotion)item).amount;
                }
                ui_potion_slot.UpdateSprite();
            }

            else if (stats.inventory_space > 0 && stats.currently_held_item == null) {
                for (int i = 0; i < stats.inventory_size; i++) {
                    if (stats.inventory[i] == null) {
                        stats.currently_held_item = item;
                        ui_script.Sync();
                        ui_script.ShowInventory();
                        break;
                    }
                }
            }

            else {
                item.Drop(transform.position + (GameState.rng.Next(10) / 10.0f) * transform.forward
                        + (GameState.rng.Next(10) / 10.0f) * transform.right);
                Debug.Log("No room, dropping " + item.name);
                ui_script.Sync();
                ui_script.ShowInventory();
            }
        }

        public void SyncStats()
        {
            foreach (Attack a in stats.learned_attacks) {
                a.ScaleWithPlayerStats(stats);
            }

            foreach (Spell s in stats.learned_spells) {
                s.ScaleWithPlayerStats(stats);
            }

            if (ui_active_attack != null) {
                Image img_icon = ui_active_attack.GetComponent<Image>();
                img_icon.sprite = GameData.attack_sprites[stats.active_attack.sprite_index];
                ui_active_attack_text.text = stats.active_attack.name;
            }

            if (ui_active_spell != null) {
                Image img_icon = ui_active_spell.GetComponent<Image>();
                img_icon.sprite = GameData.spell_sprites[stats.active_spell.sprite_index];
                ui_active_spell_text.text = stats.active_spell.name;
            }
        }
    }
}

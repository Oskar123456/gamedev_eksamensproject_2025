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
        bool did_pickup = false;
        bool is_attacking = false;
        bool is_casting = false;
        bool is_picking_up = false;
        bool is_falling = false;
        bool is_mouse_hover_ui = false;
        bool is_mouse_hover_enemy = false;
        bool is_mouse_hover_floor = false;
        bool is_mouse_hover_loot = false;
        /* state parameters */
        float attack_time_left;
        float cast_time_left;
        float cast_cooldown_left;
        float pickup_time_left;
        float hit_time_left;
        float follow_time_left;
        /* movement */
        float move_speed = 0.2f;
        public float move_speed_normal = 0.2f;
        public float move_speed_sprint_multi = 2f;

        float v_horizontal_0, v_horizontal;
        float v_vertical_0, v_vertical;
        float d_xz, d_y;
        public float v_jump = 0.5f;
        public float a_horizontal = 100, a_horizontal_sprint = 200, a_minus_horizontal = 300, a_vertical = -200f;
        public float a_minus_horizontal_fall_factor = 0.01f;
        float fall_time;
        float pitch_0, pitch;

        GameObject hover_object;
        GameObject follow_object;
        Vector3 destination_point;
        bool is_following_object = false;
        bool is_moving_to_point = false;
        /* animator parameters */
        public float anim_mul_move_speed = 13;
        public float attack_anim_time = 1.333f;
        public float attack_thrust_anim_time = 1f;
        public float cast_anim_speed = 1.4f;
        public float pickup_anim_time = 1.667f;
        float footstep_cooldown_left;
        int footstep_next;
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
        FootStepsScript footsteps;

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
            /* effects */
            foreach (Transform t in transform) {
                if (t.gameObject.name == "AudioDummyPlayerFootSteps") {
                    footsteps = t.GetComponent<FootStepsScript>();
                }
            }
            if (footsteps == null) {
                Debug.LogError("Player: no footsteps audio source");
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

            if (follow_object == null) {
                is_following_object = false;
            }

            if (is_moving_to_point) {
                if (follow_time_left <= 0) {
                    is_moving_to_point = false;
                } else {
                    follow_time_left -= Time.deltaTime;
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
                pickup_time_left -= Time.deltaTime;
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
                if (pickup_time_left > 0) {
                    is_picking_up = true;
                } else {
                    is_picking_up = false;
                }
            }

            if (is_following_object) {
                Vector3 diff_vec = follow_object.transform.position - transform.position;
                diff_vec.y = 0;
                transform.rotation = Quaternion.LookRotation(diff_vec, Vector3.up);
                pitch = transform.rotation.eulerAngles.y;
                did_move = true;
            } else if (is_moving_to_point) {
                Vector3 diff_vec = destination_point - transform.position;
                diff_vec.y = 0;
                if (diff_vec.magnitude < 0.1f) {
                    is_moving_to_point = false;
                } else {
                    transform.rotation = Quaternion.LookRotation(diff_vec, Vector3.up);
                    pitch = transform.rotation.eulerAngles.y;
                    did_move = true;
                }
            }

            UpdateVelocity();

            if (did_jump) { // fix for stuck on floor when using charactor controller
                char_ctrl.Move(trf.up * 0.001f * Time.deltaTime);
                fall_time += 0.1f;
            }

            if (is_mouse_hover_enemy) {
                Cursor.SetCursor(GameState.cursor_attack, Vector2.zero, CursorMode.Auto);
            } else {
                Cursor.SetCursor(GameState.cursor_default, Vector2.zero, CursorMode.Auto);
            }

            char_ctrl.Move(transform.forward * d_xz + Vector3.up * d_y);

            ChooseAnimation();

            // Debug.Log("is_picking_up: " + is_picking_up + " pickup_time_left: " + pickup_time_left);
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

            RaycastHit hit_info_enemy;
            RaycastHit hit_info_floor;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hit_enemy = Physics.Raycast(ray, out hit_info_enemy, 500, 1 << 10);
            bool hit_floor = Physics.Raycast(ray, out hit_info_floor, 500, 1 << 6);

            is_mouse_hover_loot  = ui_script.is_ui_object_hovered && ui_script.current_ui_object_hovered.tag == "Loot";
            GameObject loot = (is_mouse_hover_loot) ? ui_script.current_ui_object_hovered : null;

            is_mouse_hover_enemy = !(ui_script.is_ui_object_hovered && !is_mouse_hover_loot) && hit_enemy;
            GameObject enemy = (hit_enemy) ? hit_info_enemy.collider.transform.parent.gameObject : null;

            is_mouse_hover_floor = !(ui_script.is_ui_object_hovered && !is_mouse_hover_loot) && !is_mouse_hover_loot && !is_mouse_hover_enemy && hit_floor;

            if (fall_time > 0.1f || is_attacking || is_casting || is_picking_up) {
                return;
            }

            if (!Input.GetMouseButton(0)) {
                return;
            }

            destination_point = (hit_floor) ? hit_info_floor.point : Vector3.zero;

            if (is_mouse_hover_enemy) {
                is_following_object = true;
                is_moving_to_point = false;
                follow_object = enemy;
                // Debug.Log("follow_object: " + follow_object);
            }

            else if (is_mouse_hover_loot) {
                is_following_object = true;
                is_moving_to_point = false;
                LootButtonOverlayScript lbos = loot.GetComponent<LootButtonOverlayScript>();
                follow_object = lbos.loot_object;
                // Debug.Log("follow_object: " + follow_object);
            }

            else if (is_mouse_hover_floor) {
                is_following_object = false;
                is_moving_to_point = true;
                // Debug.Log("destination_point: " + destination_point.ToString());
            }

            if (is_moving_to_point) { // TODO: not entirely accurate
                follow_time_left = Vector3.Distance(destination_point, transform.position) / move_speed_normal;
            }
        }

        void Attack()
        {
            float anim_time = (stats.active_attack.stance == Stance.Slash) ? attack_anim_time : attack_thrust_anim_time;
            attack_time_left = anim_time / stats.attack_speed;
            ui_active_attack_cooldown.SetCoolDown(attack_time_left);
            animator.SetFloat("attack_speed", anim_time / attack_time_left);
            stats.active_attack.Use(transform);
            did_attack = true;
            is_attacking = true;
            is_following_object = false;
            is_moving_to_point = false;
        }

        void PollAttack()
        {
            if (!is_attacking && !is_casting && !is_picking_up) {
                if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl)) {
                    PointAtMouse();
                    Attack();
                } else if (is_following_object && follow_object.tag == "Enemy") {
                    CapsuleCollider cap_coll = follow_object.GetComponent<CapsuleCollider>();
                    if (cap_coll == null) {
                        return;
                    }
                    Vector3 diff_vec = follow_object.transform.position - transform.position;
                    diff_vec.y = 0;
                    if (diff_vec.magnitude <= stats.active_attack.range) {
                        transform.rotation = Quaternion.LookRotation(diff_vec, Vector3.up);
                        Attack();
                    }
                }
                return;
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
            if (!is_casting && !is_attacking && !is_picking_up && cast_cooldown_left <= 0) {
                if (!is_mouse_hover_ui && Input.GetMouseButton(1)) {
                    PointAtMouse();
                    cast_time_left = cast_anim_speed / stats.spell_speed;
                    cast_cooldown_left = stats.active_spell.cooldown;
                    ui_active_spell_cooldown.SetCoolDown(cast_cooldown_left);
                    animator.SetFloat("cast_speed", stats.spell_speed);
                    did_cast = true;
                    is_casting = true;
                    stats.active_spell.Use(transform);
                    is_following_object = false;
                    is_moving_to_point = false;
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

        void PointAtMouse()
        {
            if (fall_time > 0.1f) {
                return;
            }

            RaycastHit hit_info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out hit_info, 200, 1 << 8)) {
                return;
            }

            Vector3 diff_vec = hit_info.point - transform.position;
            diff_vec.y = 0;
            transform.rotation = Quaternion.LookRotation(diff_vec, Vector3.up);
        }

        void PollMisc()
        {
            pickup_time_left -= Time.deltaTime;
            if (pickup_time_left > 0) {
                is_picking_up = true;
            } else {
                is_picking_up = false;
            }

            if (fall_time > 0.1f || is_attacking || is_casting || is_picking_up) {
                return;
            }

            if (is_following_object && follow_object.tag == "Loot") {
                Vector3 diff_vec = (follow_object.transform.position) - transform.position;
                diff_vec.y = 0;

                if (diff_vec.magnitude <= stats.pickup_range) {
                    transform.rotation = Quaternion.LookRotation(diff_vec, Vector3.up);
                    DropScript script = follow_object.GetComponent<DropScript>();
                    script.OnPickUp();
                    is_following_object = false;
                    is_moving_to_point = false;
                    did_pickup = true;
                    is_picking_up = true;
                    pickup_time_left = stats.pickup_time / stats.pickup_speed;
                    animator.SetFloat("pickup_speed", pickup_anim_time / pickup_time_left);
                    // Debug.Log("pickup_time_left: " + pickup_time_left);
                }
            }
        }

        void PollMovement()
        {
            if (fall_time > 0.1f || is_attacking || is_casting || is_picking_up) {
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
                is_following_object = false;
                is_moving_to_point = false;
            }
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.normal.y >= 0.9f && fall_time > 0.1f) {
                footsteps.PlayRandom();
            }
            if (hit.normal.y < 0.1f) {
            }
        }

        void ChooseAnimation()
        {
            AnimatorStateInfo anim_state_info = animator.GetCurrentAnimatorStateInfo(0);

            if (hit_time_left > 0) {
                animator.Play("GetHit");
            }

            else if (is_attacking) {
                if (stats.active_attack.stance == Stance.Slash) {
                    animator.Play("Attack04");
                } else {
                    animator.Play("Attack01");
                }
            }

            else if (is_casting) {
                animator.Play("VictoryStart");
            }

            else if (is_picking_up) {
                animator.Play("PickUp");
            }

            else if (is_falling && fall_time > 0.1f) {
                if (d_y < 0 && fall_time > 0.2f) { animator.Play("JumpAir"); }
                else if (d_y > 0 && fall_time < 0.3f) { animator.Play("JumpStart"); }
                else if (d_y > 0) { animator.Play("JumpUp"); }
            }

            else if (d_xz > 0) {
                if (did_sprint) {
                    animator.SetFloat("move_speed", anim_mul_move_speed * MathF.Min(v_horizontal_0, move_speed) * 0.4f);
                    animator.Play("BattleRunForward");
                    float anim_time_normed = anim_state_info.normalizedTime % 1.0f;
                    if (footstep_next == 0 && (anim_time_normed > 0.07f && anim_time_normed < 0.13f)) {
                        footsteps.PlayRandom();
                        footstep_next = 1;
                    }
                    if (footstep_next == 1 && (anim_time_normed > 0.57f && anim_time_normed < 0.63f)) {
                        footsteps.PlayRandom();
                        footstep_next = 0;
                    }
                } else {
                    animator.SetFloat("move_speed", anim_mul_move_speed * MathF.Min(v_horizontal_0, move_speed));
                    animator.Play("WalkForward");
                    float anim_time_normed = anim_state_info.normalizedTime % 1.0f;
                    if (footstep_next == 0 && (anim_time_normed > 0.97f || anim_time_normed < 0.03f)) {
                        footsteps.PlayRandom();
                        footstep_next = 1;
                    }
                    if (footstep_next == 1 && (anim_time_normed > 0.47f && anim_time_normed < 0.53f)) {
                        footsteps.PlayRandom();
                        footstep_next = 0;
                    }
                }
            }

            else {
                animator.Play("Idle02");
            }
        }

        void ResetState()
        {
            is_mouse_hover_ui = false;
            is_mouse_hover_enemy = false;
            is_mouse_hover_floor = false;
            did_cast = false;
            did_jump = false;
            did_move = false;
            did_attack = false;
            did_sprint = false;
            did_pickup = false;
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
            if (stats.invulnerable) {
                return;
            }

            int d = Math.Max(damage - stats.defense, 0);

            stats.hp -= d;

            if (d > 0) {
                audio_source.clip = sounds[3];
                audio_source.Play();
                effect_blink_left_t = effect_blink_t;
                hit_time_left = stats.stun_lock;
                pickup_time_left = 0;
                is_picking_up = false;
            }

            GameObject txt = Instantiate(bad_text_prefab, Vector3.zero, Quaternion.identity, GameObject.Find("Overlay").GetComponent<Transform>());

            if (d <= 0) {
                txt.GetComponent<TextMeshProUGUI>().text = string.Format("blocked");
            } else {
                txt.GetComponent<TextMeshProUGUI>().text = string.Format("-{0} hp", d);
            }
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
            // stats.attack_speed += 0.05f;
            // stats.attack_scale += 0.05f;

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
                if (did_attack || did_cast || did_pickup) {
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
            d_xz = MathF.Min(move_speed * dt, d_xz);

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
                GameObject txt = Instantiate(bad_text_prefab, Vector3.zero, Quaternion.identity, GameObject.Find("Overlay").GetComponent<Transform>());
                txt.GetComponent<TextMeshProUGUI>().text = string.Format("no inventory space");
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



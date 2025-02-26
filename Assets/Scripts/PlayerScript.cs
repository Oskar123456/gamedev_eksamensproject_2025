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
using Attacks;
using Spells;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject current_attack;
    public GameObject current_spell;

    AttackBaseStats current_attack_stats;
    SpellBaseStats current_spell_stats;

    PlayerStats stats;
    AttackerStats attack_stats;
    CasterStats spell_stats;

    GameObject game_controller;
    CharacterController char_ctrl;
    Animator animator;
    Transform trf;
    System.Random rng = new System.Random();

    public float camera_dist = 0.5f;
    public float scroll_speed = 0.5f;

    float yaw, pitch;
    Transform cam_trf;
    float cam_trf_angle_x, cam_trf_angle_y;
    Vector3 cam_trf_dist = new Vector3(-10, 16.85f, -10);

    public List<AudioClip> sounds;
    AudioSource audio_source;

    public GameObject level_up_audio_prefab;
    public GameObject level_up_text_prefab;
    public GameObject level_up_prefab;
    public float level_up_anim_t;
    public float level_up_anim_scale;
    GameObject level_up_effect;

    /* state flags */
    bool did_move = false;
    bool did_fall = false;
    bool did_move_when_jump = false;
    bool did_jump = false;
    bool did_sprint = false;
    bool did_land = false;
    bool did_attack = false;
    bool did_cast = false;
    bool is_attacking = false;
    bool is_casting = false;
    bool was_hit = false;
    bool is_falling = false;
    /* state parameters */
    float move_speed;
    public float move_speed_normal = 8.0f;
    public float move_speed_sprint = 20.0f;

    public float fall_init = -0.5f;
    public float fall_gravity = 30f;
    public float fall_max = 1f;
    float fall_speed = 0f;
    float fall_begin_t;

    float attack_time_left = 0;
    float cast_time_t = 2.33f;
    float cast_time_left = 0;
    float cast_cooldown_left = 0;
    float hit_time_left = 0;
    /* animator parameters */
    float anim_mul_move_speed = 0.25f;
    float anim_mul_fall_speed = 0.15f;
    float[] anim_attack_time = { 1.0f, 1.333f };
    /* effects */
    Renderer renderer;
    Color color_original;
    Vector3 halfway_up_vec;
    float effect_blink_t = 0.5f;
    float effect_blink_left_t;

    void Awake()
    {
    }

    void Start()
    {
        renderer = GameObject.Find("WizardBody").GetComponent<Renderer>();
        color_original = renderer.material.color;

        trf = GetComponent<Transform>();
        char_ctrl = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        cam_trf = GameObject.Find("MainCamera").GetComponent<Transform>();
        cam_trf_angle_x = cam_trf.eulerAngles.x;
        cam_trf_angle_y = cam_trf.eulerAngles.y;

        trf.eulerAngles = new Vector3(0, pitch, 0);
        UpdateCam();
        halfway_up_vec = Vector3.up * transform.localScale.y / 2.0f;

        game_controller = GameObject.Find("GameController");

        audio_source = GetComponent<AudioSource>();

        stats = GetComponent<PlayerStats>();
        attack_stats = GetComponent<AttackerStats>();
        current_attack_stats = current_attack.GetComponent<AttackBaseStats>();

        spell_stats = GetComponent<CasterStats>();
        current_spell_stats = current_spell.GetComponent<SpellBaseStats>();

        /* test initialization */
    }

    void Update()
    {
        if (stats.hp < 1) {
            game_controller.SendMessage("OnDeath");
            return;
        }

        UpdateFall();
        if (hit_time_left <= 0) {
            PollKeys();
            PollMouse();
            PollMovement();
            PollAttack();
            PollSpell();
            PollMisc();
        }

        if (effect_blink_left_t > 0) {
            effect_blink_left_t -= Time.deltaTime;
            renderer.material.color = new Color(color_original.r + MathF.Abs(MathF.Sin(effect_blink_left_t * 20)), color_original.g, color_original.b, color_original.a);
        }

        ChooseAnimation();

        if (did_attack) {
            is_attacking = true;
        }

        if (did_jump) {
            char_ctrl.Move(trf.up * move_speed * Time.deltaTime);
        }

        if (did_move) {
            char_ctrl.Move(trf.forward * move_speed * Time.deltaTime);
        }

        ResetState();

        if (is_falling) {
            float incl_forward = (did_move_when_jump) ? 1 : 0;
            char_ctrl.Move(trf.forward * move_speed * Time.deltaTime * incl_forward);
        }

        char_ctrl.Move(Vector3.down * fall_speed);
    }

    void PollKeys() { }

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
            if (Input.GetMouseButton(0)) {
                attack_time_left = current_attack_stats.duration / attack_stats.speed;
                did_attack = true;
                animator.SetFloat("attack_speed", 1 / (current_attack_stats.duration / attack_stats.speed));

                Attack();
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
        if (!is_casting && cast_cooldown_left <= 0) {
            if (Input.GetMouseButton(1)) {
                cast_time_left = cast_time_t / spell_stats.speed;
                cast_cooldown_left = current_spell_stats.cooldown;
                animator.SetFloat("cast_speed", spell_stats.speed);
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
        if (!did_fall && (is_falling || is_attacking || is_casting)) {
            return;
        }

        did_move = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A));

        if (Input.GetKey(KeyCode.LeftShift)) {
            did_sprint = true;
            move_speed = move_speed_sprint;
        } else {
            did_sprint = false;
            move_speed = move_speed_normal;
        }

        if (Input.GetKey(KeyCode.W)) {
            pitch = 45f;
            if (Input.GetKey(KeyCode.D))
                pitch += 45f;
            if (Input.GetKey(KeyCode.A))
                pitch -= 45f;
        }
        else if (Input.GetKey(KeyCode.D)) {
            pitch = 135;
            if (Input.GetKey(KeyCode.S))
                pitch += 45f;
            if (Input.GetKey(KeyCode.W))
                pitch -= 45f;
        }
        else if (Input.GetKey(KeyCode.S)) {
            pitch = 225;
            if (Input.GetKey(KeyCode.A))
                pitch += 45f;
            if (Input.GetKey(KeyCode.D))
                pitch -= 45f;
        }
        else if (Input.GetKey(KeyCode.A)) {
            pitch = 315;
            if (Input.GetKey(KeyCode.W))
                pitch += 45f;
            if (Input.GetKey(KeyCode.S))
                pitch -= 45f;
        }

        if (Input.GetKey(KeyCode.Space)) {
            did_jump = true;
            fall_speed = fall_init;
            audio_source.clip = sounds[1];
            audio_source.Play();
            if (did_move) {
                did_move_when_jump = true;
            } else {
                did_move_when_jump = false;
            }
        }

        if (did_move) {
            trf.eulerAngles = new Vector3(0, pitch, 0);
            UpdateCam();
        }
    }

    void UpdateFall()
    {
        if (!is_falling) {
            char_ctrl.Move(Vector3.down * 0.01f);
            if (!char_ctrl.isGrounded) {
                did_fall = true;
                is_falling = true;
                fall_begin_t = Time.time;
                return;
            } else {
                char_ctrl.Move(Vector3.up * 0.01f);
            }
        } else {
            float fall_total_t = Time.time - fall_begin_t;
            float fall_done_t = fall_total_t - Time.deltaTime;
            float fall_since_last_frame = fall_gravity * (fall_total_t * fall_total_t * fall_total_t - fall_done_t * fall_done_t * fall_done_t);
            fall_speed += fall_since_last_frame;
            fall_speed = MathF.Min(fall_speed, fall_max);
        }

        if (char_ctrl.isGrounded) {
            is_falling = false;
            fall_speed = 0.01f;
            audio_source.clip = sounds[2];
            audio_source.Play();
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Vector3 normal = hit.normal;
        if (normal.y >= 0.9 && is_falling) {
            did_land = true;
            is_falling = false;
        }
    }

    void ChooseAnimation()
    {
        // AnimatorStateInfo anim_state_info = animator.GetCurrentAnimatorStateInfo(0);

        if (hit_time_left > 0) {
            hit_time_left -= Time.deltaTime;
            animator.Play("GetHit");
        }

        else if (is_attacking) {
            animator.Play("Attack04");
        }

        else if (is_casting) {
            animator.Play("VictoryStart");
        }

        else if (did_move) {
            if (did_sprint) {
                animator.SetFloat("move_speed", anim_mul_move_speed * move_speed / 2);
                animator.Play("BattleRunForward");
            } else {
                animator.SetFloat("move_speed", anim_mul_move_speed * move_speed);
                animator.Play("WalkForward");
            }
        }

        else if (is_falling) {
            if (fall_speed > 0.1f) { animator.Play("JumpAir"); }
            else if (fall_speed < fall_init / 5) { animator.Play("JumpStart"); }
            else if (fall_speed < -0.2f) { animator.Play("JumpUp"); }
        }

        else {
            animator.Play("Idle02");
        }
    }

    void ResetState()
    {
        did_jump = false;
        did_fall = false;
        did_move = false;
        did_land = false;
        did_attack = false;
        did_sprint = false;
        was_hit = false;
    }

    void UpdateCam()
    {
        cam_trf.position = trf.position + cam_trf_dist * camera_dist;
        cam_trf.eulerAngles = new Vector3(cam_trf_angle_x, cam_trf_angle_y, 0);
    }

    void OnHit(AttackHitInfo hit_info)
    {
        TakeDamage(hit_info.stats.damage);
        char_ctrl.Move(hit_info.normal * MathF.Min(0.1f * MathF.Sqrt(hit_info.stats.damage), 1.0f));
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
        was_hit = true;
        hit_time_left = stats.stun_lock;
    }

    void OnTriggerEnter(Collider collider)
    {
    }

    void AddXp(int n)
    {
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
        attack_stats.speed += 0.05f;
        attack_stats.scale += 0.05f;
    }

    void Attack()
    {
        GameObject attack_obj = Instantiate(current_attack, transform.position, transform.rotation);
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
        GameObject spell_obj = Instantiate(current_spell, transform.position, transform.rotation);
        CasterStats css = spell_obj.GetComponent<CasterStats>();
        css.caster = gameObject;
        css.entity_type = EntityType.Player;
        css.caster_tag = "Player";
        css.damage = spell_stats.damage;
        css.speed = spell_stats.speed;
        css.scale = spell_stats.scale;
    }
}

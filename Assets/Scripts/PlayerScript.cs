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
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public GameObject basic_attack;

    PlayerStats stats;
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

    /* state flags */
    bool did_move = false;
    bool did_move_when_jump = false;
    bool did_jump = false;
    bool did_sprint = false;
    bool did_land = false;
    bool did_attack = false;
    bool is_attacking = false;
    bool was_hit = false;
    bool is_falling = false;
    /* state parameters */
    float move_speed;
    public float move_speed_normal = 10.0f;
    public float move_speed_sprint = 30.0f;

    public float fall_init = -35;
    public float fall_acc = 3.0f;
    public float fall_forward_acc = 0.03f;
    public float fall_speed = 0;

    public float attack_time = 0.25f;
    public float attack_time_dmg_lo = 0.3f, attack_time_dmg_hi = 0.5f;
    float attack_time_left = 0;
    int attack_num = 0;
    /* animator parameters */
    float anim_mul_move_speed = 0.25f;
    float anim_mul_fall_speed = 0.15f;
    float[] anim_attack_time = { 1.0f, 1.333f };
    /* effects */
    Vector3 halfway_up_vec;
    Queue<GameObject> attacks = new Queue<GameObject>();

    void Start()
    {
        stats = GameState.player_stats;

        trf = GetComponent<Transform>();
        char_ctrl = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        cam_trf = GameObject.Find("MainCamera").GetComponent<Transform>();
        cam_trf_angle_x = cam_trf.eulerAngles.x;
        cam_trf_angle_y = cam_trf.eulerAngles.y;

        trf.eulerAngles = new Vector3(0, pitch, 0);
        UpdateCam();
        halfway_up_vec = Vector3.up * transform.localScale.y / 2.0f;

        /* test initialization */
        stats.attack_stats.scale = 2;
    }

    void Update()
    {
        UpdateFall();
        PollKeys();
        PollMouse();
        PollMovement();
        PollAttack();
        PollMisc();

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
        char_ctrl.Move(Vector3.down * fall_speed * Time.deltaTime);

        // cam.Update(trf.position + new Vector3(0, trf.localScale.y / 2, 0), trf.hasChanged);
        // cam_minimap.Update(cam.rotation, trf.position, trf.hasChanged);
    }

    void PollKeys()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            // cam.rot_targ += 90;
        }

        if (Input.GetKey(KeyCode.PageUp)) {
            // cam.angle += 0.1f;
        }

        if (Input.GetKey(KeyCode.PageDown)) {
            // cam.angle -= 0.1f;
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
        if (!is_attacking) {
            if (Input.GetMouseButton(0)) {
                attack_num = 1; // rng.Next() % 2;
                attack_time_left = stats.attack_stats.cooldown;
                did_attack = true;
                animator.SetFloat("attack_speed", 1 / stats.attack_stats.duration);

                GameObject attack_obj = Instantiate(basic_attack, transform.position + halfway_up_vec, transform.rotation);
                AttackScript ascr = attack_obj.GetComponent<AttackScript>();
                ascr.SetStats(stats.attack_stats);
                ascr.SetAttacker(gameObject);
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

    void PollMisc() {}

    void PollMovement()
    {
        if (is_falling || is_attacking) {
            // Debug.Log("PollMovement: is_falling");
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
        if (char_ctrl.isGrounded) {
            is_falling = false;
            fall_speed = fall_acc;
            animator.SetFloat("fall_speed", fall_speed * anim_mul_fall_speed);
            return;
        }

        is_falling = true;

        fall_speed += fall_acc;
        animator.SetFloat("fall_speed", fall_speed * anim_mul_fall_speed);
        move_speed = Math.Max(move_speed - fall_forward_acc, 0);
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

        if (is_attacking) {
            if (attack_num == 0)
                animator.Play("Attack01");
            if (attack_num == 1)
                animator.Play("Attack04");
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
            animator.SetFloat("fall_speed", anim_mul_fall_speed * fall_speed);
            if (fall_speed > 20.0f) { animator.Play("JumpAir"); }
            else if (fall_speed < -30.0f) { animator.Play("JumpStart"); }
            else if (fall_speed < 0.0f) { animator.Play("JumpUp"); }
        }

        else {
            animator.Play("Idle02");
        }
    }

    void ResetState()
    {
        did_jump = false;
        did_move = false;
        did_land = false;
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
        TakeDamage(hit_info.stats.damage);
        char_ctrl.Move(hit_info.normal * MathF.Min(0.1f * MathF.Sqrt(hit_info.stats.damage), 0.5f));
    }

    void OnEnemyCollision(EnemyStats enemy_stats)
    {
        TakeDamage(enemy_stats.collision_damage);
    }

    void TakeDamage(int damage)
    {
        stats.hp -= damage;
    }

    void OnTriggerEnter(Collider collider)
    {
    }
}

public class PlayerStats
{
    public int hp = 100, hp_max = 100;
    public AttackStats attack_stats;

    public PlayerStats() { attack_stats = new AttackStats(); attack_stats.entity_type = EntityType.Player; }
}

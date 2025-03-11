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

using UnityEngine;
using Attacks;
using Spells;

public class EnemyStats : MonoBehaviour
{
    public int xp;
    public int hp;
    public int hp_max;

    public bool invulnerable;
    public float stun_lock;

    public Attack active_attack;

    public int attack_damage;
    public float attack_speed;
    public float attack_scale;
    public float attack_range;
    public float attack_duration;
    public float attack_cooldown;
    public int collision_damage;

    public int base_hp_max;
    public int base_attack_damage;
    public float base_attack_speed;
    public float base_attack_scale;
    public float base_attack_duration;
    public float base_attack_cooldown;
    public int base_collision_damage;
    public float base_attack_range;

    public int xp_per_level;
    public int hp_max_per_level;
    public int attack_damage_per_level;
    public float attack_speed_per_level;
    public float attack_scale_per_level;
    public float attack_duration_per_level;
    public float attack_cooldown_per_level;
    public int collision_damage_per_level;

    void Awake()
    {
        xp = GameState.level * xp_per_level;
        hp_max = GameState.level * hp_max_per_level + base_hp_max;
        hp = hp_max;
        attack_damage = GameState.level * attack_damage_per_level + base_attack_damage;
        attack_speed = GameState.level * attack_speed_per_level + base_attack_speed;
        attack_scale = GameState.level * attack_scale_per_level + base_attack_scale;
        attack_duration = base_attack_duration / attack_speed;
        attack_cooldown = attack_duration;
        collision_damage = GameState.level * collision_damage_per_level + base_collision_damage;
        active_attack = new ShockWaveAttack();
        active_attack.ScaleWithEnemyStats(this);
        attack_range = base_attack_range * attack_scale;
    }
}


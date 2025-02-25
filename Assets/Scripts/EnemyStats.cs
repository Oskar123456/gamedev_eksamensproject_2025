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

public class EnemyStats : MonoBehaviour
{
    public int hp = 5 * GameState.level;
    public int hp_max = 5 * GameState.level;
    public bool invulnerable;
    public float stun_lock = 0.25f;
    public int collision_damage = 1;
}


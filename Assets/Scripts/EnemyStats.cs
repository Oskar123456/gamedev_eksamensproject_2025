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

    public int collision_damage = 1;
    public AttackStats attack_stats;

    void Start()
    {

    }

    void Update()
    {

    }
}


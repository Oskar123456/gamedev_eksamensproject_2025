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

public class PlayerStats
{
    public int hp = 100, hp_max = 100;
    public AttackStats attack_stats;

    public PlayerStats() { attack_stats = new AttackStats(); }

    void Start()
    {

    }

    void Update()
    {

    }
}

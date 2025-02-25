using System;
using UnityEngine;

public class PlayerStats
{
    public int hp = 100, hp_max = 100;
    public int xp = 0, xp_max = 1;
    public int level = 1;
    public AttackStats attack_stats;

    public PlayerStats()
    {
        attack_stats = new AttackStats();
        attack_stats.entity_type = EntityType.Player;
    }

    public bool AddXp(int xp)
    {
        bool did_level = false;

        this.xp += xp;
        while (this.xp >= xp_max) {
            this.xp -= xp_max;
            level++;
            xp_max = level * 2;
            hp_max += 1;
            hp = Math.Min(hp + 5, hp_max);
            Sync();

            did_level = true;
        }
        // Debug.Log("added " + xp + " level is now " + level
        // + " current xp: " + this.xp + "/" + xp_max);
        return did_level;
    }

    public void Sync()
    {
        attack_stats.damage += 1;
        attack_stats.duration -= attack_stats.duration / 10.0f;
        attack_stats.damage_begin_t = attack_stats.duration * 0.2f;
        attack_stats.damage_end_t = attack_stats.duration;
        attack_stats.cooldown = attack_stats.duration;
        attack_stats.scale += 0.1f;
    }
}

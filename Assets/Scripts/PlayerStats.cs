using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int hp = 100, hp_max = 100;
    public int xp = 0, xp_max = 1;
    public int level = 1;
    public AttackStats attack_stats;

    public PlayerStats() { attack_stats = new AttackStats(); attack_stats.entity_type = EntityType.Player; }

    public void AddXp(int xp)
    {
        this.xp += xp;
        while (this.xp >= xp_max) {
            this.xp -= xp_max;
            level++;
            xp_max = level * level;
            hp_max += 1;
            hp = Math.Min(hp + 5, hp_max);
            attack_stats.damage += 1;
            attack_stats.duration -= attack_stats.cooldown / 10.0f;
            attack_stats.damage_begin_t = attack_stats.duration * 0.2f;
            attack_stats.damage_end_t = attack_stats.duration;
            attack_stats.cooldown = attack_stats.duration;
            attack_stats.scale += 0.1f;
        }
        // Debug.Log("added " + xp + " level is now " + level + " current xp: " + this.xp + "/" + xp_max);
    }
}

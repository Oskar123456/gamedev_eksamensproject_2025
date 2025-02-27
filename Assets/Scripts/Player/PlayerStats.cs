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

public class PlayerStats : MonoBehaviour
{
    public int hp = 100, hp_max = 100;
    public int xp = 0, xp_max = 1;
    public int level = 1;
    public bool invulnerable;
    public float stun_lock = 0.15f;
    public float move_speed_bonus = 0;
    public int skill_points = 0;
    public List<int> learned_attacks;
    public List<int> attack_levels;
    public List<int> learned_spells;
    public List<int> spell_levels;
    public int active_attack, active_spell;

    public bool AddXp(int xp)
    {
        bool did_level = false;

        this.xp += xp;
        while (this.xp >= xp_max) {
            this.xp -= xp_max;
            hp_max += 1;
            skill_points += 1;
            hp = Math.Min(hp + 5, hp_max);
            did_level = true;
            level++;
            xp_max = level * level;
        }
        // Debug.Log("added " + xp + " level is now " + level
        // + " current xp: " + this.xp + "/" + xp_max);
        return did_level;
    }

    public void Set(PlayerStats ps)
    {
        this.hp = ps.hp;
        this.hp_max = ps.hp_max;
        this.xp = ps.xp;
        this.xp_max = ps.xp_max;
        this.level = ps.level;
        this.invulnerable = ps.invulnerable;
        this.stun_lock = ps.stun_lock;
        this.move_speed_bonus = ps.move_speed_bonus;
        this.skill_points = ps.skill_points;
        this.active_attack = ps.active_attack;
        this.active_spell = ps.active_spell;

        this.learned_attacks = new List<int>();
        this.learned_spells = new List<int>();
        this.spell_levels = new List<int>();
        for (int i = 0; i < ps.learned_attacks.Count; i++) {
            this.learned_attacks.Add(ps.learned_attacks[i]);
        }
        for (int i = 0; i < ps.learned_spells.Count; i++) {
            this.learned_spells.Add(ps.learned_spells[i]);
        }
        for (int i = 0; i < ps.spell_levels.Count; i++) {
            this.spell_levels.Add(ps.spell_levels[i]);
        }
    }
}

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
    public List<int> learned_attacks;
    public List<int> learned_spells;
    public int active_attack, active_spell;

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
            did_level = true;
        }
        // Debug.Log("added " + xp + " level is now " + level
        // + " current xp: " + this.xp + "/" + xp_max);
        return did_level;
    }
}

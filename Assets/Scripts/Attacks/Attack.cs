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
using System.Collections.Generic;
using Spells;
using Player;
using UnityEngine;

namespace Attacks
{
    public abstract class Attack
    {
        public int prefab_index;
        public int sprite_index;
        public string name;
        public Stance stance = Stance.Slash;

        public int level;

        public int damage, damage_base, damage_per_level;
        public float duration, duration_base, duration_per_level;
        public float cooldown, cooldown_base, cooldown_per_level;
        public float scale, scale_base, scale_per_level;
        public DamageType damage_type;
        public float range, range_base;

        public string description = "no description";

        public abstract void ScaleWithPlayerStats(PlayerStats ps);
        public abstract void ScaleWithEnemyStats(EnemyStats es);

        public abstract void Use(Transform parent);

        public abstract string GetDescriptionString(string delimiter);
        public abstract string GetLevelUpDescriptionString(string delimiter);
    }
}


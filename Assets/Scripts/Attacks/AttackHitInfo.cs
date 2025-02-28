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
using UnityEngine;

namespace Attacks
{

    public class AttackHitInfo
    {
        public EntityType entity_type;
        public float when_t;
        public Vector3 normal;

        public int damage;
        public DamageType damage_type;

        public AttackHitInfo(EntityType et, AttackBaseStats stats, float when_t, Vector3 normal)
        {
            this.entity_type = et;
            this.damage = stats.damage;
            this.damage_type = stats.damage_type;
            this.when_t = when_t;
            this.normal = normal;
        }

        public AttackHitInfo(EntityType et, SpellBaseStats stats, CasterStats cs, float when_t, Vector3 normal)
        {
            this.entity_type = et;
            this.damage = stats.GetDamage(cs);
            this.damage_type = stats.damage_type;
            this.when_t = when_t;
            this.normal = normal;
        }
    }

}

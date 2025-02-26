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
        public AttackBaseStats stats;
        public float when_t;
        public Vector3 normal;

        public AttackHitInfo(EntityType et, AttackBaseStats stats, float when_t, Vector3 normal)
        {
            this.entity_type = et;
            this.stats = stats;
            this.when_t = when_t;
            this.normal = normal;
        }

        public AttackHitInfo(EntityType et, SpellBaseStats stats, float when_t, Vector3 normal)
        {
            this.entity_type = et;
            this.stats = new AttackBaseStats();
            this.stats.damage = stats.damage;
            this.when_t = when_t;
            this.normal = normal;
        }
    }

}

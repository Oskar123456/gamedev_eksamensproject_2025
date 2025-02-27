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

using Attacks;
using UnityEngine;

namespace Spells
{
    public class SpellBaseStats : MonoBehaviour
    {
        public int damage;
        public float scale;
        public float speed;
        public float duration;
        public float cooldown;
        public float hit_effect_duration;
        public DamageType damage_type;

        public EntityType caster_entity_type;
        public string caster_tag;
        public GameObject caster;

        public void SetStats(CasterStats caster_stats)
        {
            damage = damage + caster_stats.damage;
            scale = scale * caster_stats.scale;
        }

        public void SetCasterEntityType(EntityType et) { caster_entity_type = et; }
        public void SetCaster(GameObject a) { caster = a; caster_tag = a.tag; }
    }
}

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
        public int base_damage = 1;
        public int damage;
        public float base_scale = 1;
        public float scale;
        public float base_duration = 1;
        public float duration;

        public float speed;
        public float cooldown;
        public float hit_effect_duration;
        public DamageType damage_type;
        public bool does_damage = true;

        public int damage_per_level = 1;
        public float duration_per_level = 0.1f;
        public float scale_per_level = 0.1f;

        public EntityType caster_entity_type;
        public string caster_tag;
        public GameObject caster;

        public int GetDamage(CasterStats caster_stats)
        {
            if (!does_damage)
                return 0;
            return base_damage + caster_stats.damage + (caster_stats.spell_level - 1) * damage_per_level;
        }

        public float GetScale(CasterStats caster_stats)
        {
            return base_scale * caster_stats.scale + (caster_stats.spell_level - 1) * scale_per_level;
        }

        public float GetDuration(CasterStats caster_stats)
        {
            return base_duration * caster_stats.duration + (caster_stats.spell_level - 1) * duration_per_level;
        }

        public void SetCasterEntityType(EntityType et) { caster_entity_type = et; }
        public void SetCaster(GameObject a) { caster = a; caster_tag = a.tag; }
    }
}

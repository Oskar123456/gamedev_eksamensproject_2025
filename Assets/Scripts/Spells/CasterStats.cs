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

namespace Spells
{
    public class CasterStats : MonoBehaviour
    {
        public GameObject caster;
        public string caster_tag;
        public EntityType entity_type;
        public int damage;
        public float speed;
        public float scale;
        public float duration = 1;
        public int damage_bonus_ice, damage_bonus_fire, damage_bonus_normal;
        public int spell_level = 1;

        public void Set(CasterStats cs)
        {
            this.caster = cs.caster;
            this.caster_tag = cs.caster_tag;
            this.entity_type = cs.entity_type;
            this.damage = cs.damage;
            this.speed = cs.speed;
            this.scale = cs.scale;
            this.damage_bonus_ice = cs.damage_bonus_ice;
            this.damage_bonus_fire = cs.damage_bonus_fire;
            this.damage_bonus_normal = cs.damage_bonus_normal;
            this.spell_level = cs.spell_level;
        }
    }

}


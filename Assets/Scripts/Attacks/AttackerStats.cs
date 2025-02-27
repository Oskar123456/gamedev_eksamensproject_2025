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

namespace Attacks
{
    public class AttackerStats : MonoBehaviour
    {
        public GameObject attacker;
        public string attacker_tag;
        public EntityType entity_type;
        public int damage;
        public float speed;
        public float scale;
        public int damage_bonus_ice, damage_bonus_fire, damage_bonus_normal;

        public void Set(AttackerStats ats)
        {
            this.attacker = ats.attacker;
            this.attacker_tag = ats.attacker_tag;
            this.entity_type = ats.entity_type;
            this.damage = ats.damage;
            this.speed = ats.speed;
            this.scale = ats.scale;
            this.damage_bonus_ice = ats.damage_bonus_ice;
            this.damage_bonus_fire = ats.damage_bonus_fire;
            this.damage_bonus_normal = ats.damage_bonus_normal;
        }
    }
}


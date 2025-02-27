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
    }
}


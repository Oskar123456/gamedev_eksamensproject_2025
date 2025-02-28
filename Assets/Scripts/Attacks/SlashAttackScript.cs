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
    public class SlashAttackScript : MonoBehaviour
    {
        public GameObject attack_effect_prefab;
        public GameObject hit_effect_prefab;
        public GameObject audio_hit_dummy;

        AttackStats stats;

        Vector3 origin;

        public float base_duration;
        float created_t, alive_t;
        public float damage_begin_fraction_t, damage_end_fraction_t;
        float damage_begin_t, damage_end_t;

        List<GameObject> was_damaged;

        void Awake()
        {
            stats = GetComponent<AttackStats>();
        }

        void Start()
        {
            was_damaged = new List<GameObject>();

            created_t = Time.time;
            damage_begin_t = damage_begin_fraction_t * stats.duration;
            damage_end_t = damage_end_fraction_t * stats.duration;

            origin = attacker_stats.attacker.transform.position;
            halfway_up_vec = Vector3.up * (attacker_stats.attacker.transform.lossyScale.y / 2.0f);

            ParticleSystem ps = GetComponent<ParticleSystem>();
            var main = ps.main;
            main.simulationSpeed = base_duration / stats.duration;

            Destroy(effect, stats.duration);
            Destroy(gameObject, stats.duration);
        }

        void Update()
        {
            alive_t = Time.time - created_t;
        }

        void OnTriggerStay(Collider collider)
        {
            if (collider.gameObject.tag == stats.attacker.tag) {
                return;
            }

            if (alive_t < damage_begin_t || alive_t > damage_end_t)
                return;

            if (was_damaged.Contains(collider.gameObject))
                return;

            Vector3 halfway_up_vec = Vector3.up * collider.gameObject.transform.lossyScale.y / 2.0f;

            GameObject hit_effect_sound = Instantiate(audio_hit_dummy, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
            GameObject hit_effect = Instantiate(hit_effect_prefab, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
            Destroy(hit_effect, stats.hit_effect_duration);
            Destroy(hit_effect_sound, 1);

            was_damaged.Add(collider.gameObject);

            Vector3 normal = Vector3.Normalize(collider.transform.position - origin);
            normal.y = 0;

            collider.SendMessage("OnHit", new HitInfo(normal, stats.attacker, stats.damage, stats.damage_type), SendMessageOptions.DontRequireReceiver);
        }
    }

    public class SlashAttack : Attack
    {
        public override void ScaleWithPlayerStats(PlayerStats ps)
        {
            damage   = (damage_per_level   * level + damage_base)   + ps.attack_damage;
            scale    = (scale_per_level    * level + scale_base)    * ps.attack_scale;
            duration = base_duration / ps.attack_speed;
            cooldown = duration;
        }

        public override void ScaleWithEnemyStats(EnemyStats es) { }

        public override void Use(Transform parent)
        {
            GameObject instance = Instantiate(GameData.attack_list[prefab_index],
                    parent.position + Vector3.up * parent.lossyScale.y / 2,
                    parent.rotation * Quaternion.Euler(0, 0, -10));
            AttackStats attack_stats = instance.GetComponent<AttackStats>();
            attack_stats.damage = damage;
            attack_stats.scale = scale;
            attack_stats.duration = duration;
            attack_stats.cooldown = cooldown;
            attack_stats.damage_type = damage_type;
            attack_stats.attacker = parent.gameObject;
        }

        public override string GetDescriptionString(string delimiter)
        {
            return string.Format("{0}{1}Level: {2}{3}{4}Dmg: {5}{6}Scale:{7}", name,
                    delimiter, level, delimiter,
                    delimiter, damage,
                    delimiter, scale);
        }

        public override string GetLevelUpDescriptionString(string delimiter, string string_delimiter, PlayerStats ps)
        {
            string current = string.Format("{0}{1}Current level: {2}{3}{4}Dmg: {5}{6}Scale:{7}", name,
                    delimiter, level, delimiter,
                    delimiter, damage,
                    delimiter, scale);

            level++;
            ScaleWithPlayerStats(ps);

            string next = string.Format("{0}{1}Next level: {2}{3}{4}Dmg: {5}{6}Scale:{7}", name,
                    delimiter, level, delimiter,
                    delimiter, damage,
                    delimiter, scale);

            level--;
            ScaleWithPlayerStats(ps);

            return current + string_delimiter + next;
        }
    }
}

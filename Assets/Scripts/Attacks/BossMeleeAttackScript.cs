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
    public class BossMeleeAttackScript : MonoBehaviour
    {
        public GameObject hit_effect_prefab;
        public GameObject audio_hit_dummy;

        AudioSource audio_source;

        AttackStats stats;

        Vector3 origin;

        float created_t, alive_t;
        float damage_begin_t, damage_end_t;
        public float duration_t, begin_t, end_t;

        List<GameObject> was_damaged;

        void Awake()
        {
            stats = GetComponent<AttackStats>();
            was_damaged = new List<GameObject>();
            audio_source = GetComponent<AudioSource>();
        }

        void Start()
        {
            was_damaged = new List<GameObject>();

            created_t = Time.time;
            damage_begin_t = begin_t * (stats.duration / duration_t);
            damage_end_t = end_t * (stats.duration / duration_t);

            origin = stats.attacker.transform.position;

            ParticleSystem ps = GetComponent<ParticleSystem>();
            var main = ps.main;
            main.simulationSpeed = duration_t / stats.duration;

            foreach (Transform t in transform) {
                ps = t.GetComponent<ParticleSystem>();
                main = ps.main;
                main.simulationSpeed = duration_t / stats.duration;
            }

            Destroy(gameObject, stats.duration);
        }

        void Update()
        {
            alive_t = Time.time - created_t;
        }

        void OnTriggerStay(Collider collider)
        {
            if (alive_t < damage_begin_t || alive_t > damage_end_t)
                return;

            if (stats.attacker == null || collider.gameObject.tag == stats.attacker.tag) {
                return;
            }

            if (was_damaged.Contains(collider.gameObject))
                return;

            was_damaged.Add(collider.gameObject);

            Vector3 halfway_up_vec = Vector3.up * collider.gameObject.transform.lossyScale.y / 2.0f;
            Vector3 normal = Vector3.Normalize(collider.transform.position - origin);
            normal.y = 0;

            GameObject hit_effect_sound = Instantiate(audio_hit_dummy, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
            GameObject hit_effect = Instantiate(hit_effect_prefab, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
            Destroy(hit_effect, 0.4f);
            Destroy(hit_effect_sound, 1);

            collider.SendMessage("OnHit", new HitInfo(normal, stats.attacker, stats.damage, stats.damage_type), SendMessageOptions.DontRequireReceiver);
        }
    }

    public class BossMeleeAttack : Attack
    {
        public BossMeleeAttack()
        {
            prefab_index = 2;
            sprite_index = 2;
            name = "Force Attack";
            level = 1;
            damage_base = 1; damage_per_level = 1;
            duration_base = 1; duration_per_level = 0;
            cooldown_base = 1; cooldown_per_level = 0;
            scale_base = 1; scale_per_level = 0.1f;
            damage_type = DamageType.Normal;

        }

        public override void ScaleWithPlayerStats(PlayerStats ps)
        {
            damage   = (damage_per_level * level + damage_base) + ps.attack_damage;
            scale    = (scale_per_level  * level + scale_base)  * ps.attack_scale;
            duration = duration_base     / ps.attack_speed;
            cooldown = duration;
        }

        public override void ScaleWithEnemyStats(EnemyStats es)
        {
            damage   = damage_base   + es.attack_damage;
            scale    = scale_base    * es.attack_scale;
            duration = duration_base / es.attack_speed;
            cooldown = duration;
        }

        public override void Use(Transform parent)
        {
            GameObject instance = GameState.InstantiateParented(GameData.attack_prefabs[prefab_index], parent.position, parent.rotation, parent);
            AttackStats attack_stats = instance.GetComponent<AttackStats>();
            attack_stats.damage = damage;
            attack_stats.scale = scale;
            attack_stats.duration = duration;
            attack_stats.base_duration = 1;
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

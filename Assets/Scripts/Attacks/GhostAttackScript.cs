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
    public class GhostAttackScript : MonoBehaviour
    {
        public GameObject effect_prefab;
        public GameObject hit_effect_prefab;
        public GameObject audio_dummy;
        public GameObject audio_hit_dummy;

        AudioSource audio_source;
        CapsuleCollider coll;
        AttackStats stats;

        Vector3 origin;
        Vector3 pos_start;
        Vector3 pos_end;

        float created_t, alive_t;
        bool created = false;
        public float delay = 0.55f;
        public float degrees_orig;
        public float offs_start = 0.0f;
        public float offs_end = 5f;
        public float effect_duration = 0.20f;

        List<GameObject> was_damaged;

        void Awake()
        {
            audio_source = GetComponent<AudioSource>();
            stats = GetComponent<AttackStats>();
            coll = GetComponent<CapsuleCollider>();
            coll.enabled = false;
            origin = transform.position;
        }

        void Start()
        {
            was_damaged = new List<GameObject>();

            created_t = Time.time;
            delay = delay / (stats.base_duration / stats.duration);
            effect_duration = effect_duration / (stats.base_duration / stats.duration);

            offs_start *= stats.scale;
            offs_end *= stats.scale;

            Vector3 halfway_up_vec = Vector3.up * (stats.attacker.transform.lossyScale.y / 2);

            transform.localScale *= 1 + ((stats.scale - 1) / 2);

            Instantiate(audio_dummy, stats.attacker.transform.position + halfway_up_vec, transform.rotation, stats.attacker.transform);

            GameObject effect = Instantiate(effect_prefab, transform.position + transform.forward * offs_start + halfway_up_vec,
                    transform.rotation * Quaternion.Euler(0, 0, 90), stats.attacker.transform);

            effect.transform.localScale *= 1 + ((stats.scale - 1) / 2);
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.simulationSpeed = stats.base_duration / stats.duration;

            foreach (Transform t in effect.transform) {
                t.localScale *= 1 + ((stats.scale - 1) / 2);
                ps = t.gameObject.GetComponent<ParticleSystem>();
                if (ps == null) { continue; }
                main = ps.main;
                main.simulationSpeed = stats.base_duration / stats.duration;
            }

            Destroy(gameObject, delay + effect_duration);
            Destroy(effect, effect_duration + delay + 0.25f);

            pos_start = transform.position + transform.forward * offs_start * 5 + halfway_up_vec;
            pos_end = transform.position + transform.forward * offs_end + transform.forward * offs_start * 1 + halfway_up_vec;
        }

        void Update()
        {
            alive_t = Time.time - created_t;

            if (alive_t < delay) {
                return;
            }

            float alive_t_frac = (alive_t - delay) / effect_duration;

            if (!created) {
                coll.enabled = true;
                created = true;
            }

            Vector3 pos_current = Vector3.Lerp(pos_start, pos_end, alive_t_frac);
            transform.position = pos_current;
        }

        void OnTriggerStay(Collider collider)
        {
            if (was_damaged.Contains(collider.gameObject)) {
                return;
            }

            if (stats.attacker == null || collider.gameObject.tag == stats.attacker.tag) {
                return;
            }

            GameObject hit_effect_sound = Instantiate(audio_hit_dummy, collider.transform.position, Quaternion.identity);
            GameObject hit_effect = Instantiate(hit_effect_prefab, collider.transform.position, Quaternion.identity);
            Destroy(hit_effect, 0.4f);
            Destroy(hit_effect_sound, 1);

            was_damaged.Add(collider.gameObject);

            Vector3 normal = Vector3.Normalize(collider.transform.position - origin);
            normal.y = 0;

            collider.SendMessage("OnHit", new HitInfo(normal, stats.attacker, stats.damage, stats.damage_type), SendMessageOptions.DontRequireReceiver);
        }
    }

    public class GhostAttackNormal : Attack
    {
        public GhostAttackNormal()
        {
            prefab_index = 4;
            sprite_index = 0;
            name = "Ghost Attack Normal";
            level = 1;
            damage_base = 1; damage_per_level = 1;
            duration_base = 0.75f; duration_per_level = 0;
            cooldown_base = 1; cooldown_per_level = 0;
            scale_base = 1; scale_per_level = 0.025f;
            damage_type = DamageType.Normal;
        }

        public override void ScaleWithPlayerStats(PlayerStats ps)
        {
            damage   = (damage_per_level * level + damage_base) + ps.attack_damage + ps.attack_damage_normal;
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
            Transform t = parent;

            GameObject instance = GameState.InstantiateParented(GameData.attack_prefabs[prefab_index], parent.position + parent.forward * (parent.lossyScale.x * 0.67f), parent.rotation, t);

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

        public override string GetLevelUpDescriptionString(string delimiter)
        {
            return "";
        }
    }
}

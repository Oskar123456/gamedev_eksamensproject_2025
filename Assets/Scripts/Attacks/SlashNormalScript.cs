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
    public class SlashNormalScript : MonoBehaviour
    {
        public GameObject effect_prefab;
        public GameObject hit_effect_prefab;
        public GameObject audio_dummy;
        public GameObject audio_hit_dummy;

        AudioSource audio_source;
        CapsuleCollider coll;
        AttackStats stats;

        Vector3 origin;

        float created_t, alive_t;
        bool created = false;
        public float delay = 0.33f;
        public float radius = 1.75f;
        public float degrees_orig;
        public float degrees_start = -90f;
        public float degrees_end = 180f;
        public float effect_duration = 0.35f;

        List<GameObject> was_damaged;

        void Awake()
        {
            audio_source = GetComponent<AudioSource>();
            stats = GetComponent<AttackStats>();
            coll = GetComponent<CapsuleCollider>();
            coll.enabled = false;
        }

        void Start()
        {
            was_damaged = new List<GameObject>();

            created_t = Time.time;
            delay = delay / (stats.base_duration / stats.duration);
            effect_duration = effect_duration / (stats.base_duration / stats.duration);

            radius *= stats.scale;
            transform.localScale *= stats.scale;

            origin = stats.attacker.transform.position + Vector3.up * (stats.attacker.transform.lossyScale.y / 2);
            degrees_orig = transform.rotation.eulerAngles.y;

            Destroy(gameObject, delay + effect_duration);
        }

        void Update()
        {
            alive_t = Time.time - created_t;

            if (alive_t < delay) {
                return;
            }

            float alive_t_frac = (alive_t - delay) / effect_duration;

            if (!created) {
                Instantiate(audio_dummy, stats.attacker.transform.position + Vector3.up * (stats.attacker.transform.lossyScale.y / 2), transform.rotation, stats.attacker.transform);
                GameObject effect = Instantiate(effect_prefab, stats.attacker.transform.position + Vector3.up * (stats.attacker.transform.lossyScale.y / 2),
                        transform.rotation, stats.attacker.transform);
                Destroy(effect, effect_duration);
                effect.transform.localScale *= stats.scale;
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.simulationSpeed = stats.base_duration / stats.duration;
                coll.enabled = true;
                created = true;
            }

            float degrees_current = Mathf.Lerp(degrees_orig + degrees_start, degrees_orig + degrees_end, alive_t_frac) % 360f;
            float degrees_current_rad = degrees_current * ((2 * MathF.PI) / 360f);
            transform.position = new Vector3(radius * MathF.Cos(360 - degrees_current_rad) + origin.x, origin.y, radius * MathF.Sin(360 - degrees_current_rad) + origin.z);
        }

        void OnTriggerStay(Collider collider)
        {
            if (was_damaged.Contains(collider.gameObject)) {
                return;
            }

            if (stats.attacker == null || collider.gameObject.tag == stats.attacker.tag) {
                return;
            }

            Vector3 halfway_up_vec = Vector3.up * collider.gameObject.transform.lossyScale.y / 2.0f;

            GameObject hit_effect_sound = Instantiate(audio_hit_dummy, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
            GameObject hit_effect = Instantiate(hit_effect_prefab, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
            Destroy(hit_effect, 0.4f);
            Destroy(hit_effect_sound, 1);

            was_damaged.Add(collider.gameObject);

            Vector3 normal = Vector3.Normalize(collider.transform.position - origin);
            normal.y = 0;

            collider.SendMessage("OnHit", new HitInfo(normal, stats.attacker, stats.damage, stats.damage_type), SendMessageOptions.DontRequireReceiver);
        }
    }

    public class SlashNormal : Attack
    {
        public SlashNormal()
        {
            prefab_index = 0;
            sprite_index = 0;
            name = "Slash";
            level = 1;
            damage_base = 1; damage_per_level = 1;
            duration_base = 1; duration_per_level = 0;
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

        public override void ScaleWithEnemyStats(EnemyStats es) { }

        public override void Use(Transform parent)
        {
            GameObject instance = GameState.InstantiateParented(GameData.attack_prefabs[prefab_index],
                    parent.position + Vector3.up * parent.lossyScale.y / 2,
                    parent.rotation * Quaternion.Euler(0, 0, -5), parent);
            instance.transform.localScale = instance.transform.localScale * scale;

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

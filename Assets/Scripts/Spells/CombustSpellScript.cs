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
using System.Collections;
using System.Collections.Generic;
using Attacks;
using Spells;
using UnityEngine;

namespace Spells
{
    public class CombustSpellScript : MonoBehaviour
    {
        public GameObject effect_prefab;
        public GameObject hit_effect_prefab;
        public GameObject audio_dummy;
        public GameObject audio_hit_dummy;

        AudioSource audio_source;
        SphereCollider coll;
        SpellStats stats;

        Vector3 origin;

        float created_t, alive_t;
        bool created = false;
        public float delay = 1.00f;
        public Vector3 scale_start = new Vector3(2, 2, 2);
        public Vector3 scale_end = new Vector3(8, 4, 8);
        public float effect_duration = 0.20f;

        List<GameObject> was_damaged;

        void Awake()
        {
            audio_source = GetComponent<AudioSource>();
            stats = GetComponent<SpellStats>();
            coll = GetComponent<SphereCollider>();
            coll.enabled = false;
        }

        void Start()
        {
            was_damaged = new List<GameObject>();

            created_t = Time.time;
            delay = delay / (stats.base_duration / stats.duration);
            effect_duration = effect_duration / (stats.base_duration / stats.duration);

            scale_start *= 1 + ((stats.scale - 1) / 2);
            scale_end *= 1 + ((stats.scale - 1) / 2);

            GameObject effect = Instantiate(effect_prefab, stats.caster.transform.position + Vector3.up * (stats.caster.transform.lossyScale.y / 2),
                    stats.caster.transform.rotation, stats.caster.transform);
            effect.transform.localScale *= 1 + ((stats.scale - 1) / 2);

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.simulationSpeed = stats.base_duration / stats.duration;

            foreach (Transform t in ps.transform) {
                t.localScale *= 1 + ((stats.scale - 1) / 2);
                ps = effect.GetComponent<ParticleSystem>();
                main = ps.main;
                main.simulationSpeed = stats.base_duration / stats.duration;
            }

            Destroy(effect, effect_duration + delay);
            Destroy(gameObject, effect_duration + delay);
        }

        void Update()
        {
            alive_t = Time.time - created_t;

            if (alive_t < delay) {
                return;
            }

            float alive_t_frac = (alive_t - delay) / effect_duration;

            if (!created) {
                Instantiate(audio_dummy, stats.caster.transform.position + Vector3.up * (stats.caster.transform.lossyScale.y / 2), transform.rotation, stats.caster.transform);
                coll.enabled = true;
                created = true;
            }

            transform.localScale = Vector3.Lerp(scale_start, scale_end, alive_t_frac);
        }

        void OnTriggerStay(Collider collider)
        {
            if (stats.caster == null || collider.gameObject.tag == stats.caster.tag) {
                return;
            }

            if (was_damaged.Contains(collider.gameObject)) {
                return;
            }

            was_damaged.Add(collider.gameObject);

            Vector3 halfway_up_vec = Vector3.up * collider.gameObject.transform.lossyScale.y / 2.0f;

            GameObject hit_effect_sound = Instantiate(audio_hit_dummy, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
            GameObject hit_effect = Instantiate(hit_effect_prefab, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
            Destroy(hit_effect, 0.4f);
            Destroy(hit_effect_sound, 1);

            collider.gameObject.SendMessage("OnHit", new HitInfo(Vector3.zero, stats.caster, stats.damage, stats.damage_type), SendMessageOptions.DontRequireReceiver);
        }
    }

    public class CombustSpell : Spell
    {
        public CombustSpell()
        {
            prefab_index = 2;
            sprite_index = 2;
            name = "Combust";
            level = 1;
            damage_base = 4; damage_per_level = 1;
            duration_base = 1.2f; duration_per_level = 0f;
            cooldown_base = 8; cooldown_per_level = 0;
            scale_base = 1f; scale_per_level = 0.1f;
            damage_type = DamageType.Ice;
        }

        public override void ScaleWithPlayerStats(PlayerStats ps)
        {
            damage   = (damage_per_level   * level + damage_base)   + ps.spell_damage + ps.spell_damage_ice;
            scale    = (scale_per_level    * level + scale_base)    * ps.spell_scale;
            duration = (duration_per_level * level + duration_base) * ps.spell_duration;
            cooldown = (cooldown_per_level * level + cooldown_base) * ps.spell_cooldown;
        }

        public override void ScaleWithEnemyStats(EnemyStats es) { }

        public override void Use(Transform parent)
        {
            GameObject instance = GameState.InstantiateParented(GameData.spell_prefabs[prefab_index],
                    parent.position + Vector3.up * (parent.lossyScale.y / 2), Quaternion.identity, parent);

            SpellStats spell_stats = instance.GetComponent<SpellStats>();
            spell_stats.damage = damage;
            spell_stats.scale = scale;
            spell_stats.duration = duration;
            spell_stats.base_duration = duration_base;
            spell_stats.damage_type = damage_type;
            spell_stats.caster = parent.gameObject;
        }

        public override string GetDescriptionString(string delimiter)
        {
            return string.Format("{0}{1}Level: {2}{3}{4}Dmg: {5}{6}Scale:{7: 0.00}{8}Duration: {9: 0.00}", name,
                    delimiter, level, delimiter,
                    delimiter, damage,
                    delimiter, scale,
                    delimiter, duration);
        }

        public override string GetLevelUpDescriptionString(string delimiter, string string_delimiter, PlayerStats ps)
        {
            string current = string.Format("{0}{1}Current level: {2}{3}{4}Dmg: {5}{6}Scale:{7: 0.00}{8}Duration: {9: 0.00}", name,
                    delimiter, level, delimiter,
                    delimiter, damage,
                    delimiter, scale,
                    delimiter, duration);

            level++;
            ScaleWithPlayerStats(ps);

            string next = string.Format("{0}{1}Next level: {2}{3}{4}Dmg: {5}{6}Scale:{7: 0.00}{8}Duration: {9: 0.00}", name,
                    delimiter, level, delimiter,
                    delimiter, damage,
                    delimiter, scale,
                    delimiter, duration);

            level--;
            ScaleWithPlayerStats(ps);

            return current + string_delimiter + next;
        }
    }
}

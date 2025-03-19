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
    public class MeteorStrikeSpellScript : MonoBehaviour
    {
        AudioSource audio_source;
        public GameObject audio_hit_dummy;
        public GameObject hit_effect_prefab;

        SpellStats stats;

        float created_t, alive_t;
        float damage_begin_t, damage_end_t;
        public float duration_t, begin_t, end_t;

        public float damage_cooldown = 0.5f;

        List<GameObject> was_damaged;
        List<float> was_damaged_t;

        void Awake()
        {
            stats = GetComponent<SpellStats>();
            audio_source = GetComponent<AudioSource>();
            was_damaged = new List<GameObject>();
            was_damaged_t = new List<float>();
        }

        void Start()
        {
            Destroy(gameObject, stats.duration);
            audio_source.Play();
        }

        void Update()
        {
        }

        void OnTriggerStay(Collider collider)
        {
            if (stats.caster == null || collider.gameObject.tag == stats.caster.tag) {
                return;
            }

            bool damaged_before = false;
            for (int i = 0; i < was_damaged.Count; i++) {
                if (was_damaged[i] == collider.gameObject) {
                    if (Time.time - was_damaged_t[i] > damage_cooldown) {
                        was_damaged_t[i] = Time.time;
                        damaged_before = true;
                    } else {
                        return;
                    }
                }
            }

            if (!damaged_before) {
                was_damaged.Add(collider.gameObject);
                was_damaged_t.Add(Time.time);
            }


            Vector3 halfway_up_vec = Vector3.up * collider.gameObject.transform.lossyScale.y / 2.0f;

            GameObject hit_effect_sound = Instantiate(audio_hit_dummy, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
            GameObject hit_effect = Instantiate(hit_effect_prefab, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
            Destroy(hit_effect, 0.4f);
            Destroy(hit_effect_sound, 1);

            collider.SendMessage("OnHit", new HitInfo(Vector3.zero, stats.caster, stats.damage, stats.damage_type), SendMessageOptions.DontRequireReceiver);
        }
    }

    public class MeteorStrikeSpell : Spell
    {
        public MeteorStrikeSpell()
        {
            prefab_index = 3;
            sprite_index = 0;
            name = "MeteorStrike";
            level = 0;
            damage_base = 1; damage_per_level = 1;
            duration_base = 2; duration_per_level = 0.1f;
            cooldown_base = 8; cooldown_per_level = 0;
            scale_base = 0.25f; scale_per_level = 0.05f;
            damage_type = DamageType.Fire;
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
            RaycastHit hit_info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out hit_info, 500, 1 << 6)) {
                return;
            }

            GameObject instance = GameState.InstantiateGlobal(GameData.spell_prefabs[prefab_index], hit_info.point + Vector3.up * 0.25f, Quaternion.identity);
            instance.transform.localScale = new Vector3(instance.transform.localScale.x * scale, instance.transform.localScale.y, instance.transform.localScale.z * scale);

            SpellStats spell_stats = instance.GetComponent<SpellStats>();
            spell_stats.damage = damage;
            spell_stats.scale = scale;
            spell_stats.duration = duration;
            spell_stats.base_duration = 4;
            spell_stats.damage_type = damage_type;
            spell_stats.caster = parent.gameObject;
        }

        public override string GetDescriptionString(string delimiter)
        {
            return string.Format("Dmg: {0}{1}AoE:{2}{3}Duration: {4}s", damage, delimiter, scale, delimiter, duration);
        }

        public override string GetLevelUpDescriptionString(string delimiter)
        {
            return string.Format("+{0} damage{1}+{2}% AoE{3}+{4}s Duration", damage_per_level, delimiter,
                    (int)(scale_per_level * 100), delimiter, (duration_per_level));
        }
    }
}

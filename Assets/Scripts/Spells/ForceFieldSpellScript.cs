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
    public class ForceFieldSpellScript : MonoBehaviour
    {

        AudioSource audio_source;

        SpellStats stats;

        float created_t, alive_t;
        float damage_begin_t, damage_end_t;
        public float duration_t, begin_t, end_t;

        void Start()
        {
            created_t = Time.time;

            stats = GetComponent<SpellStats>();
            audio_source = GetComponent<AudioSource>();
            stats.caster.GetComponent<PlayerStats>().invulnerable = true;
            transform.SetParent(stats.caster.transform);
        }

        void Update()
        {
            if (Time.time - created_t > stats.duration) {
                stats.caster.GetComponent<PlayerStats>().invulnerable = false;
                Destroy(gameObject);
            }
        }

        void OnTriggerEnter(Collider collider) { }
    }

    public class ForceFieldSpell : Spell
    {
        public ForceFieldSpell()
        {
            prefab_index = 1;
            sprite_index = 1;
            name = "Force Field";
            level = 0;
            damage_base = 0; damage_per_level = 0;
            duration_base = 1; duration_per_level = 0.1f;
            cooldown_base = 4; cooldown_per_level = 0;
            scale_base = 1; scale_per_level = 0;
            damage_type = DamageType.Normal;

        }

        public override void ScaleWithPlayerStats(PlayerStats ps)
        {
            damage   = 0;
            scale    = 1;
            duration = (duration_per_level * level + duration_base) * ps.spell_duration;
            cooldown = (cooldown_per_level * level + cooldown_base) - (cooldown_base * ps.spell_cooldown);
        }

        public override void ScaleWithEnemyStats(EnemyStats es) { }

        public override void Use(Transform parent)
        {
            RaycastHit hit_info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out hit_info, 500, 1 << 6)) {
                return;
            }

            GameObject instance = GameState.InstantiateGlobal(GameData.spell_prefabs[prefab_index], parent.position + (Vector3.up * parent.lossyScale.y / 2), Quaternion.identity);
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
            return string.Format("Dmg: {0}{1}{2:0.00}s cooldown", damage, delimiter, scale, delimiter, cooldown);
        }

        public override string GetLevelUpDescriptionString(string delimiter)
        {
            return string.Format("+{0}s Duration", (duration_per_level));
        }
    }
}

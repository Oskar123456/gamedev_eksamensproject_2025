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

        float created_t;
        float alive_t, left_t, left_t_fraction;

        void Start()
        {
            created_t = Time.time;

            stats = GetComponent<SpellStats>();
            audio_source = GetComponent<AudioSource>();
            stats.caster.GetComponent<PlayerStats>().invulnerable = true;
            transform.SetParent(caster_stats.caster.transform);
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
        public override void ScaleWithPlayerStats(PlayerStats ps)
        {
            damage   = 0;
            scale    = 1;
            duration = (duration_per_level * level + duration_base) * ps.spell_duration;
            cooldown = (cooldown_per_level * level + cooldown_base) * ps.spell_cooldown;
        }

        public override void ScaleWithEnemyStats(EnemyStats es) { }

        public override void Use(Transform parent)
        {
            RaycastHit hit_info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out hit_info, 500, 1 << 6)) {
                Destroy(gameObject);
                return;
            }

            GameObject instance = Instantiate(GameData.spell_list[prefab_index], parent.position + (Vector3.up * parent.lossyScale.y / 2), Quaternion.identity);
            SpellStats spell_stats = instance.GetComponent<SpellStats>();
            spell_stats.damage = damage;
            spell_stats.scale = scale;
            spell_stats.duration = duration;
            spell_stats.cooldown = cooldown;
            spell_stats.damage_type = damage_type;
            spell_stats.attacker = parent.gameObject;
        }

        public override string GetDescriptionString(string delimiter)
        {
            return string.Format("{0}{1}Level: {2}{3}{4}Buff: InvulnDuration: {5}", name,
                    delimiter, level, delimiter,
                    delimiter, duration);
        }

        public override string GetLevelUpDescriptionString(string delimiter, string string_delimiter, PlayerStats ps)
        {
            return string.Format("{0}{1}Current level: {2}{3}Buff: Invuln{4}Duration: {5}", name,
                    delimiter, level, delimiter,
                    delimiter, duration);

            level++;
            ScaleWithPlayerStats(ps);

            return string.Format("{0}{1}Next level: {2}{3}Buff: Invuln{4}Duration: {5}", name,
                    delimiter, level, delimiter,
                    delimiter, duration + duration_per_level);

            level--;
            ScaleWithPlayerStats(ps);

            return current + string_delimiter + next;
        }
    }
}

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

using Attacks;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Spells
{
    public class Buff : MonoBehaviour
    {
        AudioSource audio_source;
        // public List<AudioClip> sounds;
        // public GameObject audio_hit_dummy;
        // public GameObject hit_effect_prefab;

        SpellBaseStats stats;
        CasterStats caster_stats;

        float created_t;
        float alive_t, left_t, left_t_fraction;

        void Start()
        {
            created_t = Time.time;

            stats = GetComponent<SpellBaseStats>();
            caster_stats = GetComponent<CasterStats>();
            audio_source = GetComponent<AudioSource>();

            caster_stats.caster.GetComponent<PlayerStats>().invulnerable = true;
            transform.SetParent(caster_stats.caster.transform);
            transform.position = transform.position + (Vector3.up * caster_stats.caster.transform.lossyScale.y);
        }

        void Update()
        {
            if (Time.time - created_t > stats.duration) {
                Destroy(gameObject);
                caster_stats.caster.GetComponent<PlayerStats>().invulnerable = false;
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            // if (collider.gameObject.tag == "Attack") {
            //     AttackScript ascr = collider.gameObject.GetComponent<AttackScript>();
            //     if (ascr.GetAttacker().tag == "Enemy") {
            //         Destroy(collider.gameObject);
            //     }
            // }

            // if (collider.gameObject.tag != "Enemy") {
            //     return;
            // }

//             Vector3 normal = Vector3.Normalize(collider.transform.position - transform.position) * 0.4f;

//             collider.gameObject.SendMessage("OnPush", normal);
        }
    }
}

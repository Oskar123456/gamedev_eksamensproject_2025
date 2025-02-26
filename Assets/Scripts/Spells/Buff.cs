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

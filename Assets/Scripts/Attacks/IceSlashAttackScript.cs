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
    public class IceSlashAttackScript : MonoBehaviour
    {
        public GameObject attack_effect_prefab;
        public GameObject hit_effect_prefab;
        public List<AudioClip> sounds;

        AudioSource audio_source;

        AttackBaseStats stats;
        AttackerStats attacker_stats;

        Vector3 origin;
        Vector3 halfway_up_vec;

        float created_t;
        float alive_t, left_t, left_t_fraction;

        List<GameObject> was_damaged;

        public void SetStats()
        {
            attacker_stats = GetComponent<AttackerStats>();

            stats.damage = stats.damage + attacker_stats.damage;
            stats.duration = stats.duration / attacker_stats.speed;
            stats.damage_begin_t = stats.damage_begin_t / attacker_stats.speed;
            stats.damage_end_t = stats.damage_end_t / attacker_stats.speed;
            stats.scale = stats.scale * attacker_stats.scale;
            stats.range = stats.range * attacker_stats.scale;
        }

        void Awake()
        {
            stats = GetComponent<AttackBaseStats>();
            was_damaged = new List<GameObject>();
            audio_source = GetComponent<AudioSource>();
        }

        void Start()
        {
            SetStats();

            created_t = Time.time;
            left_t = stats.duration;
            left_t_fraction = 1;

            origin = attacker_stats.attacker.transform.position;
            halfway_up_vec = Vector3.up * (attacker_stats.attacker.transform.lossyScale.y / 2.0f);

            transform.rotation = attacker_stats.attacker.transform.rotation * Quaternion.Euler(0, 0, -10);
            transform.localScale = transform.localScale * stats.scale;

            GameObject effect = Instantiate(attack_effect_prefab, transform.position + halfway_up_vec,
                    attacker_stats.attacker.transform.rotation * Quaternion.Euler(0, 0, -10), transform);

            effect.transform.localScale = new Vector3(effect.transform.localScale.x * stats.scale / MathF.Max(transform.localScale.x, 1),
                effect.transform.localScale.y * stats.scale / MathF.Max(transform.localScale.y, 1),
                effect.transform.localScale.z * stats.scale / MathF.Max(transform.localScale.z, 1));

            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.simulationSpeed = stats.base_duration / stats.duration;

            Destroy(effect, stats.duration);
            Destroy(gameObject, stats.duration);

            audio_source.clip = sounds[0];
            audio_source.Play();
        }

        void Update()
        {
            alive_t = Time.time - created_t;
        }

        void OnTriggerStay(Collider collider)
        {
            if (alive_t < stats.damage_begin_t || alive_t > stats.damage_end_t)
                return;

            if (collider.gameObject.tag == attacker_stats.attacker_tag) {
                return;
            }

            if (was_damaged.Contains(collider.gameObject))
                return;

            was_damaged.Add(collider.gameObject);

            Vector3 normal = Vector3.Normalize(collider.transform.position - origin);
            normal.y = 0;
            Vector3 halfway_up_vec = Vector3.up * collider.gameObject.transform.lossyScale.y / 2.0f;

            GameObject hit_effect = Instantiate(hit_effect_prefab, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
            Destroy(hit_effect, stats.hit_effect_duration);

            collider.SendMessage("OnHit", new AttackHitInfo(attacker_stats.entity_type, stats, Time.time, normal), SendMessageOptions.DontRequireReceiver);

            audio_source.clip = sounds[1];
            audio_source.Play();
        }
    }
}

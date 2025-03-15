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
    public class ThunderBossAttack : MonoBehaviour
    {
        public GameObject effect_prefab;
        public GameObject hit_effect_prefab;
        public GameObject audio_dummy;
        public GameObject audio_hit_dummy;

        SphereCollider coll;
        AttackStats stats;
        Vector3 origin;

        float created_t, alive_t;
        public float delay = 0.0f;
        public float effect_duration = 2.167f;
        bool created = false;

        List<GameObject> was_damaged;

        void Awake()
        {
            stats = GetComponent<AttackStats>();
            coll = GetComponent<SphereCollider>();
            coll.enabled = false;
        }

        void Start()
        {
            was_damaged = new List<GameObject>();

            created_t = Time.time;
            delay = delay / (stats.base_duration / stats.duration);
            effect_duration = effect_duration / (stats.base_duration / stats.duration);

            origin = stats.attacker.transform.position;

            transform.position = transform.parent.position + transform.parent.up * -1f;
            transform.localScale *= stats.scale;

            Destroy(gameObject, stats.duration);

            Debug.Log("duration : " + stats.duration + " scale: " + stats.scale + " pos: " + transform.position);
        }

        void Update()
        {
            alive_t = Time.time - created_t;

            if (alive_t < delay) {
                return;
            }

            float alive_t_frac = (alive_t - delay) / effect_duration;

            if (!created) {
                GameObject sound_effect = Instantiate(audio_dummy, transform.position, transform.rotation, stats.attacker.transform);
                Destroy(sound_effect, effect_duration);
                GameObject effect = Instantiate(effect_prefab, transform.position, transform.rotation, transform);
                Destroy(effect, effect_duration);
                effect.transform.localScale *= stats.scale;

                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.simulationSpeed = stats.base_duration / stats.duration;
                foreach (Transform t in transform) {
                    t.localScale *= stats.scale;
                    ps = t.GetComponent<ParticleSystem>();
                    main = ps.main;
                    main.simulationSpeed = stats.base_duration / stats.duration;
                }

                coll.enabled = true;
                created = true;
            }
        }

        void OnTriggerStay(Collider collider)
        {
            if (was_damaged.Contains(collider.gameObject)) {
                return;
            }

            if (stats.attacker == null || collider.gameObject.tag == stats.attacker.tag) {
                return;
            }

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

    public class MeleeChargeBossAttack : Attack
    {
        public MeleeChargeBossAttack()
        {
            prefab_index = 3;
            sprite_index = 3;
            name = "Charge Attack";
            level = 1;
            damage_base = 1; damage_per_level = 1;
            duration_base = 0.8f; duration_per_level = 0;
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
            Transform t = parent;
            bool is_player = (parent.gameObject.tag == "Player");
            if (is_player) {
                t = parent.Find("root/pelvis/Weapon/Staff01PolyArt").GetComponent<Transform>();
            }

            GameObject instance = GameState.InstantiateParented(GameData.attack_prefabs[prefab_index], parent.position, parent.rotation, t);

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

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
using Player;
using UnityEngine;

namespace Attacks
{
    public class MagicThrustScript : MonoBehaviour
    {
        public GameObject effect_prefab;
        public GameObject hit_effect_prefab;
        public GameObject audio_dummy_charge;
        public GameObject audio_dummy_thrust;
        public GameObject audio_hit_dummy;

        AudioSource audio_source;
        CapsuleCollider coll;
        AttackStats stats;
        Transform parent;
        Transform effect_trf;

        Vector3 origin;
        public float offs_start = 0.88f;
        public float offs_finish = 4f;

        float created_t, alive_t;
        bool created = false;
        public float delay = 0.5f;
        public float effect_duration = 0.16f;

        bool is_warrior = false;

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
            parent = transform.parent;
            Vector3 effect_off = Vector3.zero;

            bool is_player = (transform.parent.gameObject.tag == "Player");

            if (is_player) {
                Transform trf = transform.parent.Find("root/pelvis/Weapon/Staff01PolyArt");

                if (trf != null) {
                    parent = trf;
                    transform.position = trf.position;
                    transform.parent = trf;
                    transform.localPosition = (Vector3.up * -0.75f);
                    transform.localScale *= stats.scale * 8;
                    transform.localRotation = Quaternion.identity;
                    effect_off = (Vector3.up * -0.75f);
                } else {
                    Transform t = transform.parent.Find("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r/lowerarm_r/hand_r/weapon_r").GetComponent<Transform>();
                    delay = 0.138f;
                    effect_duration = 0.395f;
                    transform.position = t.position;
                    transform.parent = t;
                    transform.localPosition = (Vector3.down * -1.15f);
                    transform.localScale *= stats.scale * 8;
                    transform.localRotation = Quaternion.identity;
                    parent = t;
                    effect_off = (Vector3.down * -1.15f);
                    is_warrior = true;
                }
            }

            was_damaged = new List<GameObject>();

            created_t = Time.time;
            delay = delay / (stats.base_duration / stats.duration);
            effect_duration = effect_duration / (stats.base_duration / stats.duration);

            origin = transform.position;

            // Instantiate(audio_dummy_charge, transform.position, transform.rotation, parent);
            GameObject effect = Instantiate(effect_prefab, transform.position, transform.rotation, parent);
            effect_trf = effect.GetComponent<Transform>();

            effect_trf.localScale *= stats.scale * 8;
            ParticleSystem ps = effect.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.simulationSpeed = stats.base_duration / stats.duration;

            foreach (Transform t in effect_trf) {
                t.localScale *= stats.scale * 8;
                ps = t.GetComponent<ParticleSystem>();
                main = ps.main;
                main.simulationSpeed = stats.base_duration / stats.duration;
            }

            Destroy(gameObject, delay + effect_duration);
            Destroy(effect, delay + effect_duration);
        }

        void Update()
        {
            alive_t = Time.time - created_t;

            effect_trf.rotation = stats.attacker.transform.rotation;
            transform.rotation = stats.attacker.transform.rotation;

            if (alive_t < delay) {
                return;
            }

            float alive_t_frac = (alive_t - delay) / effect_duration;

            if (!created) {
                origin = transform.position + (is_warrior ? Vector3.down * 0.65f : Vector3.zero);
                Instantiate(audio_dummy_thrust, transform.position, transform.rotation, parent);
                coll.enabled = true;
                created = true;
            }

            transform.position = Vector3.Lerp(origin, origin + stats.attacker.transform.forward * offs_finish, alive_t_frac);
        }

        void OnTriggerStay(Collider collider)
        {
            if (was_damaged.Contains(collider.gameObject)) {
                return;
            }

            if (stats.attacker == null || collider.gameObject.tag == stats.attacker.tag) {
                return;
            }

            if (stats.attacker == null || collider.gameObject.tag == "Ignore") {
                return;
            }

            GameObject hit_effect_sound = Instantiate(audio_hit_dummy, collider.gameObject.transform.position, Quaternion.identity);
            GameObject hit_effect = Instantiate(hit_effect_prefab, collider.gameObject.transform.position, Quaternion.identity);
            Destroy(hit_effect, 0.4f);
            Destroy(hit_effect_sound, 1);

            was_damaged.Add(collider.gameObject);

            Vector3 normal = Vector3.Normalize(collider.transform.position - origin);
            normal.y = 0;

            collider.SendMessage("OnHit", new HitInfo(normal, stats.attacker, stats.damage, stats.damage_type), SendMessageOptions.DontRequireReceiver);
        }
    }

    public class MagicThrust : Attack
    {
        public MagicThrust()
        {
            prefab_index = 2;
            sprite_index = 4;
            name = "Jab";
            level = 0;
            stance = Stance.Thrust;
            damage_base = 1; damage_per_level = 1;
            duration_base = 0.76f; duration_per_level = 0;
            cooldown_base = 1; cooldown_per_level = 0;
            scale_base = 0.1f; scale_per_level = 0.0025f;
            range_base = 3.5f;
            damage_type = DamageType.Normal;
        }

        public override void ScaleWithPlayerStats(PlayerStats ps)
        {
            damage   = (damage_per_level * level + damage_base) + ps.attack_damage + ps.attack_damage_ice + ps.attack_damage_fire;
            scale    = (scale_per_level  * level + scale_base)  * ps.attack_scale;
            range    = range_base        * ps.attack_scale;
            duration = duration_base     / ps.attack_speed;
            cooldown = duration;
        }

        public override void ScaleWithEnemyStats(EnemyStats es) { }

        public override void Use(Transform parent)
        {
            Transform t = parent;

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
            return string.Format("Dmg: {0}{1}AoE:{2}", damage, delimiter, scale);
        }

        public override string GetLevelUpDescriptionString(string delimiter)
        {
            return string.Format("+{0} damage{1}+{2}% AoE", damage_per_level, delimiter, (int)(scale_per_level * 100));
        }
    }
}

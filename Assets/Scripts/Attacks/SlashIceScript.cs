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
    public class SlashIceScript : MonoBehaviour
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
            Transform t = transform.parent;
            bool is_player = (transform.parent.gameObject.tag == "Player");
            delay = delay / (stats.base_duration / stats.duration);

            if (is_player)
            {
                Transform probe = transform.parent.Find("root/pelvis/Weapon/Staff01PolyArt");

                if (probe != null)
                {
                    t = transform.parent.Find("root/pelvis/Weapon/Staff01PolyArt").GetComponent<Transform>();
                    transform.parent = t;
                    transform.localPosition = (Vector3.up * -0.6f);
                    transform.localScale *= stats.scale;
                    transform.localRotation = Quaternion.identity;
                }

                if (probe == null)
                {
                    t = transform.parent.Find("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r/lowerarm_r/hand_r/weapon_r").GetComponent<Transform>();
                    transform.parent = t;
                    delay = 0.096f;
                    effect_duration = 0.4f;
                    transform.localPosition = (Vector3.down * -1f);
                    transform.localScale *= stats.scale * 0.6f;
                    transform.localRotation = Quaternion.identity;
                }


            }

            was_damaged = new List<GameObject>();

            created_t = Time.time;
            effect_duration = effect_duration / (stats.base_duration / stats.duration);

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
                // Debug.Log("Create effect at : " + alive_t);
                Instantiate(audio_dummy, stats.attacker.transform.position + Vector3.up * (stats.attacker.transform.lossyScale.y / 2), transform.rotation, stats.attacker.transform);

                Quaternion rot = Quaternion.Euler(stats.attacker.transform.rotation.eulerAngles.x,
                        stats.attacker.transform.rotation.eulerAngles.y,
                        stats.attacker.transform.rotation.eulerAngles.z - 10);

                GameObject effect = Instantiate(effect_prefab, stats.attacker.transform.position + Vector3.up * (stats.attacker.transform.lossyScale.y / 2),
                        rot, stats.attacker.transform);
                Destroy(effect, effect_duration);
                effect.transform.localScale *= stats.scale;
                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.simulationSpeed = stats.base_duration / stats.duration;
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

            if (stats.attacker == null || collider.gameObject.tag == "Ignore") {
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

    public class SlashIce : Attack
    {
        public SlashIce()
        {
            prefab_index = 1;
            sprite_index = 1;
            name = "Ice Slash";
            level = 0;
            damage_base = 1; damage_per_level = 1;
            duration_base = 1; duration_per_level = 0;
            cooldown_base = 1; cooldown_per_level = 0;
            scale_base = 1; scale_per_level = 0.025f;
            range_base = 1.8f;
            damage_type = DamageType.Normal;
        }

        public override void ScaleWithPlayerStats(PlayerStats ps)
        {
            damage   = (damage_per_level * level + damage_base) + ps.attack_damage + ps.attack_damage_ice;
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

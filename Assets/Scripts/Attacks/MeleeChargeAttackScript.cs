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
    public class MeleeChargeAttackScript : MonoBehaviour
    {
        public GameObject effect_prefab;
        public GameObject hit_effect_prefab;
        public GameObject audio_dummy;
        public GameObject audio_hit_dummy;

        SphereCollider coll;
        AttackStats stats;
        Vector3 origin;

        float created_t, alive_t;
        public float delay = 0.33f;
        public float effect_duration = 1f;
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

            origin = stats.attacker.transform.position;

            bool is_player = (transform.parent.gameObject.tag == "Player");

            if (is_player) {
                Transform trf = transform.parent.Find("root/pelvis/Weapon/Staff01PolyArt");
                if (trf != null) {
                    transform.parent = trf;
                    transform.position = trf.position;
                    transform.localPosition = Vector3.down * 1.15f;
                } else {
                    Transform t = transform.parent.Find("root/pelvis/spine_01/spine_02/spine_03/clavicle_r/upperarm_r/lowerarm_r/hand_r/weapon_r").GetComponent<Transform>();
                    delay = 0.096f;
                    effect_duration = 0.437f;
                    transform.position = t.position;
                    transform.parent = t;
                    transform.localPosition = (Vector3.down * -1.15f);
                }
            }

            transform.localScale *= 1 + ((stats.scale - 1) / 2);

            created_t = Time.time;
            delay = delay / (stats.base_duration / stats.duration);
            effect_duration = effect_duration / (stats.base_duration / stats.duration);

            Destroy(gameObject, stats.duration);
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
                // effect.transform.localScale *= 1 + ((stats.scale - 1) / 2);

                ParticleSystem ps = effect.GetComponent<ParticleSystem>();
                var main = ps.main;
                main.simulationSpeed = stats.base_duration / stats.duration;
                foreach (Transform t in transform) {
                    // t.localScale *= 1 + (stats.scale - 1) / 2;
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

            if (stats.attacker == null || collider.gameObject.tag == "Ignore") {
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

    public class MeleeChargeAttack : Attack
    {
        public MeleeChargeAttack()
        {
            prefab_index = 3;
            sprite_index = 3;
            name = "Charge Attack";
            level = 0;
            damage_base = 1; damage_per_level = 1;
            duration_base = 0.8f; duration_per_level = 0;
            cooldown_base = 1; cooldown_per_level = 0;
            scale_base = 1; scale_per_level = 0.1f;
            range_base = 1.65f;
            damage_type = DamageType.Normal;

        }

        public override void ScaleWithPlayerStats(PlayerStats ps)
        {
            damage   = (damage_per_level * level + damage_base) + ps.attack_damage;
            scale    = (scale_per_level  * level + scale_base)  * ps.attack_scale;
            range    = range_base        * (1 + ((ps.attack_scale - 1) / 2));
            duration = duration_base     / ps.attack_speed;
            cooldown = duration;
        }

        public override void ScaleWithEnemyStats(EnemyStats es)
        {
            damage   = damage_base   + es.attack_damage;
            scale    = scale_base    * es.attack_scale;
            range    = range_base    * (1 + ((es.attack_scale - 1) / 2));
            duration = duration_base / es.attack_speed;
            cooldown = duration;
        }

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

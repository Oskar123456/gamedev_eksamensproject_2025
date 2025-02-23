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
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    public GameObject attack_effect_prefab;
    public GameObject hit_effect_prefab;
    public float yaw, pitch, roll;
    // public float end_yaw, end_pitch, end_roll;
    public float offset_forward, offset_right, offset_up;
    // public float acceleration;

    // public Vector3 rotation_axis;
    // public float rotation_degrees;

    Vector3 origin, position, end_position;
    Quaternion rotation, end_rotation;

    AttackStats stats;
    GameObject attacker;

    float alive_t, left_t, left_t_fraction;

    public void SetStats(AttackStats ats) { stats = ats; }
    public AttackStats GetStats() { return stats; }

    public void SetAttacker(GameObject a) { attacker = a; }
    public GameObject GetAttacker() { return attacker; }

    List<GameObject> was_damaged;

    void Awake()
    {
        was_damaged = new List<GameObject>();
    }

    void Start()
    {
        stats.created_t = Time.time;
        left_t = stats.duration;
        left_t_fraction = 1;

        GameObject effect = Instantiate(attack_effect_prefab, transform.position, attacker.transform.rotation * Quaternion.Euler(yaw, pitch, roll));

        transform.rotation = attacker.transform.rotation;
        position = transform.position + offset_forward * attacker.transform.forward + offset_up * attacker.transform.up + offset_right * attacker.transform.right;
        transform.position = position;
        transform.localScale = transform.localScale * stats.scale;

        Destroy(effect, stats.duration);
        Destroy(gameObject, stats.duration);
    }

    void Update()
    {
        alive_t = Time.time - stats.created_t;
    }

    void OnTriggerStay(Collider collider)
    {
        if (alive_t < stats.damage_begin_t || alive_t > stats.damage_end_t)
            return;

        if (collider.gameObject == attacker) {
            return;
        }

        if (was_damaged.Contains(collider.gameObject))
            return;

        was_damaged.Add(collider.gameObject);

        Vector3 normal = Vector3.Normalize(collider.transform.position - transform.position);
        Vector3 halfway_up_vec = Vector3.up * collider.gameObject.transform.lossyScale.y / 2.0f;

        GameObject hit_effect = Instantiate(hit_effect_prefab, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
        Destroy(hit_effect, stats.hit_effect_duration);

        collider.SendMessage("OnHit", new AttackHitInfo(stats, Time.time, normal), SendMessageOptions.DontRequireReceiver);
    }
}

public class AttackStats
{
    public EntityType entity_type = EntityType.None;
    public int damage = 1;
    public float duration = 0.25f;
    public float hit_effect_duration = 0.25f;
    public float cooldown = 0.25f;
    public float created_t, damage_begin_t = 0.05f, damage_end_t = 0.25f;
    public float scale = 1;
    public float range = 1;

    public static AttackStats Ghost()
    {
        AttackStats ats = new AttackStats();
        ats.entity_type = EntityType.Enemy;
        ats.damage = 1;
        ats.duration = 1;
        ats.hit_effect_duration = 0.25f;
        ats.cooldown = 1;
        ats.damage_begin_t = 0.50f;
        ats.damage_end_t = 0.70f;
        ats.range = 6;
        ats.scale = 1;
        return ats;
    }
}

public class AttackHitInfo
{
    public AttackStats stats;
    public float when_t;
    public Vector3 normal;

    public AttackHitInfo(AttackStats stats, float when_t, Vector3 normal)
    {
        this.stats = stats;
        this.when_t = when_t;
        this.normal = normal;
    }
}

public enum EntityType { None, Player, Enemy }

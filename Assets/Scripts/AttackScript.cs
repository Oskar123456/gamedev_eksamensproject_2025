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
    public float offset_forward, offset_right, offset_up;

    Vector3 origin, position, end_position;
    Quaternion rotation, end_rotation;

    AttackStats stats;
    GameObject attacker;
    string attacker_tag;

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

        attacker_tag = attacker.tag;

        GameObject effect = Instantiate(attack_effect_prefab, transform.position, attacker.transform.rotation * Quaternion.Euler(yaw, pitch, roll));
        effect.transform.localScale = effect.transform.localScale * stats.scale;
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.simulationSpeed = stats.base_duration / stats.duration;

        origin = attacker.transform.position;
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

        if (collider.gameObject?.tag == attacker_tag) {
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

        collider.SendMessage("OnHit", new AttackHitInfo(stats, Time.time, normal), SendMessageOptions.DontRequireReceiver);
    }
}

public class AttackStats
{
    public EntityType entity_type = EntityType.None;
    public int damage = 1;
    public float base_duration = 0.25f;
    public float duration = 0.4f;
    public float hit_effect_duration = 0.4f;
    public float cooldown = 0.4f;
    public float created_t, damage_begin_t = 0.08f, damage_end_t = 0.4f;
    public float scale = 1;
    public float range = 1;

    public static AttackStats Ghost()
    {
        AttackStats ats = new AttackStats();
        ats.entity_type = EntityType.Enemy;
        ats.damage = 1 * GameState.level;
        ats.duration = 1 - (MathF.Min(0.8f, MathF.Sqrt(GameState.level) * 0.1f));
        ats.base_duration = 1;
        ats.hit_effect_duration = ats.duration / 4.0f;
        ats.cooldown = ats.duration;
        ats.damage_begin_t = ats.duration * 0.7f;
        ats.damage_end_t = ats.duration;
        ats.range = 5f;
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

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
    public List<AudioClip> sounds;
    public float yaw, pitch, roll;
    public float yaw_effect, pitch_effect, roll_effect;
    public float offset_forward, offset_right, offset_up;
    public float offset_forward_effect, offset_right_effect, offset_up_effect;

    AudioSource audio_source;

    Vector3 origin, position, end_position;
    Quaternion rotation, end_rotation;

    AttackBaseStats base_stats;
    GameObject attacker;
    string attacker_tag;
    EntityType attacker_entity_type;

    float created_t;
    float alive_t, left_t, left_t_fraction;

    public void SetStats(AttackStats attacker_stats)
    {
        base_stats.damage = base_stats.damage + attacker_stats.damage;
        base_stats.duration = base_stats.duration / attacker_stats.speed;
        base_stats.damage_begin_t = base_stats.damage_begin_t / attacker_stats.speed;
        base_stats.damage_end_t = base_stats.damage_end_t / attacker_stats.speed;
        base_stats.scale = base_stats.scale * attacker_stats.scale;
        base_stats.range = base_stats.range * attacker_stats.scale;
    }

    public AttackBaseStats GetStats() { return base_stats; }

    public void SetAttackerEntityType(EntityType et) { attacker_entity_type = et; }
    public void SetAttacker(GameObject a) { attacker = a; }
    public GameObject GetAttacker() { return attacker; }

    List<GameObject> was_damaged;

    void Awake()
    {
        base_stats = GetComponent<AttackBaseStats>();
        was_damaged = new List<GameObject>();
        audio_source = GetComponent<AudioSource>();
    }

    void Start()
    {
        created_t = Time.time;
        left_t = base_stats.duration;
        left_t_fraction = 1;

        attacker_tag = attacker.tag;

        GameObject effect = Instantiate(attack_effect_prefab, transform.position,
                attacker.transform.rotation * Quaternion.Euler(yaw_effect, pitch_effect, roll_effect));
        effect.transform.localScale = effect.transform.localScale * base_stats.scale;

        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.simulationSpeed = base_stats.base_duration / base_stats.duration;

        origin = attacker.transform.position;
        transform.rotation = attacker.transform.rotation;
        position = transform.position + offset_forward * attacker.transform.forward + offset_up * attacker.transform.up + offset_right * attacker.transform.right;
        transform.position = position;
        transform.localScale = transform.localScale * base_stats.scale;

        Destroy(effect, base_stats.duration);
        Destroy(gameObject, base_stats.duration);

        audio_source.clip = sounds[0];
        audio_source.Play();
    }

    void Update()
    {
        alive_t = Time.time - created_t;
    }

    void OnTriggerStay(Collider collider)
    {
        if (alive_t < base_stats.damage_begin_t || alive_t > base_stats.damage_end_t)
            return;

        if (collider.gameObject.tag == attacker_tag) {
            return;
        }

        if (was_damaged.Contains(collider.gameObject))
            return;

        was_damaged.Add(collider.gameObject);

        Vector3 normal = Vector3.Normalize(collider.transform.position - origin);
        normal.y = 0;
        Vector3 halfway_up_vec = Vector3.up * collider.gameObject.transform.lossyScale.y / 2.0f;

        GameObject hit_effect = Instantiate(hit_effect_prefab, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
        Destroy(hit_effect, base_stats.hit_effect_duration);

        collider.SendMessage("OnHit", new AttackHitInfo(attacker_entity_type, base_stats, Time.time, normal), SendMessageOptions.DontRequireReceiver);

        audio_source.clip = sounds[1];
        audio_source.Play();
    }
}

public class AttackHitInfo
{
    public EntityType entity_type;
    public AttackBaseStats stats;
    public float when_t;
    public Vector3 normal;

    public AttackHitInfo(EntityType et, AttackBaseStats stats, float when_t, Vector3 normal)
    {
        this.entity_type = et;
        this.stats = stats;
        this.when_t = when_t;
        this.normal = normal;
    }
}

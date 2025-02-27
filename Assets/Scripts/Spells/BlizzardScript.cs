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

public class BlizzardScript : MonoBehaviour
{
    AudioSource audio_source;
    public List<AudioClip> sounds;
    public GameObject audio_hit_dummy;
    // public GameObject spell_effect_prefab;
    public GameObject hit_effect_prefab;

    SpellBaseStats stats;
    CasterStats caster_stats;

    float created_t;
    float alive_t, left_t, left_t_fraction;

    public float damage_cooldown = 0.5f;

    List<GameObject> was_damaged;
    List<float> was_damaged_t;

    void Awake()
    {
        stats = GetComponent<SpellBaseStats>();
        caster_stats = GetComponent<CasterStats>();
        audio_source = GetComponent<AudioSource>();
        was_damaged = new List<GameObject>();
        was_damaged_t = new List<float>();
    }

    void Start()
    {
        created_t = Time.time;
        left_t = stats.GetDuration(caster_stats);
        left_t_fraction = 1;

        RaycastHit hit_info;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit_info, 500, 1 << 6)) {
            Destroy(gameObject);
            return;
        }

        transform.position = hit_info.point;

        Destroy(gameObject, stats.GetDuration(caster_stats));

        audio_source.clip = sounds[0];
        audio_source.Play();
    }

    void Update()
    {
        alive_t = Time.time - created_t;
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == caster_stats.caster_tag) {
            return;
        }

        bool damaged_before = false;
        for (int i = 0; i < was_damaged.Count; i++) {
            if (was_damaged[i] == collider.gameObject) {
                if (Time.time - was_damaged_t[i] > damage_cooldown) {
                    was_damaged_t[i] = Time.time;
                    damaged_before = true;
                } else {
                    return;
                }
            }
        }

        if (!damaged_before) {
            was_damaged.Add(collider.gameObject);
            was_damaged_t.Add(Time.time);
        }


        Vector3 halfway_up_vec = Vector3.up * collider.gameObject.transform.lossyScale.y / 2.0f;

        GameObject hit_effect_sound = Instantiate(audio_hit_dummy, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
        GameObject hit_effect = Instantiate(hit_effect_prefab, collider.gameObject.transform.position + halfway_up_vec, Quaternion.identity);
        Destroy(hit_effect, stats.hit_effect_duration);
        Destroy(hit_effect_sound, 1);

        collider.SendMessage("OnHit", new AttackHitInfo(caster_stats.entity_type, stats, caster_stats, Time.time, Vector3.zero), SendMessageOptions.DontRequireReceiver);
    }
}

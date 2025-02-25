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

public class AttackBaseStats : MonoBehaviour
{
    public int damage = 1;
    public float base_duration = 0.25f;
    public float cooldown = 0.4f;
    public float duration = 0.4f;
    public float hit_effect_duration = 0.4f;
    public float damage_begin_t = 0.08f, damage_end_t = 0.4f;
    public float scale = 1;
    public float range = 1;
}

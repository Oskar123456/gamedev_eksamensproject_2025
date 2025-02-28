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

namespace Spells
{
    public class SpellStats : MonoBehaviour
    {
        public int damage;
        public float scale;
        public float duration;
        public float base_duration;
        public DamageType damage_type;
        public GameObject caster;
    }
}


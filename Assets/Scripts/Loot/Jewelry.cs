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
 *
 * */

using System;
using System.Collections.Generic;
using Attacks;
using Spells;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Loot
{

    public class Amulet : Jewelry
    {
        public Amulet()
        {
            should_show = true;
            weight = 500;
            prefab_index = 1;
            sprite_index = 12;
            name = "Amulet";
            description = "A mysterious heirloom";
            hp = GameState.rng.Next() % 25 + 1;
            spell_damage = GameState.rng.Next() % 3 + 1;
            spell_speed = (GameState.rng.Next() % 3 + 1) * 0.1f;
            spell_scale = (GameState.rng.Next() % 3 + 1) * 0.1f;
            attack_damage = GameState.rng.Next() % 3 + 1;
            attack_speed = (GameState.rng.Next() % 3 + 1) * 0.1f;
            attack_scale = (GameState.rng.Next() % 3 + 1) * 0.1f;
        }
    }

}


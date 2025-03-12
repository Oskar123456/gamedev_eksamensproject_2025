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

    public class Staff : Weapon
    {
        public Staff()
        {
            should_show = true;
            weight = 500;
            prefab_index = 1;
            sprite_index = 7;
            name = "staff";
            description = "a good staff";
            spell_damage = GameState.rng.Next() % 3 + 1;
            spell_speed = (GameState.rng.Next() % 3 + 1) * 0.1f;
            spell_scale = (GameState.rng.Next() % 3 + 1) * 0.1f;
        }
    }

    public class Axe : Weapon
    {
        public Axe()
        {
            should_show = true;
            weight = 500;
            prefab_index = 1;
            sprite_index = 22;
            name = "Axe";
            description = "A simple axe";
            attack_damage = GameState.rng.Next() % 3 + 1;
            attack_speed = (GameState.rng.Next() % 6 + 1) * 0.1f;
            attack_scale = (GameState.rng.Next() % 3 + 1) * 0.1f;
        }
    }

    public class SteelAxe : Weapon
    {
        public SteelAxe()
        {
            should_show = true;
            weight = 250;
            prefab_index = 1;
            sprite_index = 24;
            name = "Steel Axe";
            description = "A steel axe";
            attack_damage = GameState.rng.Next() % 6 + 1;
            attack_speed = (GameState.rng.Next() % 1 + 1) * 0.1f;
            attack_scale = (GameState.rng.Next() % 6 + 1) * 0.1f;
        }
    }

}

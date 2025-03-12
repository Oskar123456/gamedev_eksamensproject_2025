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

    public class LeatherBoots : Boots
    {
        public LeatherBoots()
        {
            should_show = true;
            weight = 500;
            prefab_index = 1;
            sprite_index = 17;
            name = "Leather Boots";
            description = "Simply boots";
            hp = GameState.rng.Next() % 5 + 1;
            move_speed_bonus = (GameState.rng.Next() % 35 + 1) / 100.0f + 0.1f;
        }
    }

}


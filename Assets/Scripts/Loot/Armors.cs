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

    public class LeatherArmor : Armor
    {
        public LeatherArmor()
        {
            should_show = true;
            weight = 500;
            prefab_index = 1;
            sprite_index = 15;
            name = "Leather Armor";
            description = "Thick leather armor";
            hp = GameState.rng.Next() % 5 + 1;
        }
    }

}


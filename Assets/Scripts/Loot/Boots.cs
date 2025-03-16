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
            move_speed_bonus = (GameState.rng.Next() % 25 + 1) / 100.0f + 0.1f;
        }
    }

    public class BanditBoots : Boots
    {
        public BanditBoots()
        {
            should_show = true;
            weight = 150;
            prefab_index = 1;
            sprite_index = 34;
            name = "Bandit Boots";
            description = "A bandit's boots";
            hp = GameState.rng.Next() % 15 + 1;
            move_speed_bonus = (GameState.rng.Next() % 35 + 1) / 100.0f + 0.1f;
        }
    }

    public class ScoutBoots : Boots
    {
        public ScoutBoots()
        {
            should_show = true;
            weight = 150;
            prefab_index = 1;
            sprite_index = 33;
            name = "Scout Boots";
            description = "A scout's boots";
            hp = GameState.rng.Next() % 15 + 1;
            move_speed_bonus = (GameState.rng.Next() % 65 + 1) / 100.0f + 0.1f;
        }
    }

    public class BattleGuardBoots : Boots
    {
        public BattleGuardBoots()
        {
            should_show = true;
            weight = 50;
            prefab_index = 1;
            sprite_index = 35;
            name = "Battle Guard Boots";
            description = "Thick steel plated boots";
            hp = GameState.rng.Next() % 25 + 1;
            move_speed_bonus = (GameState.rng.Next() % 35 + 1) / 100.0f + 0.1f;
        }
    }

    public class DarkMountainBoots : Boots
    {
        public DarkMountainBoots()
        {
            should_show = true;
            weight = 50;
            prefab_index = 1;
            sprite_index = 36;
            name = "Dark Mountain Boots";
            description = "Forged in mount doom";
            hp = GameState.rng.Next() % 55 + 1;
            move_speed_bonus = (GameState.rng.Next() % 35 + 1) / 100.0f + 0.1f;
        }
    }

    public class GreyKnightBoots : Boots
    {
        public GreyKnightBoots()
        {
            should_show = true;
            weight = 50;
            prefab_index = 1;
            sprite_index = 37;
            name = "Grey Knight Boots";
            description = "Battle hardened";
            hp = GameState.rng.Next() % 55 + 1;
            move_speed_bonus = (GameState.rng.Next() % 35 + 1) / 100.0f + 0.1f;
        }
    }

    public class ManticoreBoots : Boots
    {
        public ManticoreBoots()
        {
            should_show = true;
            weight = 25;
            prefab_index = 1;
            sprite_index = 38;
            name = "Manticore Boots";
            description = "Exquisite boots";
            hp = GameState.rng.Next() % 55 + 1;
            move_speed_bonus = (GameState.rng.Next() % 45 + 1) / 100.0f + 0.1f;
        }
    }

}


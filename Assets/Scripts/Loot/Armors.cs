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

    public class BanditArmor : Armor
    {
        public BanditArmor()
        {
            should_show = true;
            weight = 250;
            prefab_index = 1;
            sprite_index = 26;
            name = "Bandit Armor";
            description = "A bandit's armor";
            hp = GameState.rng.Next() % 15 + 1;
            hit_recovery = (GameState.rng.Next() % 2 + 1) * 0.01f;
            defense = GameState.rng.Next() % 2 + 1;
        }
    }

    public class ScoutArmor : Armor
    {
        public ScoutArmor()
        {
            should_show = true;
            weight = 250;
            prefab_index = 1;
            sprite_index = 32;
            name = "Scout Armor";
            description = "A Scout's armor";
            hp = GameState.rng.Next() % 15 + 1;
            hit_recovery = (GameState.rng.Next() % 5 + 1) * 0.01f;
        }
    }

    public class BattleGuardArmor : Armor
    {
        public BattleGuardArmor()
        {
            should_show = true;
            weight = 50;
            prefab_index = 1;
            sprite_index = 27;
            name = "Battle Guard Armor";
            description = "Thick steel plated armor";
            hp = GameState.rng.Next() % 55 + 1;
            hit_recovery = (GameState.rng.Next() % 5 + 1) * 0.01f;
            defense = GameState.rng.Next() % 2 + 1;
        }
    }

    public class DarkMountainArmor : Armor
    {
        public DarkMountainArmor()
        {
            should_show = true;
            weight = 50;
            prefab_index = 1;
            sprite_index = 28;
            name = "Dark Mountain Armor";
            description = "Forged in mount doom";
            hp = GameState.rng.Next() % 55 + 1;
            hit_recovery = (GameState.rng.Next() % 4 + 1) * 0.01f;
            defense = GameState.rng.Next() % 7 + 1;
        }
    }

    public class GreyKnightArmor : Armor
    {
        public GreyKnightArmor()
        {
            should_show = true;
            weight = 50;
            prefab_index = 1;
            sprite_index = 29;
            name = "Grey Knight Armor";
            description = "Thick battle hardened armor";
            hp = GameState.rng.Next() % 75 + 1;
            hit_recovery = (GameState.rng.Next() % 5 + 1) * 0.01f;
            defense = GameState.rng.Next() % 8 + 1;
        }
    }

    public class ManticoreArmor : Armor
    {
        public ManticoreArmor()
        {
            should_show = true;
            weight = 50;
            prefab_index = 1;
            sprite_index = 30;
            name = "Manticore Armor";
            description = "Thick sturdy armor";
            hp = GameState.rng.Next() % 85 + 1;
            hit_recovery = (GameState.rng.Next() % 5 + 1) * 0.01f;
            defense = GameState.rng.Next() % 6 + 1;
        }
    }

    public class RoyalTunicArmor : Armor
    {
        public RoyalTunicArmor()
        {
            should_show = true;
            weight = 25;
            prefab_index = 1;
            sprite_index = 31;
            name = "Royal Tunic";
            description = "Exquisite armor, made of the finest materials";
            hp = GameState.rng.Next() % 115 + 1;
            hit_recovery = (GameState.rng.Next() % 5 + 1) * 0.01f;
            defense = GameState.rng.Next() % 10 + 1;
        }
    }

}


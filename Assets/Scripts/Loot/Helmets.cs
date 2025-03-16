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

    public class LeatherHelmet : Helmet
    {
        public LeatherHelmet()
        {
            should_show = true;
            weight = 500;
            prefab_index = 1;
            sprite_index = 20;
            name = "Leather helmet";
            description = "Helmet made of hardened leather";
            hp = GameState.rng.Next() % 5 + 1;
        }
    }

    public class BattleGuardHelmet : Helmet
    {
        public BattleGuardHelmet()
        {
            should_show = true;
            weight = 250;
            prefab_index = 1;
            sprite_index = 39;
            name = "BattleGuard Helmet";
            description = "Helmet made of iron";
            hp = GameState.rng.Next() % 25 + 1;
            hit_recovery = (GameState.rng.Next() % 1 + 1) * 0.01f;
            defense = GameState.rng.Next() % 1 + 1;
        }
    }

    public class DarkMountainHelmet : Helmet
    {
        public DarkMountainHelmet()
        {
            should_show = true;
            weight = 250;
            prefab_index = 1;
            sprite_index = 40;
            name = "Dark Mountain Helmet";
            description = "Helmet made of iron";
            hp = GameState.rng.Next() % 35 + 1;
            hit_recovery = (GameState.rng.Next() % 2 + 1) * 0.01f;
            defense = GameState.rng.Next() % 4 + 1;
        }
    }

    public class HawkHelmet : Helmet
    {
        public HawkHelmet()
        {
            should_show = true;
            weight = 250;
            prefab_index = 1;
            sprite_index = 41;
            name = "Hawk Helmet";
            description = "Helmet made of feathers";
            hp = GameState.rng.Next() % 35 + 1;
            hit_recovery = (GameState.rng.Next() % 4 + 1) * 0.01f;
            defense = GameState.rng.Next() % 2 + 1;
        }
    }

    public class KnightHelmet : Helmet
    {
        public KnightHelmet()
        {
            should_show = true;
            weight = 150;
            prefab_index = 1;
            sprite_index = 42;
            name = "Knight's Helmet";
            description = "Helmet made for a knight";
            hp = GameState.rng.Next() % 55 + 1;
            hit_recovery = (GameState.rng.Next() % 2 + 1) * 0.01f;
            defense = GameState.rng.Next() % 5 + 1;
        }
    }

    public class HighKnightHelmet : Helmet
    {
        public HighKnightHelmet()
        {
            should_show = true;
            weight = 100;
            prefab_index = 1;
            sprite_index = 43;
            name = "Fancy Knight's Helmet";
            description = "Helmet made for a fancy knight";
            hp = GameState.rng.Next() % 75 + 1;
            hit_recovery = (GameState.rng.Next() % 2 + 1) * 0.01f;
            defense = GameState.rng.Next() % 7 + 1;
        }
    }

    public class ManticoreHelmet : Helmet
    {
        public ManticoreHelmet()
        {
            should_show = true;
            weight = 150;
            prefab_index = 1;
            sprite_index = 44;
            name = "Manticore Helmet";
            description = "Helmet made for a carnivore";
            hp = GameState.rng.Next() % 55 + 1;
            hit_recovery = (GameState.rng.Next() % 2 + 1) * 0.01f;
            defense = GameState.rng.Next() % 5 + 1;
        }
    }

    public class ScoutHelmet : Helmet
    {
        public ScoutHelmet()
        {
            should_show = true;
            weight = 350;
            prefab_index = 1;
            sprite_index = 45;
            name = "Scout's Helmet";
            description = "Helmet made for a scout";
            hp = GameState.rng.Next() % 25 + 1;
            hit_recovery = (GameState.rng.Next() % 2 + 1) * 0.01f;
        }
    }

}


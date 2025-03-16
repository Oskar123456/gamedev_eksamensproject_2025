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
            weight = 25;
            prefab_index = 1;
            sprite_index = 12;
            name = "Amulet";
            description = "A mysterious heirloom";
            hp = GameState.rng.Next() % 55;
            spell_damage = GameState.rng.Next() % 4;
            spell_damage_ice = GameState.rng.Next() % 4;
            spell_damage_fire = GameState.rng.Next() % 4;
            spell_speed = (GameState.rng.Next() % 4) * 0.1f;
            spell_scale = (GameState.rng.Next() % 4) * 0.1f;
            attack_damage = GameState.rng.Next() % 4;
            attack_damage_ice = GameState.rng.Next() % 4;
            attack_damage_fire = GameState.rng.Next() % 4;
            attack_speed = (GameState.rng.Next() % 4) * 0.1f;
            attack_scale = (GameState.rng.Next() % 4) * 0.1f;
        }
    }

    public class BronzeRing : Jewelry
    {
        public BronzeRing()
        {
            should_show = true;
            weight = 50;
            prefab_index = 1;
            sprite_index = 48;
            name = "A Bronze Ring";
            description = "A mysterious heirloom";
            hp = GameState.rng.Next() % 15;
            spell_damage = GameState.rng.Next() % 1;
            spell_damage_ice = GameState.rng.Next() % 1;
            spell_damage_fire = GameState.rng.Next() % 1;
            spell_speed = (GameState.rng.Next() % 1) * 0.1f;
            spell_scale = (GameState.rng.Next() % 1) * 0.1f;
            attack_damage = GameState.rng.Next() % 1;
            attack_damage_ice = GameState.rng.Next() % 1;
            attack_damage_fire = GameState.rng.Next() % 1;
            attack_speed = (GameState.rng.Next() % 1) * 0.1f;
            attack_scale = (GameState.rng.Next() % 1) * 0.1f;
        }
    }

    public class SilverRing : Jewelry
    {
        public SilverRing()
        {
            should_show = true;
            weight = 25;
            prefab_index = 1;
            sprite_index = 49;
            name = "A Silver Ring";
            description = "A mysterious heirloom";
            hp = GameState.rng.Next() % 25;
            spell_damage = GameState.rng.Next() % 2;
            spell_damage_ice = GameState.rng.Next() % 2;
            spell_damage_fire = GameState.rng.Next() % 2;
            spell_speed = (GameState.rng.Next() % 2) * 0.1f;
            spell_scale = (GameState.rng.Next() % 2) * 0.1f;
            attack_damage = GameState.rng.Next() % 2;
            attack_damage_ice = GameState.rng.Next() % 2;
            attack_damage_fire = GameState.rng.Next() % 2;
            attack_speed = (GameState.rng.Next() % 2) * 0.1f;
            attack_scale = (GameState.rng.Next() % 2) * 0.1f;
        }
    }

    public class GoldRing : Jewelry
    {
        public GoldRing()
        {
            should_show = true;
            weight = 10;
            prefab_index = 1;
            sprite_index = 50;
            name = "A Gold Ring";
            description = "A mysterious heirloom";
            hp = GameState.rng.Next() % 45;
            spell_damage = GameState.rng.Next() % 4;
            spell_damage_ice = GameState.rng.Next() % 4;
            spell_damage_fire = GameState.rng.Next() % 4;
            spell_speed = (GameState.rng.Next() % 4) * 0.1f;
            spell_scale = (GameState.rng.Next() % 4) * 0.1f;
            attack_damage = GameState.rng.Next() % 4;
            attack_damage_ice = GameState.rng.Next() % 4;
            attack_damage_fire = GameState.rng.Next() % 4;
            attack_speed = (GameState.rng.Next() % 4) * 0.1f;
            attack_scale = (GameState.rng.Next() % 4) * 0.1f;
        }
    }

}


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
            spell_damage = GameState.rng.Next() % 2;
            spell_speed = (GameState.rng.Next() % 2) * 0.1f;
            spell_scale = (GameState.rng.Next() % 2) * 0.1f;
            spell_cooldown = (GameState.rng.Next() % 2) * 0.1f;
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
            attack_damage = GameState.rng.Next() % 2;
            attack_speed = (GameState.rng.Next() % 3) * 0.1f;
            attack_scale = (GameState.rng.Next() % 2) * 0.1f;
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
            attack_damage = GameState.rng.Next() % 3;
            attack_speed = (GameState.rng.Next() % 1) * 0.1f;
            attack_scale = (GameState.rng.Next() % 3) * 0.1f;
        }
    }

     public class DivineDemonStaff : Weapon
    {
        public DivineDemonStaff()
        {
            should_show = true;
            weight = 500;
            prefab_index = 1;
            sprite_index = 0;
            name = "Demon King's Staff";
            description = "A legendary demon king's staff";
            spell_damage = GameState.rng.Next() % 10 + 5;
            spell_speed = (GameState.rng.Next() % 10 + 5) * 0.1f;
            spell_scale = (GameState.rng.Next() % 10 + 5) * 0.1f;
            spell_cooldown = (GameState.rng.Next() % 8) * 0.1f;
        }
    }

    public class InfantrySword : Weapon
    {
        public InfantrySword()
        {
            should_show = true;
            weight = 150;
            prefab_index = 1;
            sprite_index = 51;
            name = "Infantry Sword";
            description = "A simple infantry sword";
            attack_damage = GameState.rng.Next() % 2;
            attack_speed = (GameState.rng.Next() % 6) * 0.1f;
            attack_scale = (GameState.rng.Next() % 1) * 0.1f;
        }
    }

    public class VikingAxe : Weapon
    {
        public VikingAxe()
        {
            should_show = true;
            weight = 100;
            prefab_index = 1;
            sprite_index = 52;
            name = "Viking Axe";
            description = "A viking's axe, simple, but heavy";
            attack_damage = GameState.rng.Next() % 8;
            attack_speed = (GameState.rng.Next() % 1) * 0.1f;
            attack_scale = (GameState.rng.Next() % 6) * 0.1f;
        }
    }

    public class MageStaff : Weapon
    {
        public MageStaff()
        {
            should_show = true;
            weight = 50;
            prefab_index = 1;
            sprite_index = 53;
            name = "Mage Staff";
            description = "A mage's staff";
            spell_damage = GameState.rng.Next() % 3;
            spell_damage_ice = GameState.rng.Next() % 3;
            spell_damage_fire = GameState.rng.Next() % 3;
            spell_speed = (GameState.rng.Next() % 3) * 0.1f;
            spell_scale = (GameState.rng.Next() % 3) * 0.1f;
            spell_cooldown = (GameState.rng.Next() % 4) * 0.1f;
        }
    }

    public class WizardStaff : Weapon
    {
        public WizardStaff()
        {
            should_show = true;
            weight = 25;
            prefab_index = 1;
            sprite_index = 54;
            name = "Wizard Staff";
            description = "A wizard's staff";
            spell_damage = GameState.rng.Next() % 5;
            spell_damage_ice = GameState.rng.Next() % 5;
            spell_damage_fire = GameState.rng.Next() % 5;
            spell_speed = (GameState.rng.Next() % 5) * 0.1f;
            spell_scale = (GameState.rng.Next() % 5) * 0.1f;
            spell_cooldown = (GameState.rng.Next() % 6) * 0.1f;
        }
    }

    public class AssassinDagger : Weapon
    {
        public AssassinDagger()
        {
            should_show = true;
            weight = 100;
            prefab_index = 1;
            sprite_index = 55;
            name = "Assassin Dagger";
            description = "Sharp, deadly";
            attack_damage = GameState.rng.Next() % 1;
            attack_damage_ice = GameState.rng.Next() % 2;
            attack_damage_fire = GameState.rng.Next() % 2;
            attack_speed = (GameState.rng.Next() % 12) * 0.1f;
        }
    }

    public class BronzeDagger : Weapon
    {
        public BronzeDagger()
        {
            should_show = true;
            weight = 100;
            prefab_index = 1;
            sprite_index = 56;
            name = "Bronze Dagger";
            description = "Mysterious dagger";
            attack_damage = GameState.rng.Next() % 1;
            attack_damage_ice = GameState.rng.Next() % 1;
            attack_damage_fire = GameState.rng.Next() % 1;
            attack_speed = (GameState.rng.Next() % 8) * 0.1f;
        }
    }

    public class Mace : Weapon
    {
        public Mace()
        {
            should_show = true;
            weight = 75;
            prefab_index = 1;
            sprite_index = 57;
            name = "Mace";
            description = "A simple but heavy mace";
            attack_damage = GameState.rng.Next() % 14;
            attack_speed = (GameState.rng.Next() % 3) * -0.1f;
            attack_scale = (GameState.rng.Next() % 14) * 0.1f;
        }
    }

}

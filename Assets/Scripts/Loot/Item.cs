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
using Attacks;
using Spells;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Loot
{

    public abstract class Item
    {
        public int prefab_index;
        public int sprite_index;

        public bool should_show;
        public string name;
        public string description;
        public int weight;
        public bool is_equippable;
        public bool is_consumable;
        public bool is_consumed_on_pickup;
        public float duration;
        public int amount;

        public abstract float Consume(PlayerStats ps);
        public abstract string EffectString();
        public abstract void Equip(PlayerStats ps);
        public abstract void UnEquip(PlayerStats ps);

        public void Drop(Vector3 position)
        {
            GameObject go = GameState.InstantiateGlobal(GameData.item_prefabs[prefab_index], position, Quaternion.identity);
            Debug.Log("Drop " + name + " with prefab: " + prefab_index);
            DropScript ds = go.GetComponent<DropScript>();
            ds.item = this;
        }
    }

    public class Gold : Item
    {
        public Gold()
        {
            prefab_index = 0;
            sprite_index = 0;

            weight = 1000;
            is_equippable = false;
            is_consumable = true;
            is_consumed_on_pickup = true;
            duration = 0;
            amount = (GameState.rng.Next(GameState.level * 3)) + 1;
        }

        public override float Consume(PlayerStats ps) { ps.gold += amount; amount = 0; return 0; }
        public override string EffectString() { return string.Format("+{0} gold", amount); }
        public override void Equip(PlayerStats ps) { return; }
        public override void UnEquip(PlayerStats ps) { return; }
    }

    public class Helmet : Item
    {
        public override float Consume(PlayerStats ps) { return 0; }
        public override string EffectString() { return string.Format("+ boots"); }
        public override void Equip(PlayerStats ps) { return; }
        public override void UnEquip(PlayerStats ps) { return; }
    }

    public class Jewelry : Item
    {
        public override float Consume(PlayerStats ps) { return 0; }
        public override string EffectString() { return string.Format("+ boots"); }
        public override void Equip(PlayerStats ps) { return; }
        public override void UnEquip(PlayerStats ps) { return; }
    }

    public class Armor : Item
    {
        public override float Consume(PlayerStats ps) { return 0; }
        public override string EffectString() { return string.Format("+ boots"); }
        public override void Equip(PlayerStats ps) { return; }
        public override void UnEquip(PlayerStats ps) { return; }
    }

    public class Weapon : Item
    {
        public int attack_damage;
        public int attack_damage_normal;
        public int attack_damage_ice;
        public int attack_damage_fire;
        public float attack_speed;
        public float attack_scale;

        public int spell_damage;
        public int spell_damage_normal;
        public int spell_damage_ice;
        public int spell_damage_fire;
        public float spell_speed;
        public float spell_scale;

        public override string EffectString()
        {
            return name + Environment.NewLine + description + Environment.NewLine
                + string.Format("+{0}/{1}/{2}/{3} attack damage{4}+{5}% attack speed{6}+{7}%AoE{8}+{9}/{10}/{11}/{12} spell damage{13}+{14}% cast speed{15}+{16}%AoE",
                        attack_damage, attack_damage_normal, attack_damage_ice, attack_damage_fire,
                        Environment.NewLine, attack_speed, Environment.NewLine, attack_scale, Environment.NewLine,
                        spell_damage, spell_damage_normal, spell_damage_ice, spell_damage_fire,
                        Environment.NewLine, spell_speed, Environment.NewLine, spell_scale);
        }

        public override void Equip(PlayerStats ps)
        {
            ps.attack_damage += attack_damage;
            ps.attack_damage_normal += attack_damage_normal;
            ps.attack_damage_ice += attack_damage_ice;
            ps.attack_damage_fire += attack_damage_fire;
            ps.attack_speed += attack_speed;
            ps.attack_scale += attack_scale;
            ps.spell_damage += spell_damage;
            ps.spell_damage_normal += spell_damage_normal;
            ps.spell_damage_ice += spell_damage_ice;
            ps.spell_damage_fire += spell_damage_fire;
            ps.spell_speed += spell_speed;
            ps.spell_scale += spell_scale;
            ps.active_attack.ScaleWithPlayerStats(ps);
            ps.active_spell.ScaleWithPlayerStats(ps);
        }

        public override void UnEquip(PlayerStats ps)
        {
            ps.attack_damage -= attack_damage;
            ps.attack_damage_normal -= attack_damage_normal;
            ps.attack_damage_ice -= attack_damage_ice;
            ps.attack_damage_fire -= attack_damage_fire;
            ps.attack_speed -= attack_speed;
            ps.attack_scale -= attack_scale;
            ps.spell_damage -= spell_damage;
            ps.spell_damage_normal -= spell_damage_normal;
            ps.spell_damage_ice -= spell_damage_ice;
            ps.spell_damage_fire -= spell_damage_fire;
            ps.spell_speed -= spell_speed;
            ps.spell_scale -= spell_scale;
            ps.active_attack.ScaleWithPlayerStats(ps);
            ps.active_spell.ScaleWithPlayerStats(ps);
        }

        public override float Consume(PlayerStats ps) { return 0; }
    }

    public class Boots : Item
    {
        public override float Consume(PlayerStats ps) { return 0; }
        public override string EffectString() { return string.Format("+ boots"); }
        public override void Equip(PlayerStats ps) { return; }
        public override void UnEquip(PlayerStats ps) { return; }
    }

}

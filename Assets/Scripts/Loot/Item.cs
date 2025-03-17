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
        public Color text_color = Color.white;

        public abstract float Consume(PlayerStats ps);
        public abstract string EffectString();
        public abstract void Equip(PlayerStats ps);
        public abstract void UnEquip(PlayerStats ps);

        public void Drop(Vector3 position)
        {
            GameObject go = GameState.InstantiateGlobal(GameData.item_prefabs[prefab_index], position, Quaternion.identity);
            // Debug.Log("Drop " + name + " with prefab: " + prefab_index);
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

            weight = 9000;
            is_equippable = false;
            is_consumable = true;
            is_consumed_on_pickup = true;
            duration = 0;
            amount = (GameState.rng.Next(GameState.level * 3)) + 1;
            name = amount.ToString() + " gold";
            text_color = new Color(1, 0.8f, 0, 0.75f);
        }

        public override float Consume(PlayerStats ps) { ps.gold += amount; amount = 0; return 0; }
        public override string EffectString() { return string.Format("+{0} gold", amount); }
        public override void Equip(PlayerStats ps) { return; }
        public override void UnEquip(PlayerStats ps) { return; }
    }

    public class HealthPotion : Item
    {
        public int hp;

        public HealthPotion()
        {
            prefab_index = 1;
            sprite_index = 25;

            weight = 1000;
            should_show = true;
            is_equippable = false;
            is_consumable = true;
            is_consumed_on_pickup = false;
            duration = 0;
            amount = 1;
            hp = 10;
            name = "Health Potion";
            description = "Restores 10 health";
            text_color = Color.red;
        }

        public override float Consume(PlayerStats ps) { ps.hp = Math.Min(ps.hp_max, ps.hp + hp); amount -= 1; return hp; }
        public override string EffectString() { return string.Format("Restores +{0} health (x{1})", hp, amount); }
        public override void Equip(PlayerStats ps) { return; }
        public override void UnEquip(PlayerStats ps) { return; }
    }

    public class Helmet : Item
    {
        public int hp;
        public int defense;
        public float hit_recovery;

        public override float Consume(PlayerStats ps) { return 0; }

        public override string EffectString()
        {
            string str = name + Environment.NewLine + description + Environment.NewLine + Environment.NewLine;

            str += (hp != 0) ?  string.Format("+{0} to max health", hp) + Environment.NewLine  : "";
            str += (defense != 0) ?  string.Format("+{0} to defense", defense) + Environment.NewLine  : "";
            str += (hit_recovery != 0) ?  string.Format("{0: 0.00}s to hit recovery", hit_recovery) + Environment.NewLine  : "";

            return str;
        }

        public override void Equip(PlayerStats ps)
        {
            ps.hp_max += hp;
            ps.hp += hp;
            ps.defense += defense;
            ps.stun_lock -= hit_recovery;
        }

        public override void UnEquip(PlayerStats ps)
        {
            ps.hp_max -= hp;
            ps.hp -= hp;
            ps.defense -= defense;
            ps.stun_lock += hit_recovery;
        }
    }

    public class Jewelry : Item
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

        public int hp;
        public float move_speed_bonus;

        public override string EffectString()
        {
            string str = name + Environment.NewLine + description + Environment.NewLine + Environment.NewLine;

            str += (hp != 0) ? string.Format("+{0} to health", hp) + Environment.NewLine : "";
            str += (move_speed_bonus != 0) ?  string.Format("+{0}% to movement speed", (int)(100 * move_speed_bonus)) + Environment.NewLine  : "";

            str += (attack_damage != 0) ?  string.Format("+{0} to attack damage", attack_damage) + Environment.NewLine  : "";
            str += (attack_damage_ice != 0) ?  string.Format("+{0} to attack ice damage", attack_damage_ice) + Environment.NewLine  : "";
            str += (attack_damage_fire != 0) ?  string.Format("+{0} to attack fire damage", attack_damage_fire) + Environment.NewLine  : "";
            str += (attack_speed != 0) ?  string.Format("+{0}% to attack speed", (int)(100 * attack_speed)) + Environment.NewLine  : "";
            str += (attack_scale != 0) ?  string.Format("+{0}% to attack AoE", (int)(100 * attack_scale)) + Environment.NewLine  : "";

            str += (spell_damage != 0) ?  string.Format("+{0} to spell damage", spell_damage) + Environment.NewLine  : "";
            str += (spell_damage_ice != 0) ?  string.Format("+{0} to spell ice damage", spell_damage_ice) + Environment.NewLine  : "";
            str += (spell_damage_fire != 0) ?  string.Format("+{0} to spell fire damage", spell_damage_fire) + Environment.NewLine  : "";
            str += (spell_speed != 0) ?  string.Format("+{0}% to cast speed", (int)(100 * spell_speed)) + Environment.NewLine  : "";
            str += (spell_scale != 0) ?  string.Format("+{0}% to spell AoE", (int)(100 * spell_scale)) : "";

            return str;
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
            ps.move_speed_bonus += move_speed_bonus;
            ps.hp_max += hp;
            ps.hp += hp;
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
            ps.move_speed_bonus -= move_speed_bonus;
            ps.hp_max -= hp;
            ps.hp -= hp;
        }

        public override float Consume(PlayerStats ps) { return 0; }
    }

    public class Armor : Item
    {
        public int hp;
        public int defense;
        public float hit_recovery;

        public override float Consume(PlayerStats ps) { return 0; }

        public override string EffectString()
        {
            string str = name + Environment.NewLine + description + Environment.NewLine + Environment.NewLine;

            str += (hp != 0) ?  string.Format("+{0} to max health", hp) + Environment.NewLine  : "";
            str += (defense != 0) ?  string.Format("+{0} to defense", defense) + Environment.NewLine  : "";
            str += (hit_recovery != 0) ?  string.Format("{0: 0.00}s to hit recovery", hit_recovery) + Environment.NewLine  : "";

            return str;
        }

        public override void Equip(PlayerStats ps)
        {
            ps.hp_max += hp;
            ps.hp += hp;
            ps.defense += defense;
            ps.stun_lock -= hit_recovery;
        }

        public override void UnEquip(PlayerStats ps)
        {
            ps.hp_max -= hp;
            ps.hp -= hp;
            ps.defense -= defense;
            ps.stun_lock += hit_recovery;
        }
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
        public float spell_cooldown;

        public override string EffectString()
        {
            string str = name + Environment.NewLine + description + Environment.NewLine + Environment.NewLine;

            str += (attack_damage > 0) ?  string.Format("+{0} to attack damage", attack_damage) + Environment.NewLine  : "";
            str += (attack_damage_ice > 0) ?  string.Format("+{0} to attack ice damage", attack_damage_ice) + Environment.NewLine  : "";
            str += (attack_damage_fire > 0) ?  string.Format("+{0} to attack fire damage", attack_damage_fire) + Environment.NewLine  : "";
            str += (attack_speed != 0) ?  string.Format("+{0}% to attack speed", (int)(100 * attack_speed)) + Environment.NewLine  : "";
            str += (attack_scale != 0) ?  string.Format("+{0}% to attack AoE", (int)(100 * attack_scale)) + Environment.NewLine  : "";

            str += (spell_damage > 0) ?  string.Format("+{0} to spell damage", spell_damage) + Environment.NewLine  : "";
            str += (spell_damage_ice > 0) ?  string.Format("+{0} to spell ice damage", spell_damage_ice) + Environment.NewLine  : "";
            str += (spell_damage_fire > 0) ?  string.Format("+{0} to spell fire damage", spell_damage_fire) + Environment.NewLine  : "";
            str += (spell_speed != 0) ?  string.Format("+{0}% to cast speed", (int)(100 * spell_speed)) + Environment.NewLine : "";
            str += (spell_scale != 0) ?  string.Format("+{0}% to spell AoE", (int)(100 * spell_scale)) + Environment.NewLine : "";
            str += (spell_cooldown != 0) ?  string.Format("-{0}% to spell cooldown", (int)(100 * spell_cooldown)) : "";

            return str;
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
            ps.spell_cooldown += spell_cooldown;
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
            ps.spell_cooldown -= spell_cooldown;
            ps.active_attack.ScaleWithPlayerStats(ps);
            ps.active_spell.ScaleWithPlayerStats(ps);
        }

        public override float Consume(PlayerStats ps) { return 0; }
    }

    public class Boots : Item
    {
        public int hp;
        public float move_speed_bonus;

        public override float Consume(PlayerStats ps) { return 0; }

        public override string EffectString()
        {
            string str = name + Environment.NewLine + description + Environment.NewLine + Environment.NewLine;

            str += (hp != 0) ?  string.Format("+{0} to max health", hp) + Environment.NewLine  : "";
            str += (move_speed_bonus != 0) ?  string.Format("+{0}% to movement speed", (int)(move_speed_bonus * 100)) + Environment.NewLine  : "";

            return str;
        }

        public override void Equip(PlayerStats ps)
        {
            ps.hp_max += hp;
            ps.hp += hp;
            ps.move_speed_bonus += move_speed_bonus;
        }

        public override void UnEquip(PlayerStats ps)
        {
            ps.hp_max -= hp;
            ps.hp -= hp;
            ps.move_speed_bonus -= move_speed_bonus;
        }
    }

}

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Attacks;
using Spells;
using Loot;

public class PlayerStats : MonoBehaviour
{
    public int level = 1;
    public int xp = 0;
    public int xp_max = 1;
    public int gold = 0;

    public int hp = 100;
    public int hp_max = 100;
    public bool invulnerable;
    public float stun_lock = 0.15f;
    public float move_speed_bonus = 0;

    public List<Item> items = new List<Item>();

    public int skill_points = 0;
    public List<Attack> learned_attacks = new List<Attack>();
    public List<Spell> learned_spells = new List<Spell>();
    public Attack active_attack;
    public Spell active_spell;

    public int attack_damage;
    public int attack_damage_normal;
    public int attack_damage_ice;
    public int attack_damage_fire;
    public float attack_speed;
    public float attack_scale;
    public float attack_duration;
    public float attack_cooldown;

    public int spell_damage;
    public int spell_damage_normal;
    public int spell_damage_ice;
    public int spell_damage_fire;
    public float spell_speed;
    public float spell_scale;
    public float spell_duration;
    public float spell_cooldown;

    public Helmet helmet;
    public Jewelry jewelry;
    public Armor armor;
    public Weapon weapon;
    public Boots boots;

    public int inventory_size = 25;
    public int inventory_space = 25;
    public Item[] inventory;
    public Item currently_held_item;

    public override string ToString()
    {
        return string.Format("Level: {0}{1}XP: {2}/{3}{4}HP: {5}/{6}{7}Skill Points: {8}{9}Attack Damage: {10}/{11}/{12}/{13}{14}Attack Speed: {15}%{16}Attack AoE: {17}%{18}Spell Damage: {19}/{20}/{21}/{22}{23}Cast Speed: {24}%{25}Spell AoE: {26}%{27}",
                level, Environment.NewLine,
                xp, xp_max, Environment.NewLine,
                hp, hp_max, Environment.NewLine,
                skill_points, Environment.NewLine,
                attack_damage, attack_damage_normal, attack_damage_ice, attack_damage_fire, Environment.NewLine,
                attack_speed, Environment.NewLine,
                attack_scale, Environment.NewLine,
                spell_damage, spell_damage_normal, spell_damage_ice, spell_damage_fire, Environment.NewLine,
                spell_speed, Environment.NewLine,
                spell_scale, Environment.NewLine);
    }

    public bool AddXp(int xp)
    {
        bool did_level = false;

        this.xp += xp;
        while (this.xp >= xp_max) {
            this.xp -= xp_max;
            hp_max += 1;
            skill_points += 1;
            hp = Math.Min(hp + 5, hp_max);
            did_level = true;
            level++;
            xp_max = level * level;
        }

        return did_level;
    }

    public void CopyFrom(PlayerStats ps)
    {
        this.level = ps.level;
        this.xp = ps.xp;
        this.xp_max = ps.xp_max;

        this.hp = ps.hp;
        this.hp_max = ps.hp_max;
        this.invulnerable = ps.invulnerable;
        this.stun_lock = ps.stun_lock;
        this.move_speed_bonus = ps.move_speed_bonus;

        this.skill_points = ps.skill_points;
        this.active_attack = ps.active_attack;
        this.active_spell = ps.active_spell;

        this.items = new List<Item>();
        this.learned_attacks = new List<Attack>();
        this.learned_spells = new List<Spell>();

        for (int i = 0; i < ps.items.Count; i++)
            this.items.Add(ps.items[i]);
        for (int i = 0; i < ps.learned_attacks.Count; i++)
            this.learned_attacks.Add(ps.learned_attacks[i]);
        for (int i = 0; i < ps.learned_spells.Count; i++)
            this.learned_spells.Add(ps.learned_spells[i]);

        this.attack_damage = ps.attack_damage;
        this.attack_damage_normal = ps.attack_damage_normal;
        this.attack_damage_ice = ps.attack_damage_ice;
        this.attack_damage_fire = ps.attack_damage_fire;
        this.attack_speed = ps.attack_speed;
        this.attack_scale = ps.attack_scale;
        this.attack_duration = ps.attack_duration;
        this.attack_cooldown = ps.attack_cooldown;

        this.spell_damage = ps.spell_damage;
        this.spell_damage_normal = ps.spell_damage_normal;
        this.spell_damage_ice = ps.spell_damage_ice;
        this.spell_damage_fire = ps.spell_damage_fire;
        this.spell_speed = ps.spell_speed;
        this.spell_scale = ps.spell_scale;
        this.spell_duration = ps.spell_duration;
        this.spell_cooldown = ps.spell_cooldown;

        this.helmet = ps.helmet;
        this.armor = ps.armor;
        this.jewelry = ps.jewelry;
        this.weapon = ps.weapon;
        this.boots = ps.boots;

        this.currently_held_item = ps.currently_held_item;
        this.inventory_size = ps.inventory_size;
        this.inventory_space = ps.inventory_space;
        this.inventory = new Item[this.inventory_size];
        if (ps.inventory != null) {
            for (int i = 0; i < ps.inventory_size; i++) {
                this.inventory[i] = ps.inventory[i];
            }
        }
    }
}

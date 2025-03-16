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

using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System;
using UnityEngine;
using Attacks;
using Spells;
using Loot;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public List<GameObject> attack_list_prefabs;
    public List<GameObject> spell_list_prefabs;
    public List<GameObject> item_list_prefabs;

    public List<Sprite> _spell_sprites;
    public List<Sprite> _attack_sprites;
    public List<Sprite> _item_sprites;

    public static List<GameObject> attack_prefabs;
    public static List<GameObject> spell_prefabs;
    public static List<GameObject> item_prefabs;
    public static List<Attack> attack_list;
    public static List<Spell> spell_list;
    public static List<Item> item_list;

    public static List<Item> item_list_Boss;

    public static List<Sprite> spell_sprites;
    public static List<Sprite> attack_sprites;
    public static List<Sprite> item_sprites;

    public static Helmet placeholder_helmet;
    public static Jewelry placeholder_jewelry;
    public static Armor placeholder_armor;
    public static Weapon placeholder_weapon;
    public static Boots placeholder_boots;

    static int item_total_weights = 0;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        attack_prefabs = attack_list_prefabs;
        spell_prefabs = spell_list_prefabs;
        item_prefabs = item_list_prefabs;

        spell_sprites = _spell_sprites;
        attack_sprites = _attack_sprites;
        item_sprites = _item_sprites;

        attack_list = new List<Attack>();
        attack_list.Add(new SlashNormal());
        attack_list.Add(new SlashIce());
        attack_list.Add(new MeleeChargeAttack());

        spell_list = new List<Spell>();
        spell_list.Add(new BlizzardSpell());
        spell_list.Add(new ForceFieldSpell());
        spell_list.Add(new CombustSpell());

        item_list = new List<Item>();

        item_list.Add(new Gold());
        item_list.Add(new Staff());
        item_list.Add(new Axe());
        item_list.Add(new SteelAxe());
        item_list.Add(new AssassinDagger());
        item_list.Add(new BronzeDagger());
        item_list.Add(new MageStaff());
        item_list.Add(new WizardStaff());
        item_list.Add(new Mace());
        item_list.Add(new VikingAxe());
        item_list.Add(new InfantrySword());

        item_list.Add(new LeatherArmor());
        item_list.Add(new ScoutArmor());
        item_list.Add(new BanditArmor());
        item_list.Add(new GreyKnightArmor());
        item_list.Add(new ManticoreArmor());
        item_list.Add(new DarkMountainArmor());
        item_list.Add(new BattleGuardArmor());
        item_list.Add(new RoyalTunicArmor());

        item_list.Add(new LeatherHelmet());
        item_list.Add(new ScoutHelmet());
        item_list.Add(new HawkHelmet());
        item_list.Add(new KnightHelmet());
        item_list.Add(new HighKnightHelmet());
        item_list.Add(new ManticoreHelmet());
        item_list.Add(new BattleGuardHelmet());
        item_list.Add(new DarkMountainHelmet());

        item_list.Add(new LeatherBoots());
        item_list.Add(new BanditBoots());
        item_list.Add(new GreyKnightBoots());
        item_list.Add(new ManticoreBoots());
        item_list.Add(new BattleGuardBoots());
        item_list.Add(new DarkMountainBoots());
        item_list.Add(new ScoutBoots());

        item_list.Add(new Amulet());
        item_list.Add(new BronzeRing());
        item_list.Add(new SilverRing());
        item_list.Add(new GoldRing());

        item_list.Add(new HealthPotion());

        item_list_Boss = new List<Item>();
        item_list_Boss.Add(new DivineDemonStaff());


        foreach (Item i in item_list) {
            item_total_weights += i.weight;
        }

        placeholder_helmet = new Helmet();
        placeholder_helmet.prefab_index = 1;
        placeholder_helmet.sprite_index = 1;
        placeholder_jewelry = new Jewelry();
        placeholder_jewelry.prefab_index = 1;
        placeholder_jewelry.sprite_index = 2;
        placeholder_armor = new Armor();
        placeholder_armor.prefab_index = 1;
        placeholder_armor.sprite_index = 3;
        placeholder_weapon = new Weapon();
        placeholder_weapon.prefab_index = 1;
        placeholder_weapon.sprite_index = 4;
        placeholder_boots = new Boots();
        placeholder_boots.prefab_index = 1;
        placeholder_boots.sprite_index = 5;
    }

    public static Item GenerateLoot()
    {
        int r = GameState.rng.Next(item_total_weights);
        int w = 0;
        int i;
        for (i = -1; r >= w; ++i) {
            w += item_list[i + 1].weight;
        }

        Type t = item_list[i].GetType();
        // Debug.Log("GenerateLoot: " + t.FullName);
        return (Item)Activator.CreateInstance(t);
    }

public static Item GenerateBossLoot()
    {

        Type t = item_list_Boss[ GameState.rng.Next(item_list_Boss.Count)].GetType();
        // Debug.Log("GenerateLoot: " + t.FullName);
        return (Item)Activator.CreateInstance(t);
    }

}

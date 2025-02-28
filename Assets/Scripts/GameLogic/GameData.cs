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
using System;
using UnityEngine;
using Attacks;
using Spells;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public List<GameObject> attack_list_prefabs;
    public List<GameObject> spell_list_prefabs;
    public List<Sprite> _spell_sprites;
    public List<Sprite> _attack_sprites;

    public static List<GameObject> attack_prefabs;
    public static List<GameObject> spell_prefabs;
    public static List<Attack> attack_list;
    public static List<Spell> spell_list;
    public static List<Sprite> spell_sprites;
    public static List<Sprite> attack_sprites;

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

        spell_sprites = _spell_sprites;
        attack_sprites = _attack_sprites;

        attack_list = new List<Attack>();
        attack_list.Add(new SlashAttack());
        attack_list.Add(new IceSlashAttack());
        attack_list.Add(new ShockWaveAttack());

        spell_list = new List<Spell>();
        spell_list.Add(new BlizzardSpell());
        spell_list.Add(new ForceFieldSpell());
    }
}

public enum EntityType { None, Player, Enemy }
public enum EnemyType { Ghost }

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

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using Attacks;
using Spells;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public GameObject player_prefab;
    public Texture2D cursor_default_pre;
    public Texture2D cursor_attack_pre;
    private static GameObject pf;

    public static GameState Instance;
    public static Texture2D cursor_default;
    public static Texture2D cursor_attack;

    public static Transform player_trf;

    static PlayerStats player_stats_saved;

    public static string level_name;
    public static int level = 0;
    public static Vector2Int level_seed;
    public static int difficulty = 0;
    public static System.Random rng = new System.Random();

    public static bool has_died;
    public static bool first_load = true;

    public static List<Attack> attacks = new List<Attack>();
    public static List<Spell> spells = new List<Spell>();

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        cursor_attack = cursor_attack_pre;
        cursor_default = cursor_default_pre;

        Cursor.SetCursor(cursor_default, Vector2.zero, CursorMode.Auto);

        pf = player_prefab;
        player_stats_saved = GetComponent<PlayerStats>();

        spells.Add(new BlizzardSpell());
        spells.Add(new ForceFieldSpell());
        spells.Add(new CombustSpell());
        spells.Add(new MeteorSpell());
        attacks.Add(new SlashNormal());
        attacks.Add(new SlashIce());
        attacks.Add(new MeleeChargeAttack());
        attacks.Add(new MagicThrust());
    }

    void Start()
    {
    }

    public static void ChangePf(GameObject new_pf)
    {
        pf = new_pf;
        Reset();
    }


    public static void InstantiatePlayer()
    {
        GameObject p;
        PlayerStats ps;

        if (first_load) {
            first_load = false;
            p = Instantiate(pf, new Vector3(0, -100, 0), Quaternion.identity);
            ps = p.GetComponent<PlayerStats>();

            ps.learned_attacks.Add(attacks[0]);
            ps.learned_spells.Add(spells[0]);
            ps.active_attack = ps.learned_attacks[0];
            ps.active_spell = ps.learned_spells[0];
            ps.active_attack.level = 1;
            ps.active_spell.level = 1;

            player_stats_saved.CopyFrom(ps);
            Destroy(p);
        }

        if (GameObject.Find("Player") == null) {
            p = Instantiate(pf, new Vector3(0, -100, 0), Quaternion.identity);
            p.name = "Player";
        } else {
            p = GameObject.Find("Player");
        }

        ps = p.GetComponent<PlayerStats>();
        ps.CopyFrom(player_stats_saved);
    }

    public static void Reset()
    {
        level_name = "";
        level = 0;
        difficulty = 0;

        GameObject p = Instantiate(pf, new Vector3(0, -100, 0), Quaternion.identity);
        PlayerStats ps = p.GetComponent<PlayerStats>();

        spells = new List<Spell>();
        attacks = new List<Attack>();

        spells.Add(new BlizzardSpell());
        spells.Add(new ForceFieldSpell());
        spells.Add(new CombustSpell());
        spells.Add(new MeteorSpell());
        attacks.Add(new SlashNormal());
        attacks.Add(new SlashIce());
        attacks.Add(new MeleeChargeAttack());
        attacks.Add(new MagicThrust());

        ps.learned_attacks.Add(attacks[0]);
        ps.learned_spells.Add(spells[0]);
        ps.active_attack = ps.learned_attacks[0];
        ps.active_spell = ps.learned_spells[0];
        ps.active_attack.level = 1;
        ps.active_spell.level = 1;

        player_stats_saved.CopyFrom(ps);
        Destroy(p);
    }

    public static void SavePlayerStats()
    {
        GameObject player = GameObject.Find("Player");
        PlayerStats ps = player.GetComponent<PlayerStats>();
        player_stats_saved.CopyFrom(ps);
    }

    public static void NextLevelSeed()
    {
        level_seed = new Vector2Int(rng.Next(1000), rng.Next(1000));
    }

    public static GameObject InstantiateGlobal(GameObject g, Vector3 v, Quaternion q)
    {
        return Instantiate(g, v, q);
    }

    public static GameObject InstantiateParented(GameObject g, Vector3 v, Quaternion q, Transform p)
    {
        return Instantiate(g, v, q, p);
    }
}

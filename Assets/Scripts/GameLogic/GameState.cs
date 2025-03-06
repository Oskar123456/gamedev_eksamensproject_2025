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
    private static GameObject pf;

    public static GameState Instance;

    public static Transform player_trf;

    static PlayerStats player_stats_saved;

    public static string level_name;
    public static int level = 0;
    public static int difficulty = 0;
    public static System.Random rng = new System.Random();

    public static bool has_died;
    public static bool first_load = true;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        pf = player_prefab;
        player_stats_saved = GetComponent<PlayerStats>();
    }

    void Start()
    {
    }

    public static void InstantiatePlayer()
    {
        GameObject p;
        PlayerStats ps;

        if (first_load) {
            first_load = false;
            p = Instantiate(pf, new Vector3(0, -100, 0), Quaternion.identity);
            ps = p.GetComponent<PlayerStats>();

            ps.learned_spells.Add(new BlizzardSpell());
            ps.learned_spells.Add(new ForceFieldSpell());
            ps.learned_attacks.Add(new SlashAttack());
            ps.learned_attacks.Add(new IceSlashAttack());
            ps.active_attack = ps.learned_attacks[0];
            ps.active_spell = ps.learned_spells[0];

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

        ps.learned_spells.Add(new BlizzardSpell());
        ps.learned_spells.Add(new ForceFieldSpell());
        ps.learned_attacks.Add(new SlashAttack());
        ps.learned_attacks.Add(new IceSlashAttack());
        ps.active_attack = ps.learned_attacks[0];
        ps.active_spell = ps.learned_spells[0];

        player_stats_saved.CopyFrom(ps);
        Destroy(p);
    }

    public static void SavePlayerStats()
    {
        GameObject player = GameObject.Find("Player");
        PlayerStats ps = player.GetComponent<PlayerStats>();
        player_stats_saved.CopyFrom(ps);
    }

    public static GameObject InstantiateGlobal(GameObject g, Vector3 v, Quaternion q)
    {
        return Instantiate(g, v, q);
    }
}

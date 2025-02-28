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
        player_stats = GetComponent<PlayerStats>();
        Reset();
        Destroy(GameObject.Find("Player"));
    }

    void Start()
    {
    }

    public static void InstantiatePlayer()
    {
        if (GameObject.Find("Player") != null) {
            Destroy(GameObject.Find("Player"));
        }

        GameObject p = Instantiate(pf, new Vector3(0, -100, 0), Quaternion.identity);
        p.name = "Player";
        PlayerStats ps = p.GetComponent<PlayerStats>();
        ps.CopyFrom(player_stats_saved);
    }

    public static void Reset()
    {
        level_name = "";
        level = 0;
        difficulty = 0;

        if (GameObject.Find("Player") != null) {
            Destroy(GameObject.Find("Player"));
        }

        GameObject p = Instantiate(pf, new Vector3(0, -100, 0), Quaternion.identity);
        p.name = "Player";
        PlayerStats ps = p.GetComponent<PlayerStats>();
        player_stats_saved.CopyFrom(ps);
    }

    public static void SavePlayerStats()
    {
        GameObject player = GameObject.Find("Player");
        PlayerStats ps = player.GetComponent<PlayerStats>();
        player_stats_saved.CopyFrom(ps);
    }
}

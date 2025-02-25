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
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Instance;

    public static Transform player_trf;

    public static PlayerStats player_stats;
    public static string level_name;
    public static int level = 0;
    public static int difficulty = 0;

    public int player_base_attack_damage = 1;
    public float player_base_attack_speed = 0.4f;
    public float player_base_attack_scale = 1f;
    public int player_base_hp_max = 100;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        player_stats = new PlayerStats();
        player_stats.attack_stats.damage = player_base_attack_damage;
        player_stats.attack_stats.duration = player_base_attack_damage;
        player_stats.attack_stats.scale = player_base_attack_damage;
        player_stats.hp_max = player_base_hp_max;
        player_stats.Sync();
    }

    public static void Reset()
    {
        player_stats = new PlayerStats();

        level_name = "";
        level = 0;
        difficulty = 0;
    }
}

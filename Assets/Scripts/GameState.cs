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

    static PlayerStats player_stats;
    static AttackStats player_attack_stats;

    public static string level_name;
    public static int level = 0;
    public static int difficulty = 0;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        player_stats = GetComponent<PlayerStats>();
        player_attack_stats = GetComponent<AttackStats>();

        player_stats.level = 1;
        player_attack_stats.entity_type = EntityType.Player;

        SavePlayerStats();
    }

    void Start()
    {
        SavePlayerStats();
    }

    public static void Reset()
    {
        level_name = "";
        level = 0;
        difficulty = 0;
    }

    public static void SavePlayerStats()
    {
        GameObject p = GameObject.Find("Player");
        PlayerStats p_s = p.GetComponent<PlayerStats>();
        AttackStats p_as = p.GetComponent<AttackStats>();

        player_stats.level = p_s.level;
        player_stats.hp = p_s.hp;
        player_stats.hp_max = p_s.hp_max;
        player_stats.xp = p_s.xp;
        player_stats.xp_max = p_s.xp_max;

        player_attack_stats.damage = p_as.damage;
        player_attack_stats.speed = p_as.speed;
        player_attack_stats.scale = p_as.scale;
    }

    public static void SetPlayerStats()
    {
        GameObject p = GameObject.Find("Player");
        PlayerStats p_s = p.GetComponent<PlayerStats>();
        AttackStats p_as = p.GetComponent<AttackStats>();

        p_s.level = player_stats.level;
        p_s.hp = player_stats.hp;
        p_s.hp_max = player_stats.hp_max;
        p_s.xp = player_stats.xp;
        p_s.xp_max = player_stats.xp_max;

        p_as.damage = player_attack_stats.damage;
        p_as.speed = player_attack_stats.speed;
        p_as.scale = player_attack_stats.scale;
    }
}

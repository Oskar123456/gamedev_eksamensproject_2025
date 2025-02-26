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
    public static GameState Instance;

    public static Transform player_trf;

    static PlayerStats player_stats;
    static AttackerStats player_attack_stats;
    static CasterStats player_caster_stats;

    static PlayerStatsInternal player_stats_default;
    static AttackStatsInternal player_attack_stats_default;
    static CasterStatsInternal player_caster_stats_default;

    public static string level_name;
    public static int level = 0;
    public static int difficulty = 0;

    public static bool has_died;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        player_stats = GetComponent<PlayerStats>();
        player_attack_stats = GetComponent<AttackerStats>();
        player_caster_stats = GetComponent<CasterStats>();

        player_stats.level = 1;
        player_attack_stats.entity_type = EntityType.Player;

        SaveDefaultPlayerStats();
        SavePlayerStats();
    }

    void Start()
    {
    }

    public static void Reset()
    {
        level_name = "";
        level = 0;
        difficulty = 0;
        SetPlayerStatsToDefault();
    }

    public static void SavePlayerStats()
    {
        GameObject p = GameObject.Find("Player");
        PlayerStats p_s = p.GetComponent<PlayerStats>();
        AttackerStats p_as = p.GetComponent<AttackerStats>();
        CasterStats p_cs = p.GetComponent<CasterStats>();

        player_stats.level = p_s.level;
        player_stats.hp = p_s.hp;
        player_stats.hp_max = p_s.hp_max;
        player_stats.xp = p_s.xp;
        player_stats.xp_max = p_s.xp_max;

        player_attack_stats.damage = p_as.damage;
        player_attack_stats.speed = p_as.speed;
        player_attack_stats.scale = p_as.scale;

        player_caster_stats.damage = p_cs.damage;
        player_caster_stats.speed = p_cs.speed;
        player_caster_stats.scale = p_cs.scale;
    }

    public static void SetPlayerStats()
    {
        GameObject p = GameObject.Find("Player");
        PlayerStats p_s = p.GetComponent<PlayerStats>();
        AttackerStats p_as = p.GetComponent<AttackerStats>();
        CasterStats p_cs = p.GetComponent<CasterStats>();

        p_s.level = player_stats.level;
        p_s.hp = player_stats.hp;
        p_s.hp_max = player_stats.hp_max;
        p_s.xp = player_stats.xp;
        p_s.xp_max = player_stats.xp_max;

        p_as.damage = player_attack_stats.damage;
        p_as.speed = player_attack_stats.speed;
        p_as.scale = player_attack_stats.scale;

        p_cs.damage = player_caster_stats.damage;
        p_cs.speed = player_caster_stats.speed;
        p_cs.scale = player_caster_stats.scale;
    }

    public static void SetPlayerStatsToDefault()
    {
        GameObject p = GameObject.Find("Player");
        PlayerStats p_s = p.GetComponent<PlayerStats>();
        AttackerStats p_as = p.GetComponent<AttackerStats>();
        CasterStats p_cs = p.GetComponent<CasterStats>();

        p_s.level = player_stats_default.level;
        p_s.hp = player_stats_default.hp;
        p_s.hp_max = player_stats_default.hp_max;
        p_s.xp = player_stats_default.xp;
        p_s.xp_max = player_stats_default.xp_max;

        p_as.entity_type = EntityType.Player;
        p_as.damage = player_attack_stats_default.damage;
        p_as.speed = player_attack_stats_default.speed;
        p_as.scale = player_attack_stats_default.scale;

        player_stats.level = p_s.level;
        player_stats.hp = p_s.hp;
        player_stats.hp_max = p_s.hp_max;
        player_stats.xp = p_s.xp;
        player_stats.xp_max = p_s.xp_max;

        player_attack_stats.entity_type = EntityType.Player;
        player_attack_stats.damage = p_as.damage;
        player_attack_stats.speed = p_as.speed;
        player_attack_stats.scale = p_as.scale;

        player_caster_stats.damage = p_cs.damage;
        player_caster_stats.speed = p_cs.speed;
        player_caster_stats.scale = p_cs.scale;
    }

    public static void SaveDefaultPlayerStats()
    {
        player_stats_default = new PlayerStatsInternal();
        player_attack_stats_default = new AttackStatsInternal();
        player_caster_stats_default = new CasterStatsInternal();

        GameObject p = GameObject.Find("Player");
        PlayerStats p_s = p.GetComponent<PlayerStats>();
        AttackerStats p_as = p.GetComponent<AttackerStats>();
        CasterStats p_cs = p.GetComponent<CasterStats>();

        player_stats_default.level = p_s.level;
        player_stats_default.hp = p_s.hp;
        player_stats_default.hp_max = p_s.hp_max;
        player_stats_default.xp = p_s.xp;
        player_stats_default.xp_max = p_s.xp_max;

        player_attack_stats_default.entity_type = EntityType.Player;
        player_attack_stats_default.damage = p_as.damage;
        player_attack_stats_default.speed = p_as.speed;
        player_attack_stats_default.scale = p_as.scale;

        player_caster_stats_default.damage = p_cs.damage;
        player_caster_stats_default.speed = p_cs.speed;
        player_caster_stats_default.scale = p_cs.scale;
    }
}

class PlayerStatsInternal
{
    public int hp, hp_max;
    public int xp, xp_max;
    public int level;
    public bool invulnerable;
    public float stun_lock;
}

public class AttackStatsInternal
{
    public GameObject attacker;
    public string attacker_tag;
    public EntityType entity_type;
    public int damage;
    public float speed;
    public float scale;
}

public class CasterStatsInternal
{
    public GameObject caster;
    public string caster_tag;
    public EntityType entity_type;
    public int damage;
    public float speed;
    public float scale;
}

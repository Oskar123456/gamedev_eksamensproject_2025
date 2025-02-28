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
    public static bool first_load = true;

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

        pf = player_prefab;
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
        if (first_load) {
            SaveDefaultPlayerStats();
            SetPlayerStatsToDefault();
            first_load = false;
        } else {
            SetPlayerStats();
        }
    }

    public static void Reset()
    {
        level_name = "";
        level = 0;
        difficulty = 0;
        SetPlayerStatsToDefault();
        SavePlayerStats();
        for (int i = 0; i < player_stats_default.spell_levels.Count; i++) {
            Debug.Log(player_stats_default.spell_levels[i]);
        }
    }

    public static void SavePlayerStats()
    {
        GameObject p = GameObject.Find("Player");
        PlayerStats p_s = p.GetComponent<PlayerStats>();
        AttackerStats p_as = p.GetComponent<AttackerStats>();
        CasterStats p_cs = p.GetComponent<CasterStats>();

        player_stats.Set(p_s);
        player_attack_stats.Set(p_as);
        player_caster_stats.Set(p_cs);
    }

    public static void SetPlayerStats()
    {
        GameObject p = GameObject.Find("Player");
        PlayerStats p_s = p.GetComponent<PlayerStats>();
        AttackerStats p_as = p.GetComponent<AttackerStats>();
        CasterStats p_cs = p.GetComponent<CasterStats>();

        p_s.Set(player_stats);
        p_as.Set(player_attack_stats);
        p_cs.Set(player_caster_stats);
    }

    public static void SetPlayerStatsToDefault()
    {
        GameObject p = GameObject.Find("Player");
        PlayerStats p_s = p.GetComponent<PlayerStats>();
        AttackerStats p_as = p.GetComponent<AttackerStats>();
        CasterStats p_cs = p.GetComponent<CasterStats>();

        player_stats_default.SetOther(p_s);
        player_attack_stats_default.SetOther(p_as);
        player_caster_stats_default.SetOther(p_cs);

        player_stats_default.SetOther(player_stats);
        player_attack_stats_default.SetOther(player_attack_stats);
        player_caster_stats_default.SetOther(player_caster_stats);
    }

    static void SaveDefaultPlayerStats()
    {
        player_stats_default = new PlayerStatsInternal();
        player_stats_default.learned_attacks = new List<int>();
        player_stats_default.learned_spells = new List<int>();
        player_stats_default.spell_levels = new List<int>();
        player_attack_stats_default = new AttackStatsInternal();
        player_caster_stats_default = new CasterStatsInternal();

        GameObject p = GameObject.Find("Player");
        PlayerStats p_s = p.GetComponent<PlayerStats>();
        AttackerStats p_as = p.GetComponent<AttackerStats>();
        CasterStats p_cs = p.GetComponent<CasterStats>();

        player_stats_default.Set(p_s);
        player_attack_stats_default.Set(p_as);
        player_caster_stats_default.Set(p_cs);
    }
}

class PlayerStatsInternal
{
    public int hp, hp_max;
    public int xp, xp_max;
    public int level;
    public bool invulnerable;
    public float stun_lock;
    public int skill_points = 0;
    public List<int> learned_attacks;
    public List<int> learned_spells;
    public List<int> spell_levels;
    public int active_attack, active_spell;

    public void Set(PlayerStats ps)
    {
        this.level = ps.level;
        this.hp = ps.hp;
        this.hp_max = ps.hp_max;
        this.xp = ps.xp;
        this.xp_max = ps.xp_max;
        this.skill_points = ps.skill_points;
        this.active_attack = ps.active_attack;
        this.active_spell = ps.active_spell;

        this.learned_attacks = new List<int>();
        this.learned_spells = new List<int>();
        this.spell_levels = new List<int>();
        for (int i = 0; i < ps.learned_attacks.Count; i++) {
            this.learned_attacks.Add(ps.learned_attacks[i]);
        }
        for (int i = 0; i < ps.learned_spells.Count; i++) {
            this.learned_spells.Add(ps.learned_spells[i]);
        }
        for (int i = 0; i < ps.spell_levels.Count; i++) {
            this.spell_levels.Add(ps.spell_levels[i]);
        }
    }

    public void SetOther(PlayerStats ps)
    {
        ps.level = this.level;
        ps.hp = this.hp;
        ps.hp_max = this.hp_max;
        ps.xp = this.xp;
        ps.xp_max = this.xp_max;
        ps.skill_points = this.skill_points;
        ps.active_attack = this.active_attack;
        ps.active_spell = this.active_spell;

        ps.learned_attacks = new List<int>();
        ps.learned_spells = new List<int>();
        ps.spell_levels = new List<int>();
        for (int i = 0; i < this.learned_attacks.Count; i++) {
            ps.learned_attacks.Add(this.learned_attacks[i]);
        }
        for (int i = 0; i < this.learned_spells.Count; i++) {
            ps.learned_spells.Add(this.learned_spells[i]);
        }
        for (int i = 0; i < this.spell_levels.Count; i++) {
            ps.spell_levels.Add(this.spell_levels[i]);
        }
    }
}

public class AttackStatsInternal
{
    public GameObject attacker;
    public string attacker_tag;
    public EntityType entity_type;
    public int damage;
    public float speed;
    public float scale;

    public void Set(AttackerStats ats)
    {
        this.attacker = ats.attacker;
        this.attacker_tag = ats.attacker_tag;
        this.entity_type = ats.entity_type;
        this.damage = ats.damage;
        this.speed = ats.speed;
        this.scale = ats.scale;
    }

    public void SetOther(AttackerStats ats)
    {
        ats.attacker = this.attacker;
        ats.attacker_tag = this.attacker_tag;
        ats.entity_type = this.entity_type;
        ats.damage = this.damage;
        ats.speed = this.speed;
        ats.scale = this.scale;
    }
}

public class CasterStatsInternal
{
    public GameObject caster;
    public string caster_tag;
    public EntityType entity_type;
    public int damage;
    public float speed;
    public float scale;
    public int spell_level;

    public void Set(CasterStats cs)
    {
        this.caster = cs.caster;
        this.caster_tag = cs.caster_tag;
        this.entity_type = cs.entity_type;
        this.damage = cs.damage;
        this.speed = cs.speed;
        this.scale = cs.scale;
        this.spell_level = cs.spell_level;
    }

    public void SetOther(CasterStats cs)
    {
        cs.caster = this.caster;
        cs.caster_tag = this.caster_tag;
        cs.entity_type = this.entity_type;
        cs.damage = this.damage;
        cs.speed = this.speed;
        cs.scale = this.scale;
        cs.spell_level = this.spell_level;
    }
}

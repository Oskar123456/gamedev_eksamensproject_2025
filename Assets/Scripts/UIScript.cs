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

public class UIScript : MonoBehaviour
{
    GameObject player;
    Transform player_trf;
    GameObject screen_color;
    Image screen_color_img;

    Slider player_hp_bar;
    Slider player_xp_bar;

    TextMeshProUGUI player_info;
    TextMeshProUGUI debug_info;
    TextMeshProUGUI hp_info;
    TextMeshProUGUI xp_info;
    TextMeshProUGUI level_intro_text;

    PlayerStats player_stats;
    AttackStats player_attack_stats;

    public float fade_in = 1;
    float fade_in_left;

    float[] fps_hist;
    int fps_hist_i;

    void Start()
    {
        /* debug */
        fps_hist = new float[60];
        /* level intro */
        screen_color = GameObject.Find("ScreenPanel");
        screen_color_img = screen_color.GetComponent<Image>();
        screen_color_img.color = Color.black;
        level_intro_text = GameObject.Find("LevelIntroText").GetComponent<TextMeshProUGUI>();
        level_intro_text.text = GameState.level_name;
        /* game UI */
        player = GameObject.Find("Player");
        player_trf = player.GetComponent<Transform>();
        player_stats = player.GetComponent<PlayerStats>();
        player_attack_stats = player.GetComponent<AttackStats>();
        player_info = GameObject.Find("PlayerInfo").GetComponent<TextMeshProUGUI>();
        debug_info = GameObject.Find("DebugInfo").GetComponent<TextMeshProUGUI>();

        hp_info = GameObject.Find("PlayerHPText").GetComponent<TextMeshProUGUI>();
        xp_info = GameObject.Find("PlayerXPText").GetComponent<TextMeshProUGUI>();

        player_hp_bar = GameObject.Find("PlayerHPBar").GetComponent<Slider>();
        player_hp_bar.value = player_stats.hp / player_stats.hp_max;
        player_xp_bar = GameObject.Find("PlayerXPBar").GetComponent<Slider>();
        player_xp_bar.value = player_stats.xp / player_stats.xp_max;

        fade_in_left = fade_in;
    }

    void Update()
    {
        fps_hist[fps_hist_i % 60] = 1 / Time.deltaTime; float fps_avg = 0; fps_hist_i = (fps_hist_i + 1) % 60;
        for (int i = 0; i < 60; i++) { fps_avg += fps_hist[i]; }

        if (player_trf.hasChanged) {
            debug_info.text = string.Format("player_pos: {0} | fps: {1: .00}",
                    player_trf.position.ToString(), fps_avg / 60.0f);
        }

        if (player_trf.hasChanged) {
            player_info.text = string.Format("stats: damage: {0} | attack-speed/-scale: {1: .00}/{2: .00}",
                    player_attack_stats.damage, player_attack_stats.speed, player_attack_stats.scale);
        }

        hp_info.text = string.Format("{0}/{1} HP", player_stats.hp, player_stats.hp_max);
        xp_info.text = string.Format("{0}/{1} XP [Level {2}]", player_stats.xp, player_stats.xp_max, player_stats.level);
        player_hp_bar.value = (float)player_stats.hp / player_stats.hp_max;
        player_xp_bar.value = (float)player_stats.xp / player_stats.xp_max;

        if (fade_in > 0) {
            screen_color_img.color = new Color(0, 0, 0, 1 - ((1 - fade_in_left) / fade_in));
            fade_in_left -= Time.deltaTime;
            if (fade_in_left <= 0) {
                screen_color_img.color = new Color(1, 1, 1, 0);
                screen_color.SetActive(false);
            }
        }
    }
}

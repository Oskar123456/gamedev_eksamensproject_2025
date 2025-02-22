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
    PlayerStats player_stats;
    GameObject screen_color;
    Image screen_color_img;

    Slider player_hp_bar;

    TextMeshProUGUI player_info;
    TextMeshProUGUI debug_info;
    TextMeshProUGUI level_intro_text;

    public float fade_in = 1;
    float fade_in_left;

    void Start()
    {
        /* level intro */
        screen_color = GameObject.Find("ScreenPanel");
        screen_color_img = screen_color.GetComponent<Image>();
        screen_color_img.color = Color.black;
        level_intro_text = GameObject.Find("LevelIntroText").GetComponent<TextMeshProUGUI>();
        level_intro_text.text = GameState.level_name;
        /* game UI */
        player = GameObject.Find("Player");
        player_trf = player.GetComponent<Transform>();
        player_stats = GameState.player_stats;
        player_info = GameObject.Find("PlayerInfo").GetComponent<TextMeshProUGUI>();
        debug_info = GameObject.Find("DebugInfo").GetComponent<TextMeshProUGUI>();
        player_hp_bar = GameObject.Find("PlayerHPBar").GetComponent<Slider>();
        player_hp_bar.value = player_stats.hp / player_stats.hp_max;

        fade_in_left = fade_in;
    }

    void Update()
    {
        if (player_trf.hasChanged) {
            debug_info.text = string.Format("player_pos: {0} | fps: {1: .00}",
                    player_trf.position.ToString(), 1 / Time.deltaTime);
        }
        player_hp_bar.value = (float)player_stats.hp / player_stats.hp_max;

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

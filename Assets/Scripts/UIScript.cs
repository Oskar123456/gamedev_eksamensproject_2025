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

    Slider player_hp_bar;

    TextMeshProUGUI player_info;
    TextMeshProUGUI debug_info;

    void Start()
    {
        player = GameObject.Find("Player");
        player_trf = player.GetComponent<Transform>();
        player_stats = player.GetComponent<PlayerStats>();
        player_info = GameObject.Find("PlayerInfo").GetComponent<TextMeshProUGUI>();
        debug_info = GameObject.Find("DebugInfo").GetComponent<TextMeshProUGUI>();
        player_hp_bar = GameObject.Find("PlayerHPBar").GetComponent<Slider>();
        player_hp_bar.value = player_stats.hp / player_stats.hp_max;
    }

    void Update()
    {
        if (player_trf.hasChanged) {
            debug_info.text = string.Format("player_pos: {0} | fps: {1: .00}",
                    player_trf.position.ToString(), 1 / Time.deltaTime);
        }
        player_hp_bar.value = (float)player_stats.hp / player_stats.hp_max;
    }
}

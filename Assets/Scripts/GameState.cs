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

    public static PlayerStats player_stats;
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
        player_stats = new PlayerStats();
    }

    public static void Reset()
    {
        player_stats = new PlayerStats();
        level_name = "";
        level = 0;
        difficulty = 0;
    }
}

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

using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    static List<EnemyStats> enemy_stats_list;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

public enum EntityType { None, Player, Enemy }
public enum EnemyType { Ghost }
public enum Spell { ForceField, Blizzard }

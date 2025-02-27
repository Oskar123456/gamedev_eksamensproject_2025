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

    public List<GameObject> attack_list_prefabs;
    public List<GameObject> spell_list_prefabs;

    public static List<GameObject> attack_list;
    public static List<GameObject> spell_list;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        attack_list = attack_list_prefabs;
        spell_list = spell_list_prefabs;
    }
}

public enum EntityType { None, Player, Enemy }
public enum EnemyType { Ghost }

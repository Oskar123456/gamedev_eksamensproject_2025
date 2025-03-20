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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Player;

public class GameControllerBoss : MonoBehaviour
{
    public GameObject player_prefab;
    public GameObject level_object_container_prefab;
    public GameObject portal_entrance_prefab;
    public GameObject portal_exit_prefab;
    public GameObject finish_marker_prefab;
    public GameObject player_marker_prefab;

    public List<GameObject> enemy_prefabs;

    GameObject UI;
    GameObject active_attack_button;
    GameObject active_spell_button;
    GameObject minimap_img;
    Camera minimap_cam;
    Transform minimap_cam_trf;

    GameObject player;
    Transform player_trf;
    CharacterController player_char_ctrl;

    GameObject player_marker;
    Transform player_marker_trf;
    GameObject finish_marker;
    Transform finish_marker_trf;

    GameObject level_object_container;
    List<GameObject> enemies = new List<GameObject>();

    /* game state */
    bool minimap_maximized = false;
    Vector2 minimap_img_pos;

    bool no_player_found;

    void Awake()
    {
    }

    void Start()
    {
        GameState.InstantiatePlayer();


        Vector3 start_pos = new Vector3(0, 2, 0);
        player = GameObject.Find("Player");
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = start_pos;
        player.GetComponent<CharacterController>().enabled = true;


        player_trf = player.GetComponent<Transform>();
        player_char_ctrl = player.GetComponent<CharacterController>();
        GameState.player_trf = player_trf;

        if (player.GetComponent<PlayerScript>() != null) {
            PlayerScript ps = player.GetComponent<PlayerScript>();
            ps.camera_dist = 1.0f;
        } else {
            PlayerScriptWarrior ps = player.GetComponent<PlayerScriptWarrior>();
            ps.camera_dist = 1.0f;
        }

        GameState.level++;
        GameState.level_name = "Level " + GameState.level.ToString() + Environment.NewLine + "Boss";

        minimap_img = GameObject.Find("MiniMapImg");
        minimap_cam = GameObject.Find("MiniMapCamera").GetComponent<Camera>();
        minimap_cam_trf = GameObject.Find("MiniMapCamera").GetComponent<Transform>();
        minimap_cam.orthographic = true;
        minimap_img_pos = minimap_img.GetComponent<RectTransform>().anchoredPosition;


        UI = GameObject.Find("UI");
        active_attack_button = GameObject.Find("ActiveAttackButton");
        active_spell_button = GameObject.Find("ActiveSpellButton");


    }

    void Update()
    {
        if (player == null || player_trf == null) {
            player = GameObject.Find("Player");
            player_trf = player.GetComponent<Transform>();
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            OnContinue();
        }

        if (Input.GetKeyDown(KeyCode.Tab)) {
            if (minimap_maximized) {
                minimap_img.GetComponent<RectTransform>().anchoredPosition = minimap_img_pos;
                minimap_img.GetComponent<Transform>().localScale = new Vector3(1, 1, 1);
            } else {
                minimap_img.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                minimap_img.GetComponent<Transform>().localScale = new Vector3(4, 4, 4);
            }
            minimap_maximized = !minimap_maximized;
        }

        if (player_trf.hasChanged) {
            // player_marker_trf.position = new Vector3(player_trf.position.x, 290, player_trf.position.z);
        }
    }

    void UpdateMiniMapCam()
    {
        minimap_cam_trf.position = new Vector3(100 / 2.0f, 400, 100 / 2.0f);
        minimap_cam_trf.eulerAngles = new Vector3(90, 0, 0);
        minimap_cam.orthographicSize = (float)Math.Max(100, 100) / 2.0f;
    }

    void Clean()
    {
        Destroy(level_object_container);
        level_object_container = Instantiate(level_object_container_prefab, Vector3.zero, Quaternion.identity, transform);
    }

    void OnPortal(Portal p)
    {
        if (p == Portal.Exit)
            OnContinue();
    }

    void OnDeath()
    {
        GameState.Reset();
        GameState.has_died = true;
        SceneManager.LoadScene("Menu");
    }

    void OnContinue()
    {
        GameState.SavePlayerStats();
        SceneManager.LoadScene("Town");
    }
}

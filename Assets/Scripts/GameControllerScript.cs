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

public class GameControllerScript : MonoBehaviour
{
    public GameObject level_object_container_prefab;
    public GameObject portal_entrance_prefab;
    public GameObject portal_exit_prefab;
    public GameObject finish_marker_prefab;
    public GameObject player_marker_prefab;

    public List<GameObject> enemy_prefabs;

    GameObject minimap_img;
    Camera minimap_cam;
    Transform minimap_cam_trf;

    PCGScript PCG;
    LevelBuilder level_builder;
    GameObject arena;

    GameObject player;
    Transform player_trf;
    CharacterController player_char_ctrl;
    PlayerStats player_stats;

    GameObject player_marker;
    Transform player_marker_trf;
    GameObject finish_marker;
    Transform finish_marker_trf;

    GameObject level_object_container;
    List<GameObject> enemies = new List<GameObject>();

    /* game state */
    bool minimap_maximized = false;
    Vector2 minimap_img_pos;

    LevelType current_level_type;
    Level current_level;

    void Awake()
    {
        if (GameState.player_stats == null)
            GameState.player_stats = new PlayerStats();
        player_stats = GameState.player_stats;

        // current_level_type = (Utils.rng.Next() % 2 == 0) ? LevelType.Medieval : LevelType.Water;
        current_level_type = LevelType.Medieval;
        GameState.level_num++;
        GameState.level_name = (current_level_type == LevelType.Medieval) ? "Dungeon (" + GameState.level_num.ToString() + ")"
            : "Cistern (" + GameState.level_num.ToString() + ")";
    }

    void Start()
    {
        arena = GameObject.Find("Arena");

        PCG = arena.GetComponent<PCGScript>();

        level_builder = arena.GetComponent<LevelBuilder>();
        level_builder.Init();

        minimap_img = GameObject.Find("MiniMapImg");
        minimap_cam = GameObject.Find("MiniMapCamera").GetComponent<Camera>();
        minimap_cam_trf = GameObject.Find("MiniMapCamera").GetComponent<Transform>();
        minimap_cam.orthographic = true;
        minimap_img_pos = minimap_img.GetComponent<RectTransform>().anchoredPosition;

        player = GameObject.Find("Player");
        player_trf = player.GetComponent<Transform>();
        player_char_ctrl = player.GetComponent<CharacterController>();

        player_marker = Instantiate(player_marker_prefab, Vector3.zero, Quaternion.Euler(90, 0, 0), player_trf);
        finish_marker = Instantiate(finish_marker_prefab, Vector3.zero, Quaternion.Euler(90, 0, 0), transform);
        player_marker_trf = player_marker.GetComponent<Transform>();
        finish_marker_trf = finish_marker.GetComponent<Transform>();

        StartNew();

        Debug.Log("start: " + current_level.GetStartPosition().ToString());
        Debug.Log("finish: " + current_level.GetFinishPosition().ToString());

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene("Town");
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

    void StartNew()
    {
        Clean();

        current_level = PCG.New(current_level_type);

        UpdateMiniMapCam();
        WarpPlayerToStart();

        Vector3 start_pos = current_level.GetStartPosition();
        Vector3 finish_pos = current_level.GetFinishPosition();

        Instantiate(portal_entrance_prefab, start_pos + Vector3.up * 0.35f, Quaternion.identity, level_object_container.transform);
        GameObject portal_exit = Instantiate(portal_exit_prefab, finish_pos + Vector3.up * 4, Quaternion.identity, level_object_container.transform);
        portal_exit.GetComponent<Transform>().localScale *= 4.0f;

        finish_marker_trf.position = new Vector3(finish_pos.x, 271, finish_pos.z);
        player_marker_trf.position = new Vector3(player_trf.position.x, 270, player_trf.position.z);
    }

    void UpdateMiniMapCam()
    {
        if (current_level == null)
            return;
        minimap_cam_trf.position = new Vector3(current_level.world_width / 2.0f, 400, current_level.world_height / 2.0f);
        minimap_cam_trf.eulerAngles = new Vector3(90, 0, 0);
        minimap_cam.orthographicSize = (float)Math.Max(current_level.world_width, current_level.world_height) / 2.0f;
    }

    void WarpPlayerToStart()
    {
        Vector3 start_pos = current_level.GetStartPosition();
        player_char_ctrl.enabled = false;
        player_trf.position = start_pos;
        player_char_ctrl.enabled = true;
    }

    void Clean()
    {
        Destroy(level_object_container);
        level_object_container = Instantiate(level_object_container_prefab, Vector3.zero, Quaternion.identity, transform);
    }

    void OnPortal(Portal p)
    {
        SceneManager.LoadScene("Town");
    }
}

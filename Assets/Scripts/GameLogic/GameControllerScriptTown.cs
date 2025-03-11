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
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControllerScriptMenu : MonoBehaviour
{
    public GameObject player_prefab;
    public GameObject portal_exit_prefab;
    public GameObject finish_marker_prefab;
    public GameObject player_marker_prefab;

    public Vector3 finish_pos;

    GameObject UI;
    GameObject minimap_img;
    Camera minimap_cam;
    Transform minimap_cam_trf;

    GameObject player;
    Transform player_trf;

    GameObject player_marker;
    Transform player_marker_trf;
    GameObject finish_marker;
    Transform finish_marker_trf;

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
        GameState.level_name = "Town";

        Vector3 start_pos = new Vector3(0, 2, 0);
        player = GameObject.Find("Player");
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = start_pos;
        player.GetComponent<CharacterController>().enabled = true;

        player_trf = player.GetComponent<Transform>();

        GameState.player_trf = player_trf;

        minimap_img = GameObject.Find("MiniMapImg");
        minimap_cam = GameObject.Find("MiniMapCamera").GetComponent<Camera>();
        minimap_cam_trf = GameObject.Find("MiniMapCamera").GetComponent<Transform>();
        minimap_cam.orthographic = true;
        minimap_img_pos = minimap_img.GetComponent<RectTransform>().anchoredPosition;

        UI = GameObject.Find("UI");

        player_marker = Instantiate(player_marker_prefab, Vector3.zero, Quaternion.Euler(90, 0, 0), transform);
        finish_marker = Instantiate(finish_marker_prefab, Vector3.zero, Quaternion.Euler(90, 0, 0), transform);
        player_marker_trf = player_marker.GetComponent<Transform>();
        finish_marker_trf = finish_marker.GetComponent<Transform>();

        GameObject portal_exit = Instantiate(portal_exit_prefab, finish_pos + Vector3.up, Quaternion.identity, transform);
        portal_exit.GetComponent<Transform>().localScale *= 4.0f;

        finish_marker_trf.position = new Vector3(finish_pos.x, 290, finish_pos.z);
        player_marker_trf.position = new Vector3(player_trf.position.x, 290, player_trf.position.z);

        UpdateMiniMapCam();
    }

    void Update()
    {
        if (player == null || player_trf == null) {
            player = GameObject.Find("Player");
            player_trf = player.GetComponent<Transform>();
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
            player_marker_trf.position = new Vector3(player_trf.position.x, 290, player_trf.position.z);
            UpdateMiniMapCam();
        }
    }

    void UpdateMiniMapCam()
    {
        minimap_cam_trf.position = new Vector3(0, 300, 0);
        minimap_cam_trf.eulerAngles = new Vector3(90, 0, 0);
        minimap_cam.orthographicSize = 100;
    }

    void WarpPlayerToStart()
    {
    }

    void Clean()
    {
    }

    void OnPortal(Portal p)
    {
        if (p == Portal.Exit) {
            GameState.SavePlayerStats();
            SceneManager.LoadScene("Arena");
        }
    }

    void OnDeath()
    {
        GameState.Reset();
        GameState.has_died = true;
        SceneManager.LoadScene("Menu");
    }
}

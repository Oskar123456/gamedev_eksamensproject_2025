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
using PCG;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalExitScript2 : MonoBehaviour
{
    GameObject game_controller;
    GameObject player;
    Transform player_trf;
    Vector3 original_scale;

    public float fade_in = 5;
    float fade_in_left;

    void Awake()
    {
        game_controller = GameObject.Find("GameController");
        original_scale = transform.localScale * 4;
        fade_in_left = fade_in;
    }

    void Update()
    {
        if (player == null || player_trf == null) {
            player = GameObject.Find("Player");
            player_trf = player.GetComponent<Transform>();
            if (player_trf != null) {
                DirectAtPlayer();
            } else {
                return;
            }
        }

        if (fade_in_left > 0) {
            float fade_in_frac = (fade_in - fade_in_left) / fade_in;
            transform.localScale = Vector3.Lerp(Vector3.zero, original_scale, fade_in_frac);
            fade_in_left -= Time.deltaTime;
            if (fade_in_left <= 0) {
                transform.localScale = original_scale;
            }
        }

        if (player_trf.hasChanged) {
            DirectAtPlayer();
        }
    }

    void DirectAtPlayer()
    {
        Vector3 this_p = transform.position;
        Vector3 player_p = player_trf.position;
        this_p.y = 0;
        player_p.y = 0;
        transform.rotation = Quaternion.FromToRotation(Vector3.forward, this_p - player_p);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player")) {
            game_controller.SendMessage("OnBossPortal", Portal2.Exit);
        }
    }
}

public enum Portal2 { Exit, Entrance }


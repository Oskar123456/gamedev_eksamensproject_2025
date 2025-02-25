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

public class UIMenuScript : MonoBehaviour
{
    GameObject screen_color;
    Image screen_color_img;

    Button play_button;
    // TextMeshProUGUI title;
    TextMeshProUGUI status;

    public float fade_in = 1;
    float fade_in_left;

    void Start()
    {
        /* debug */
        GameObject.Find("Player").SetActive(false);
        /* level intro */
        screen_color = GameObject.Find("ScreenPanel");
        screen_color_img = screen_color.GetComponent<Image>();
        screen_color_img.color = Color.black;

        play_button = GameObject.Find("PlayButton").GetComponent<Button>();
        play_button.onClick.AddListener(PlayGame);
        // title = GameObject.Find("Title").GetComponent<TextMeshProUGUI>();
        status = GameObject.Find("Status").GetComponent<TextMeshProUGUI>();
        /* game UI */
        if (GameState.has_died) {
            status.text = "YOU DIED...";
        } else {
            status.text = "Welcome...";
        }

        GameState.has_died = false;

        fade_in_left = fade_in;
    }

    void Update()
    {
        if (fade_in > 0) {
            screen_color_img.color = new Color(0, 0, 0, 1 - ((1 - fade_in_left) / fade_in));
            fade_in_left -= Time.deltaTime;
            if (fade_in_left <= 0) {
                screen_color_img.color = new Color(1, 1, 1, 0);
                screen_color.SetActive(false);
            }
        }
    }

    void PlayGame()
    {
        SceneManager.LoadScene("Town");
    }
}

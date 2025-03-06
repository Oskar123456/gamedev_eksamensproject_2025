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
using System.Collections.Generic;
using Attacks;
using Spells;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Loot
{

    public class DropScript : MonoBehaviour
    {
        public GameObject loot_button_prefab;

        public Item item;

        GameObject canvas;
        Button button;
        GameObject button_text;
        RectTransform canvas_rt;
        GameObject UI;
        RectTransform UI_rt;
        GameObject ui_loot_layer;

        GameObject player;
        Transform player_trf;
        Transform player_cam_trf;

        void Start()
        {
            ui_loot_layer = GameObject.Find("LootLayer");
            UI = GameObject.Find("UI");
            UI_rt = UI.GetComponent<RectTransform>();
            player = GameObject.Find("Player");
            player_trf = player.GetComponent<Transform>();
            player_cam_trf = GameObject.Find("Main Camera").GetComponent<Transform>();

            Vector3 screen_space_pos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.5f);
            canvas = Instantiate(loot_button_prefab, screen_space_pos, Quaternion.identity, ui_loot_layer.transform);
            canvas_rt = canvas.GetComponent<RectTransform>();
            button = canvas.GetComponent<Button>();
            button.onClick.AddListener(OnPickUp);
            button_text = canvas.transform.GetChild(0).gameObject;
            button_text.GetComponent<TextMeshProUGUI>().text = item.name;
            button_text.GetComponent<TextMeshProUGUI>().color = (item.text_color != null) ? item.text_color : Color.white;
        }

        void Update()
        {
            Vector3 screen_space_pos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 0.5f);
            screen_space_pos.x = (screen_space_pos.x / Screen.width) * UI_rt.sizeDelta.x - UI_rt.sizeDelta.x / 2.0f;
            screen_space_pos.y = (screen_space_pos.y / Screen.height) * UI_rt.sizeDelta.y - UI_rt.sizeDelta.y / 2.0f;
            canvas_rt.anchoredPosition = screen_space_pos;
        }

        void OnPickUp()
        {
            GetComponent<AudioSource>().Play();
            player.SendMessage("OnPickUp", item, SendMessageOptions.DontRequireReceiver);
            Destroy(canvas);
            Destroy(gameObject);
        }
    }

}

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
using Loot;
using UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ItemSlotScript : MonoBehaviour
{
    public Sprite placeholder_sprite;
    public GameObject tooltip_prefab;

    public Item item;
    public int inventory_index;

    GameObject child;
    GameObject tooltip;
    TextMeshProUGUI tooltip_text;
    BaseEventData base_event_data;
    Image image;
    Button button;
    UIScript ui_script;
    PlayerStats player_stats;

    void Awake()
    {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(SwapItem);
        ui_script = GameObject.Find("UI").GetComponent<UIScript>();

        foreach (Transform t in transform) {
            child = t.gameObject;
        }
    }

    void Start()
    {
        foreach (Transform t in transform) {
            child = t.gameObject;
        }
    }

    void Update()
    {
    }

    void LateUpdate()
    {
        if (item == null) {
            return;
        }

        if (ui_script.current_ui_object_hovered == gameObject || (child != null && ui_script.current_ui_object_hovered == child)) {
            ui_script.item_tooltip_text.text = item.EffectString();
            ui_script.item_tooltip.SetActive(true);
        }
    }

    void SwapItem()
    {
        if (player_stats == null) {
            GameObject player = GameObject.Find("Player");
            if (player == null) {
                return;
            }
            player_stats = player.GetComponent<PlayerStats>();
        }

        if (player_stats.currently_held_item != null || item != null) {
            ui_script.PlaySwapSound();
        }

        Item temp_item = item;
        item = player_stats.currently_held_item;
        player_stats.inventory[inventory_index] = item;
        player_stats.currently_held_item = temp_item;

        ui_script.Sync();
        ui_script.ShowInventory();
    }
}

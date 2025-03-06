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
using Player;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BootSlotScript : MonoBehaviour
{
    public Sprite placeholder_sprite;

    public Item item;

    List<GameObject> children;
    Image image;
    Button button;
    UIScript ui_script;
    GameObject player;
    PlayerStats player_stats;
    PlayerScript player_script;

    void Start()
    {
        image = GameObject.Find("BootIcon").GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(SwapItem);
        ui_script = GameObject.Find("UI").GetComponent<UIScript>();

        children = new List<GameObject>();
        foreach (Transform t in transform) {
            children.Add(t.gameObject);
        }
    }

    void Update()
    {
        if (item == null) {
            return;
        }

        if (ui_script.current_ui_object_hovered == gameObject || children.Contains(ui_script.current_ui_object_hovered)) {
            ui_script.item_tooltip_text.text = item.EffectString();
            ui_script.item_tooltip.SetActive(true);
        }
    }

    void SwapItem()
    {
        if (player_stats == null) {
            player = GameObject.Find("Player");
            if (player == null) {
                return;
            }
            player_stats = player.GetComponent<PlayerStats>();
            player_script = player.GetComponent<PlayerScript>();
        }

        if (player_stats.currently_held_item != null && !(player_stats.currently_held_item is Boots)) {
            return;
        }

        if (player_stats.boots != null) {
            player_stats.boots.UnEquip(player_stats);
        }

        Item temp_item = item;
        item = player_stats.currently_held_item;
        player_stats.boots = (Boots)item;
        player_stats.currently_held_item = temp_item;

        if (player_stats.boots != null) {
            player_stats.boots.Equip(player_stats);
        }

        player_script.SyncStats();
        ui_script.SetHeldItem();
        ui_script.Sync();

        image.sprite = (item != null) ? GameData.item_sprites[item.sprite_index] : placeholder_sprite;
    }
}


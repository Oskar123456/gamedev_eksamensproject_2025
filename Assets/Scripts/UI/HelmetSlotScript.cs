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

public class HelmetSlotScript : MonoBehaviour
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
    AudioSource audio_source;

    void Awake()
    {
        audio_source = GetComponent<AudioSource>();
        image = GameObject.Find("HelmetIcon").GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(SwapItem);
        ui_script = GameObject.Find("UI").GetComponent<UIScript>();

        children = new List<GameObject>();
        foreach (Transform t in transform) {
            children.Add(t.gameObject);
        }
    }

    void Start()
    {
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

        if (player_stats.currently_held_item != null && !(player_stats.currently_held_item is Helmet)) {
            ui_script.PlaySwapFailSound();
            return;
        }

        if (player_stats.helmet != null) {
            player_stats.helmet.UnEquip(player_stats);
        }

        Item temp_item = item;
        item = player_stats.currently_held_item;
        player_stats.helmet = (Helmet)item;
        player_stats.currently_held_item = temp_item;

        if (player_stats.helmet != null) {
            player_stats.helmet.Equip(player_stats);
        }

        player.SendMessage("SyncStats");
        ui_script.SetHeldItem();
        ui_script.Sync();

        audio_source.Play();
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        if (player_stats == null) {
            player = GameObject.Find("Player");
            if (player == null) {
                return;
            }
            player_stats = player.GetComponent<PlayerStats>();
            player_script = player.GetComponent<PlayerScript>();
        }
        item = player_stats.helmet;
        image.sprite = (item != null) ? GameData.item_sprites[item.sprite_index] : placeholder_sprite;
    }
}

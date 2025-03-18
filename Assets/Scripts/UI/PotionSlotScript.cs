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

public class PotionSlotScript : MonoBehaviour
{
    public Sprite placeholder_sprite;

    public HealthPotion potions;

    List<GameObject> children;
    Image image;
    RectTransform image_trf;
    TextMeshProUGUI image_text;
    Button button;
    UIScript ui_script;
    GameObject player;
    PlayerStats player_stats;
    PlayerScript player_script;
    AudioSource audio_source;
    CoolDownScript ui_active_potion_cooldown;

    bool first = true;
    float cooldown = 1;
    float cooldown_left;

    void Awake()
    {
        audio_source = GetComponent<AudioSource>();
        image = GameObject.Find("PotionIcon").GetComponent<Image>();
        image_trf = GameObject.Find("PotionIcon").GetComponent<RectTransform>();
        image_text = GameObject.Find("PotionAmountText").GetComponent<TextMeshProUGUI>();
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
        GameObject ui_active_potion_cooldown_go = GameObject.Find("PotionCoolDown");
        if (ui_active_potion_cooldown_go != null) {
            ui_active_potion_cooldown = ui_active_potion_cooldown_go.GetComponent<CoolDownScript>();
        }
    }

    void Update()
    {
        cooldown_left -= Time.deltaTime;

        if (first) {
            UpdateSprite();
            first = false;
        }

        if (player_stats == null || player_stats.potions == null) {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            Drink();
        }
    }

    void LateUpdate()
    {
        if (player_stats == null || player_stats.potions == null) {
            return;
        }

        if (ui_script.current_ui_object_hovered == gameObject) {
            ui_script.item_tooltip_text.text = player_stats.potions.EffectString();
            ui_script.item_tooltip.SetActive(true);
        }

        UpdateSprite();
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

        if (Input.GetMouseButtonDown(1)) {
            Drink();
            return;
        }

        if (player_stats.currently_held_item == null) {
            if (player_stats.potions != null && player_stats.potions.amount > 0) {
                player_stats.currently_held_item = new HealthPotion();
                player_stats.potions.amount -= 1;
                if (player_stats.potions.amount < 1) {
                    player_stats.potions = null;
                }
                ui_script.ShowInventory();
            }
        } else if (player_stats.currently_held_item is HealthPotion) {
            HealthPotion hp_pot = (HealthPotion)player_stats.currently_held_item;
            player_stats.currently_held_item = null;
            if (player_stats.potions == null) {
                player_stats.potions = hp_pot;
            } else {
                player_stats.potions.amount += hp_pot.amount;
            }
        } else {
            ui_script.PlaySwapFailSound();
            return;
        }

        ui_script.SetHeldItem();
        ui_script.Sync();

        audio_source.Play();
        UpdateSprite();
    }

    void Drink()
    {
        if (player_stats == null) {
            player = GameObject.Find("Player");
            if (player == null) {
                return;
            }
            player_stats = player.GetComponent<PlayerStats>();
            player_script = player.GetComponent<PlayerScript>();
        }

        if (player_stats.potions == null) {
            return;
        }

        if (cooldown_left > 0) {
            ui_script.PlaySwapFailSound();
            return;
        } else {
            cooldown_left = cooldown;
            ui_active_potion_cooldown.SetCoolDown(cooldown_left);
        }

        player_stats.potions.Consume(player_stats);
        if (player_stats.potions.amount < 1) {
            player_stats.potions = null;
        }
        ui_script.PlayBuffSound();
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
        image.sprite = (player_stats.potions != null) ? GameData.item_sprites[player_stats.potions.sprite_index] : placeholder_sprite;
        image_trf.sizeDelta = (player_stats.potions != null) ? new Vector2(100, 100) : new Vector2(50, 50);
        image_text.text = (player_stats.potions != null) ? player_stats.potions.amount.ToString() : "";
    }
}

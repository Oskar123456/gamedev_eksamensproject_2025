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

namespace Player
{

public class InventoryScript : MonoBehaviour
{
    public GameObject inventory_slot_prefab;
    public GameObject inventory_slot_image_prefab;
    public Sprite placeholder_sprite;

    Sprite helmet_sprite;
    Sprite armor_sprite;
    Sprite jewelry_sprite;
    Sprite weapon_sprite;
    Sprite boot_sprite;

    GameObject player;
    PlayerStats player_stats;

    GameObject equipment_slots;
    GameObject helmet_slot;
    GameObject armor_slot;
    GameObject jewelry_slot;
    GameObject weapon_slot;
    GameObject boot_slot;
    GameObject inventory;
    WeaponSlotScript weapon_slot_script;
    HelmetSlotScript helmet_slot_script;
    ArmorSlotScript armor_slot_script;
    BootSlotScript boot_slot_script;
    JewelrySlotScript jewelry_slot_script;

    void Awake()
    {
        equipment_slots = GameObject.Find("EquipmentSlots");
        inventory = GameObject.Find("InventorySlots");
        helmet_slot = GameObject.Find("HelmetIcon");
        armor_slot = GameObject.Find("ArmorIcon");
        jewelry_slot = GameObject.Find("JewelryIcon");
        weapon_slot = GameObject.Find("WeaponIcon");
        boot_slot = GameObject.Find("BootIcon");
        helmet_sprite = helmet_slot.GetComponent<Image>().sprite;
        armor_sprite = armor_slot.GetComponent<Image>().sprite;
        jewelry_sprite = jewelry_slot.GetComponent<Image>().sprite;
        weapon_sprite = weapon_slot.GetComponent<Image>().sprite;
        boot_sprite = boot_slot.GetComponent<Image>().sprite;
        weapon_slot_script = GameObject.Find("InventoryWeaponSlot").GetComponent<WeaponSlotScript>();
        helmet_slot_script = GameObject.Find("InventoryHelmetSlot").GetComponent<HelmetSlotScript>();
        jewelry_slot_script = GameObject.Find("InventoryJewelrySlot").GetComponent<JewelrySlotScript>();
        armor_slot_script = GameObject.Find("InventoryArmorSlot").GetComponent<ArmorSlotScript>();
        boot_slot_script = GameObject.Find("InventoryBootSlot").GetComponent<BootSlotScript>();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    public void Draw()
    {
        if (player == null || player_stats == null) {
            player = GameObject.Find("Player");
            player_stats = player.GetComponent<PlayerStats>();
        }

        foreach (Transform t in inventory.transform) {
            Destroy(t.gameObject);
        }

        weapon_slot_script.item = player_stats.weapon;
        boot_slot_script.item = player_stats.boots;
        armor_slot_script.item = player_stats.armor;
        helmet_slot_script.item = player_stats.helmet;
        jewelry_slot_script.item = player_stats.jewelry;
        weapon_slot_script.UpdateSprite();
        jewelry_slot_script.UpdateSprite();
        helmet_slot_script.UpdateSprite();
        armor_slot_script.UpdateSprite();
        boot_slot_script.UpdateSprite();

        RectTransform inventory_rect = inventory.GetComponent<RectTransform>();

        float outer_padding = 34;
        float y_padding = 10;
        float padding = 2;
        float i_slot_width = (inventory_rect.rect.size.x - 2 * outer_padding - 7 * padding) / 8;
        float i_slot_height = i_slot_width;

        for (int i = 0; i < 4; i++) {
            for (int j = 0; j < 8; j++) {
                GameObject i_slot = Instantiate(inventory_slot_prefab, Vector3.zero, Quaternion.identity, inventory.transform);
                RectTransform i_slot_rt = i_slot.GetComponent<RectTransform>();
                i_slot_rt.sizeDelta = new Vector2(i_slot_width, i_slot_height);
                i_slot_rt.anchoredPosition = new Vector2(j * i_slot_width + j * padding - inventory_rect.rect.size.x / 2 + i_slot_width / 2 + outer_padding,
                        -i * i_slot_height + -i * padding + inventory_rect.rect.size.y / 2 - i_slot_width / 2 - outer_padding - y_padding);

                ItemSlotScript iss = i_slot.GetComponent<ItemSlotScript>();
                iss.inventory_index = i * 8 + j;
                iss.item = player_stats.inventory[iss.inventory_index];

                // Debug.Log("inventory slot no." + iss.inventory_index + " : "
                //         + (player_stats.inventory[iss.inventory_index] == null ? "null" : player_stats.inventory[iss.inventory_index].name));

                if (player_stats.inventory[i * 8 + j] != null) {
                    GameObject iss_img = Instantiate(inventory_slot_image_prefab, Vector3.zero, Quaternion.identity, i_slot.transform);
                    RectTransform rt = iss_img.GetComponent<RectTransform>();
                    rt.sizeDelta = i_slot_rt.sizeDelta * 0.85f;
                    rt.anchoredPosition = Vector2.zero;
                    Image img = iss_img.GetComponent<Image>();
                    img.sprite = GameData.item_sprites[player_stats.inventory[i * 8 + j].sprite_index];
                }
            }
        }
    }
}

}

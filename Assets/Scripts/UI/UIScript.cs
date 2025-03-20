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

using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Spells;
using Attacks;
using Player;
using Misc;

namespace UI
{
    public class UIScript : MonoBehaviour
    {
        public GameObject skill_tree_element;
        public GameObject skill_tree_element_icon;
        public GameObject skill_tree_element_button;
        public GameObject skill_tree_element_description;

        AudioSource audio_source;
        public AudioClip[] audio_clips;
        GameObject player;
        GameObject player_cursor_img;
        RectTransform player_cursor_img_rt;
        public GameObject item_tooltip;
        RectTransform item_tooltip_rt;
        public TextMeshProUGUI item_tooltip_text;
        GameObject active_attack_button;
        GameObject active_spell_button;
        GameObject skill_tree_plus_button;
        Transform player_trf;
        GameObject screen_color;
        Image screen_color_img;
        UITest ui_test;

        Slider player_hp_bar;
        Slider player_xp_bar;

        TextMeshProUGUI player_info;
        TextMeshProUGUI debug_info;
        TextMeshProUGUI hp_info;
        TextMeshProUGUI xp_info;
        TextMeshProUGUI level_intro_text;

        TextMeshProUGUI stats_name_text;
        TextMeshProUGUI stats_misc_text;
        TextMeshProUGUI stats_attack_text;
        TextMeshProUGUI stats_spell_text;
        TextMeshProUGUI stats_defense_text;
        TextMeshProUGUI stats_util_text;

        TextMeshProUGUI skill_tree_free_text;

        GameObject skill_tree;
        GameObject inventory;
        GameObject stats;
        GameObject skill_tree_container;
        InventoryScript inventory_script;

        GameObject pause_overlay;

        List<GameObject> occluding_gameobjects = new List<GameObject>();

        public GameObject current_ui_object_hovered;
        public bool is_ui_object_hovered;

        PlayerStats player_stats;

        public float fade_in = 1;
        float fade_in_left;

        float[] fps_hist;
        int fps_hist_i;

        bool toggled;
        bool skill_tree_toggled;
        bool stats_toggled;
        bool inventory_toggled;
        bool is_inventory_active;

        void Start()
        {
            /* debug */
            fps_hist = new float[60];
            /* level intro */
            screen_color = GameObject.Find("ScreenPanel");
            screen_color_img = screen_color.GetComponent<Image>();
            screen_color_img.color = Color.black;
            level_intro_text = GameObject.Find("LevelIntroText").GetComponent<TextMeshProUGUI>();
            /* game UI */
            ui_test = GetComponent<UITest>();
            audio_source = GetComponent<AudioSource>();

            skill_tree = GameObject.Find("SkillTree");
            skill_tree_container = GameObject.Find("SkillTreeElementContainer");
            skill_tree_plus_button = GameObject.Find("SkillTreePlusButton");
            inventory = GameObject.Find("Inventory");
            inventory_script = inventory.GetComponent<InventoryScript>();
            stats = GameObject.Find("Stats");

            stats_name_text = GameObject.Find("StatsNameText").GetComponent<TextMeshProUGUI>();
            stats_misc_text = GameObject.Find("StatsMiscText").GetComponent<TextMeshProUGUI>();
            stats_attack_text = GameObject.Find("StatsAttackText").GetComponent<TextMeshProUGUI>();
            stats_spell_text = GameObject.Find("StatsSpellText").GetComponent<TextMeshProUGUI>();
            stats_defense_text = GameObject.Find("StatsDefenseText").GetComponent<TextMeshProUGUI>();
            stats_util_text = GameObject.Find("StatsUtilText").GetComponent<TextMeshProUGUI>();

            skill_tree_free_text = GameObject.Find("SkillTreePointsText").GetComponent<TextMeshProUGUI>();

            active_attack_button = GameObject.Find("ActiveAttackButton");
            active_spell_button = GameObject.Find("ActiveSpellButton");

            player_cursor_img = GameObject.Find("CurrentlyHeldItem");
            player_cursor_img_rt = player_cursor_img.GetComponent<RectTransform>();

            item_tooltip = GameObject.Find("ItemToolTip");
            item_tooltip_rt = item_tooltip.GetComponent<RectTransform>();
            item_tooltip_text = GameObject.Find("ItemToolTipText").GetComponent<TextMeshProUGUI>();

            player_info = GameObject.Find("PlayerInfo").GetComponent<TextMeshProUGUI>();
            debug_info = GameObject.Find("DebugInfo").GetComponent<TextMeshProUGUI>();
            hp_info = GameObject.Find("PlayerHPText").GetComponent<TextMeshProUGUI>();
            xp_info = GameObject.Find("PlayerXPText").GetComponent<TextMeshProUGUI>();
            player_hp_bar = GameObject.Find("PlayerHPBar").GetComponent<Slider>();
            player_xp_bar = GameObject.Find("PlayerXPBar").GetComponent<Slider>();

            pause_overlay = GameObject.Find("PauseScreen");
            if (pause_overlay != null) {
                Button resume_button = GameObject.Find("ResumeButton").GetComponent<Button>();
                Button exit_button = GameObject.Find("MenuButton").GetComponent<Button>();
                resume_button.onClick.AddListener(() => {
                        pause_overlay.SetActive(false);
                        Time.timeScale = 1;
                        PlaySwapSound();
                        });
                exit_button.onClick.AddListener(() => {
                        GameState.Reset();
                        Time.timeScale = 1;
                        SceneManager.LoadScene("Menu");
                        PlaySwapSound();
                        });
                pause_overlay.SetActive(false);
            }

            BuildSkillTree();

            SetHeldItem();
            HideUI();

            fade_in_left = fade_in;
        }

        void Update()
        {
            if (player == null || player_trf == null || player_stats == null) {
                player = GameObject.Find("Player");
                player_trf = player.GetComponent<Transform>();
                player_stats = player.GetComponent<PlayerStats>();
            }

            item_tooltip.SetActive(false);

            hp_info.text = string.Format("{0}/{1} HP", player_stats.hp, player_stats.hp_max);
            xp_info.text = string.Format("{0}/{1} XP", player_stats.xp, player_stats.xp_max, player_stats.level);
            player_hp_bar.value = (float)player_stats.hp / player_stats.hp_max;
            player_xp_bar.value = (float)player_stats.xp / player_stats.xp_max;

            if (fade_in_left > 0) {
                level_intro_text.text = GameState.level_name;
                screen_color_img.color = new Color(0, 0, 0, 1 - ((1 - fade_in_left) / fade_in));
                fade_in_left -= Time.deltaTime;
                if (fade_in_left <= 0) {
                    screen_color_img.color = new Color(1, 1, 1, 0);
                    screen_color.SetActive(false);
                }
                return;
            }

            DrawDebugInfo();

            Vector3 mouse_pos = Input.mousePosition;

            float tooltip_offs = (mouse_pos.x < Screen.width / 2) ? 175 : -175;
            item_tooltip_rt.position = new Vector3(mouse_pos.x + tooltip_offs, mouse_pos.y, 0);
            player_cursor_img_rt.position = new Vector3(mouse_pos.x, mouse_pos.y, 0);

            if (player_stats.currently_held_item != null) {
                SetHeldItem();
                player_cursor_img.SetActive(true);
            } else {
                player_cursor_img.SetActive(false);
            }

            if (player_stats.skill_points > 0) {
                skill_tree_plus_button.SetActive(true);
            } else {
                skill_tree_plus_button.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Escape)) {
                active_attack_button.SendMessage("Hide");
                active_spell_button.SendMessage("Hide");
                if (is_inventory_active || stats_toggled || skill_tree_toggled) {
                    HideUI();
                } else {
                    if (Time.timeScale < 1) {
                        Time.timeScale = 1;
                        pause_overlay.SetActive(false);
                    } else {
                        PlaySwapSound();
                        Time.timeScale = 0;
                        pause_overlay.SetActive(true);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.T)) {
                ToggleSkillTree();
            }

            if (Input.GetKeyDown(KeyCode.C)) {
                ToggleStats();
            }

            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.E)) {
                ToggleInventory();
            }

            RemovePlayerOcclusion();
        }

        void LateUpdate()
        {
            current_ui_object_hovered = ui_test.PointerOverWhichUIElement();
            is_ui_object_hovered = (current_ui_object_hovered != null);
        }

        void PollKeys()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                HideUI();
            }

            if (Input.GetKeyDown(KeyCode.T) || Input.GetKeyDown(KeyCode.C)) {
                ToggleSkillTree();
            }

            if (Input.GetKeyDown(KeyCode.I) || Input.GetKeyDown(KeyCode.E)) {
                ToggleInventory();
            }
        }

        void ToggleUI()
        {
            toggled = !toggled;
            skill_tree.SetActive(toggled);
            inventory.SetActive(toggled);
            stats.SetActive(toggled);
            player_cursor_img.SetActive(player_stats.currently_held_item != null && player_stats.currently_held_item.should_show);
            skill_tree_toggled = toggled;
            inventory_toggled = toggled;
            stats_toggled = toggled;

            if (toggled) {
                ShowInventory();
            }
        }

        void HideUI()
        {
            skill_tree.SetActive(false);
            stats.SetActive(false);
            HideInventory();
            skill_tree_toggled = false;
            stats_toggled = false;
            inventory_toggled = false;
        }

        public void ToggleStats()
        {
            stats_toggled = !stats_toggled;
            stats.SetActive(stats_toggled);
            Sync();

            if (stats_toggled && skill_tree_toggled) {
                Sync();
                ToggleSkillTree();
            } else {
            }
        }

        public void ToggleSkillTree()
        {
            skill_tree_toggled = !skill_tree_toggled;
            skill_tree.SetActive(skill_tree_toggled);
            Sync();

            if (skill_tree_toggled) {
                if (stats_toggled) {
                    ToggleStats();
                }
            }
        }

        public void ToggleInventory()
        {
            is_inventory_active = !is_inventory_active;

            if (is_inventory_active) {
                ShowInventory();
            } else {
                HideInventory();
            }
        }

        public void ShowInventory()
        {
            // HideUI();
            inventory.SetActive(true);
            inventory_script.Draw();
            is_inventory_active = true;

            if (player_stats != null && player_stats.currently_held_item != null && player_stats.currently_held_item.should_show) {
                SetHeldItem();
                player_cursor_img.SetActive(true);
            } else {
                player_cursor_img.SetActive(false);
            }
        }

        void HideInventory()
        {
            inventory.SetActive(false);
            player_cursor_img.SetActive(false);
            is_inventory_active = false;
        }

        void BuildSkillTree()
        {
            List<GameObject> skill_tree_elements = new List<GameObject>();
            foreach (Transform t in skill_tree_container.transform) {
                skill_tree_elements.Add(t.gameObject);
            }

            int i = 0;
            for (int j = 0; j < GameState.attacks.Count; j++) {
                SkillTreeElementScript script = skill_tree_elements[i].GetComponent<SkillTreeElementScript>();
                script.attack = GameState.attacks[j];
                i++;
            }
            for (int j = 0; j < GameState.spells.Count; j++) {
                SkillTreeElementScript script = skill_tree_elements[i].GetComponent<SkillTreeElementScript>();
                script.spell = GameState.spells[j];
                i++;
            }
            foreach (GameObject elm in skill_tree_elements) {
                SkillTreeElementScript script = elm.GetComponent<SkillTreeElementScript>();
                script.Init();
            }
        }

        public void SetHeldItem()
        {
            if (player_stats == null) {
                player_cursor_img.SetActive(false);
                return;
            }

            if (player_stats.currently_held_item == null || !player_stats.currently_held_item.should_show) {
                player_cursor_img.SetActive(false);
                return;
            }

            Image img = player_cursor_img.GetComponent<Image>();
            img.sprite = GameData.item_sprites[player_stats.currently_held_item.sprite_index];
        }

        public void Sync()
        {
            stats_name_text.text = string.Format("Wizard");
            stats_misc_text.text = string.Format("Level: {0}   XP: {1}/{2}{3}Gold: {4}",
                    player_stats.level, player_stats.xp, player_stats.xp_max, Environment.NewLine, player_stats.gold);
            stats_attack_text.text = string.Format("Normal: {0} Fire: {1} Ice: {2}{3}AoE: {4}%   Speed: {5}%",
                    player_stats.attack_damage, player_stats.attack_damage_fire, player_stats.attack_damage_ice,
                    Environment.NewLine, ((int)(player_stats.attack_scale * 100)), ((int)(player_stats.attack_speed * 100)));
            stats_spell_text.text = string.Format("Normal: {0} Fire: {1} Ice: {2}{3}AoE: {4}%   Speed: {5}%{6}Cooldown Reduction: {7}%",
                    player_stats.spell_damage, player_stats.spell_damage_fire, player_stats.spell_damage_ice,
                    Environment.NewLine, ((int)(player_stats.spell_scale * 100)), ((int)(player_stats.spell_speed * 100)),
                    Environment.NewLine, ((int)(player_stats.spell_cooldown * 100)));
            stats_defense_text.text = string.Format("HP: {0}/{1}   Defense: {2}{3}Hit Recovery: {4:0.00}",
                    player_stats.hp, player_stats.hp_max, player_stats.defense, Environment.NewLine, player_stats.stun_lock);
            stats_util_text.text = string.Format("Movement Speed: {0}%", (100 + (int)(player_stats.move_speed_bonus * 100)));

            skill_tree_free_text.text = player_stats.skill_points.ToString();

            SetHeldItem();

            if (is_inventory_active && player_stats.currently_held_item != null) {
                player_cursor_img.SetActive(true);
            }

            if (player_stats.skill_points < 1) {
                skill_tree_plus_button.SetActive(false);
            } else {
                skill_tree_plus_button.SetActive(true);
            }
        }

        void DrawDebugInfo()
        {
            fps_hist[fps_hist_i % 60] = 1 / Time.deltaTime; float fps_avg = 0; fps_hist_i = (fps_hist_i + 1) % 60;
            for (int i = 0; i < 60; i++) { fps_avg += fps_hist[i]; }

            if (player_trf.hasChanged) {
                debug_info.text = string.Format("player_pos: {0} | fps: {1: .00}",
                    player_trf.position.ToString(), fps_avg / 60.0f);
            }
        }

        public void PlaySwapSound()
        {
            audio_source.clip = audio_clips[2];
            audio_source.Play();
        }

        public void PlaySwapFailSound()
        {
            audio_source.clip = audio_clips[1];
            audio_source.Play();
        }

        public void PlayBuffSound()
        {
            audio_source.clip = audio_clips[3];
            audio_source.Play();
        }

        void RemovePlayerOcclusion()
        {
            RaycastHit hit_info;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            Physics.Raycast(ray, out hit_info, 500, 1 << 11);

            List<GameObject> occluding_gameobjects_new = new List<GameObject>();

            Collider hit_c = hit_info.collider;
            GameObject hit_g = (hit_c != null) ? hit_c.gameObject : null;

            foreach (GameObject g in occluding_gameobjects) {
                if (hit_c == null || hit_g == null || hit_g != g) {
                    StonePillarScript sps = g.GetComponent<StonePillarScript>();
                    sps.UnHide();
                } else {
                    occluding_gameobjects_new.Add(g);
                }
            }

            if (hit_c != null && hit_g != null && !occluding_gameobjects_new.Contains(hit_g)) {
                StonePillarScript sps = hit_g.GetComponent<StonePillarScript>();
                sps.Hide();
                occluding_gameobjects_new.Add(hit_g);
            }

            occluding_gameobjects = occluding_gameobjects_new;
        }

    }
}

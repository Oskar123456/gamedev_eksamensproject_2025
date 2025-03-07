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
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Spells;
using Attacks;
using Player;

namespace UI
{
    public class UIScript : MonoBehaviour
    {
        public GameObject skill_tree_element;
        public GameObject skill_tree_element_icon;
        public GameObject skill_tree_element_button;
        public GameObject skill_tree_element_description;

        AudioSource audio_source;
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
        TextMeshProUGUI stats_text;

        GameObject skill_tree;
        GameObject inventory;
        GameObject stats;
        InventoryScript inventory_script;

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
            skill_tree_plus_button = GameObject.Find("SkillTreePlusButton");
            inventory = GameObject.Find("Inventory");
            inventory_script = inventory.GetComponent<InventoryScript>();
            stats = GameObject.Find("Stats");
            stats_text = GameObject.Find("StatsText").GetComponent<TextMeshProUGUI>();
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

            DrawDebugInfo();

            item_tooltip.SetActive(false);

            hp_info.text = string.Format("{0}/{1} HP", player_stats.hp, player_stats.hp_max);
            xp_info.text = string.Format("{0}/{1} XP [Level {2}]", player_stats.xp, player_stats.xp_max, player_stats.level);
            player_hp_bar.value = (float)player_stats.hp / player_stats.hp_max;
            player_xp_bar.value = (float)player_stats.xp / player_stats.xp_max;

            if (is_inventory_active) {
                Vector3 mouse_pos = Input.mousePosition;
                player_cursor_img_rt.position = new Vector3(mouse_pos.x, mouse_pos.y, 0);
                item_tooltip_rt.position = new Vector3(mouse_pos.x - 225, mouse_pos.y, 0);
            }

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

            if (Input.GetKeyDown(KeyCode.Escape)) {
                HideUI();
                active_attack_button.SendMessage("Hide");
                active_spell_button.SendMessage("Hide");
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
        }

        void ToggleStats()
        {
            stats_toggled = !stats_toggled;
            stats.SetActive(stats_toggled);
            Sync();

            if (stats_toggled) {
            } else {
            }
        }

        void ToggleSkillTree()
        {
            skill_tree_toggled = !skill_tree_toggled;
            skill_tree.SetActive(skill_tree_toggled);

            if (skill_tree_toggled) {
                stats.SetActive(false);
                BuildSkillTree();
            } else {
                foreach (Transform t in skill_tree.transform) {
                    Destroy(t.gameObject);
                }
            }
        }

        void ToggleInventory()
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
            for (int i = 0; i < player_stats.learned_spells.Count; i++) {
                GameObject container = Instantiate(skill_tree_element, Vector3.zero, Quaternion.identity, skill_tree.transform);
                GameObject icon = Instantiate(skill_tree_element_icon, Vector3.zero, Quaternion.identity, container.transform);
                GameObject description = Instantiate(skill_tree_element_description, Vector3.zero, Quaternion.identity, container.transform);

                string descr = player_stats.learned_spells[i].GetLevelUpDescriptionString(" ", Environment.NewLine, player_stats);
                description.GetComponent<TextMeshProUGUI>().text = descr;

                Sprite sprite = GameData.spell_sprites[player_stats.learned_spells[i].sprite_index];
                icon.GetComponent<Image>().sprite = sprite;

                if (player_stats.skill_points > 0) {
                    GameObject button = Instantiate(skill_tree_element_button, Vector3.zero, Quaternion.identity, container.transform);
                    int ii = i;
                    Button b = button.GetComponent<Button>();
                    b.onClick.AddListener(() => {
                            player.SendMessage("OnLevelUpSpell", ii);
                            BuildSkillTree();
                            audio_source.Play();
                            if (player_stats.skill_points < 1) {
                                skill_tree_plus_button.SetActive(false);
                            }
                            });
                    RectTransform rt_button = button.GetComponent<RectTransform>();
                    rt_button.anchoredPosition = new Vector2(-100, 0);
                }


                RectTransform rt_container = container.GetComponent<RectTransform>();
                RectTransform rt_icon = icon.GetComponent<RectTransform>();
                RectTransform rt_text = description.GetComponent<RectTransform>();

                rt_container.anchoredPosition = new Vector2(0, 430 - (i + 1) * 120 - i * 20);
                rt_icon.anchoredPosition = new Vector2(-200, 0);
                rt_text.anchoredPosition = new Vector2(110, -15);
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
            stats_text.text = player_stats.ToString();
            SetHeldItem();

            if (is_inventory_active && player_stats.currently_held_item != null) {
                player_cursor_img.SetActive(true);
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

            if (player_trf.hasChanged) {
                player_info.text = string.Format("stats: damage: {0} | attack-speed/-scale: {1: .00}/{2: .00}",
                    player_stats.attack_damage, player_stats.attack_speed, player_stats.attack_scale);
            }

        }
    }
}

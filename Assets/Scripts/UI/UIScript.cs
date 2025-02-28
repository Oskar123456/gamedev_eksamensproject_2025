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
using Attacks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Spells;
using Attacks;

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
        GameObject skill_tree_plus_button;
        Transform player_trf;
        GameObject screen_color;
        Image screen_color_img;

        Slider player_hp_bar;
        Slider player_xp_bar;

        TextMeshProUGUI player_info;
        TextMeshProUGUI debug_info;
        TextMeshProUGUI hp_info;
        TextMeshProUGUI xp_info;
        TextMeshProUGUI level_intro_text;

        GameObject skill_tree;
        GameObject inventory;

        PlayerStats player_stats;

        public float fade_in = 1;
        float fade_in_left;

        float[] fps_hist;
        int fps_hist_i;

        bool toggled;
        bool skill_tree_toggled;
        bool inventory_toggled;

        void Start()
        {
            /* debug */
            fps_hist = new float[60];
            /* level intro */
            screen_color = GameObject.Find("ScreenPanel");
            screen_color_img = screen_color.GetComponent<Image>();
            screen_color_img.color = Color.black;
            level_intro_text = GameObject.Find("LevelIntroText").GetComponent<TextMeshProUGUI>();
            level_intro_text.text = GameState.level_name;
            /* game UI */
            audio_source = GetComponent<AudioSource>();

            skill_tree = GameObject.Find("SkillTree");
            skill_tree_plus_button = GameObject.Find("SkillTreePlusButton");
            inventory = GameObject.Find("Inventory");
            player = GameObject.Find("Player");
            player_trf = player.GetComponent<Transform>();
            player_stats = player.GetComponent<PlayerStats>();

            player_info = GameObject.Find("PlayerInfo").GetComponent<TextMeshProUGUI>();
            debug_info = GameObject.Find("DebugInfo").GetComponent<TextMeshProUGUI>();
            hp_info = GameObject.Find("PlayerHPText").GetComponent<TextMeshProUGUI>();
            xp_info = GameObject.Find("PlayerXPText").GetComponent<TextMeshProUGUI>();
            player_hp_bar = GameObject.Find("PlayerHPBar").GetComponent<Slider>();
            player_xp_bar = GameObject.Find("PlayerXPBar").GetComponent<Slider>();

            HideUI();
            skill_tree_plus_button.SetActive(false);

            player_hp_bar.value = player_stats.hp / player_stats.hp_max;
            player_xp_bar.value = player_stats.xp / player_stats.xp_max;

            fade_in_left = fade_in;
        }

        void Update()
        {
            DrawDebugInfo();

            hp_info.text = string.Format("{0}/{1} HP", player_stats.hp, player_stats.hp_max);
            xp_info.text = string.Format("{0}/{1} XP [Level {2}]", player_stats.xp, player_stats.xp_max, player_stats.level);
            player_hp_bar.value = (float)player_stats.hp / player_stats.hp_max;
            player_xp_bar.value = (float)player_stats.xp / player_stats.xp_max;

            if (Input.GetKeyDown(KeyCode.Escape)) {
            }

            if (fade_in > 0) {
                screen_color_img.color = new Color(0, 0, 0, 1 - ((1 - fade_in_left) / fade_in));
                fade_in_left -= Time.deltaTime;
                if (fade_in_left <= 0) {
                    screen_color_img.color = new Color(1, 1, 1, 0);
                    screen_color.SetActive(false);
                }
                return;
            }
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
            skill_tree_toggled = toggled;
            inventory_toggled = toggled;
        }

        void HideUI()
        {
            skill_tree.SetActive(false);
            inventory.SetActive(false);
        }

        void ToggleSkillTree()
        {
            skill_tree_toggled = !skill_tree_toggled;
            skill_tree.SetActive(skill_tree_toggled);

            if (skill_tree_toggled) {
                BuildSkillTree();
            } else {
                foreach (Transform t in skill_tree.transform) {
                    Destroy(t.gameObject);
                }
            }
        }

        void ToggleInventory()
        {
            inventory_toggled = !inventory_toggled;
            inventory.SetActive(inventory_toggled);
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

        void BuildSkillTree()
        {
            for (int i = 0; i < player_stats.learned_spells.Count; i++) {
                GameObject container = Instantiate(skill_tree_element, Vector3.zero, Quaternion.identity, skill_tree.transform);
                GameObject icon = Instantiate(skill_tree_element_icon, Vector3.zero, Quaternion.identity, container.transform);
                GameObject description = Instantiate(skill_tree_element_description, Vector3.zero, Quaternion.identity, container.transform);

                string descr = player_stats.learned_spells[i].GetLevelUpDescriptionString(" ", Environment.NewLine, player_stats);
                description.GetComponent<TextMeshProUGUI>().text = descr;

                Sprite sprite = player_stats.learned_spells[i].sprite;
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
    }
}

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

namespace UI
{

    public class SkillTreeElementScript : MonoBehaviour
    {
        UIScript ui_script;
        GameObject player;
        PlayerStats player_stats;
        GameObject child;
        GameObject child_frame;
        TextMeshProUGUI level_info_text;
        TextMeshProUGUI name_text;

        public Spell spell;
        public Attack attack;

        bool first = true;

        void Start()
        {
        }

        public void Init()
        {
            Image img = GetComponent<Image>();

            child_frame = transform.GetChild(0).gameObject;
            child = transform.GetChild(1).gameObject;
            level_info_text = transform.GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();
            name_text = transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>();

            if (spell == null && attack == null) {
                gameObject.SetActive(false);
                return;
            }

            if (spell != null) {
                img.sprite = GameData.spell_sprites[spell.sprite_index];
                level_info_text.text = spell.level.ToString();
                name_text.text = spell.name;
            }
            if (attack != null) {
                img.sprite = GameData.attack_sprites[attack.sprite_index];
                level_info_text.text = attack.level.ToString();
                name_text.text = attack.name;
            }

            ui_script = GameObject.Find("UI").GetComponent<UIScript>();
            Button b = child.GetComponent<Button>();
            b.onClick.AddListener(ClickCallBack);
        }

        void LateUpdate()
        {
            if (player_stats == null) {
                player = GameObject.Find("Player");
                if (player == null) {
                    return;
                }
                player_stats = player.GetComponent<PlayerStats>();
            }

            if (spell == null && attack == null) {
                return;
            }

            if (first) {
                if (spell != null) {
                    level_info_text.text = spell.level.ToString();
                }
                if (attack != null) {
                    level_info_text.text = attack.level.ToString();
                }
                first = false;
            }

            if (player_stats.skill_points > 0) {
                child.SetActive(true);
            } else {
                child.SetActive(false);
            }

            if (ui_script.current_ui_object_hovered == gameObject || ui_script.current_ui_object_hovered == child || ui_script.current_ui_object_hovered == child_frame) {
                if (spell != null) {
                    if (spell.level == 0) {
                        ui_script.item_tooltip_text.text = "Not yet learned " + spell.name + Environment.NewLine + Environment.NewLine + spell.description;
                    } else {
                        ui_script.item_tooltip_text.text = spell.description + Environment.NewLine + Environment.NewLine
                            + "Next level: " + Environment.NewLine + spell.GetLevelUpDescriptionString(Environment.NewLine);
                    }
                }
                if (attack != null) {
                    if (attack.level == 0) {
                        ui_script.item_tooltip_text.text = "Not yet learned " + attack.name + Environment.NewLine + Environment.NewLine + attack.description;
                    } else {
                        ui_script.item_tooltip_text.text = attack.description + Environment.NewLine + Environment.NewLine
                            + "Next level: " + Environment.NewLine + attack.GetLevelUpDescriptionString(Environment.NewLine);
                    }
                }
                ui_script.item_tooltip.SetActive(true);
            }
        }

        void ClickCallBack()
        {
            if (player_stats == null) {
                GameObject player = GameObject.Find("Player");
                if (player == null) {
                    return;
                }
                player_stats = player.GetComponent<PlayerStats>();
            }

            if (player_stats.skill_points > 0) {
                if (spell != null) {
                    int idx = 0;

                    if (!player_stats.learned_spells.Contains(spell)) {
                        player_stats.learned_spells.Add(spell);
                    }

                    while (player_stats.learned_spells[idx] != spell) {
                        idx++;
                    }

                    player.SendMessage("OnLevelUpSpell", idx);
                    level_info_text.text = spell.level.ToString();
                } else if (attack != null) {
                    int idx = 0;

                    if (!player_stats.learned_attacks.Contains(attack)) {
                        player_stats.learned_attacks.Add(attack);
                    }

                    while (player_stats.learned_attacks[idx] != attack) {
                        idx++;
                    }

                    player.SendMessage("OnLevelUpAttack", idx);
                    level_info_text.text = attack.level.ToString();
                }
            } else {
                ui_script.PlaySwapFailSound();
            }

            ui_script.PlayBuffSound();
            ui_script.Sync();
        }
    }

}

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
using UI;

namespace Loot
{

    public class LootButtonOverlayScript : MonoBehaviour
    {
        public Item item;
        UIScript ui_script;

        void Start()
        {
            ui_script = GameObject.Find("UI").GetComponent<UIScript>();
        }

        void Update()
        {
        }

        void LateUpdate()
        {
            if (ui_script.current_ui_object_hovered == gameObject) {
                ui_script.item_tooltip_text.text = item.EffectString();
                ui_script.item_tooltip.SetActive(true);
            }
        }
    }

}

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

    public class SkillTreeButtonScript : MonoBehaviour
    {
        UIScript ui_script;

        void Start()
        {
            ui_script = GameObject.Find("UI").GetComponent<UIScript>();
            Button b = GetComponent<Button>();
            b.onClick.AddListener(ClickCallBack);
        }

        void Update() { }

        void ClickCallBack()
        {
            ui_script.ToggleSkillTree();
            ui_script.PlaySwapSound();
        }
    }

}

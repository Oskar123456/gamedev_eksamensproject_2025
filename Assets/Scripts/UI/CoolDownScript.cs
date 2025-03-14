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

public class CoolDownScript : MonoBehaviour
{
    Slider slider;

    float cooldown_init;
    float cooldown_left;

    void Start()
    {
        slider = GetComponent<Slider>();
        slider.value = 0;
    }

    void Update()
    {
        if (cooldown_left > 0) {
            cooldown_left -= Time.deltaTime;
            slider.value = cooldown_left / cooldown_init;
        }
    }

    public void SetCoolDown(float c)
    {
        cooldown_init = c;
        cooldown_left = c;
    }
}

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

namespace Loot
{

    public class LeatherHelmet : Helmet
    {
        public LeatherHelmet()
        {
            should_show = true;
            weight = 500;
            prefab_index = 1;
            sprite_index = 20;
            name = "Leather helmet";
            description = "Helmet made of hardened leather";
            hp = GameState.rng.Next() % 5 + 1;
        }
    }

}


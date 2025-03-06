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

namespace Loot
{

    public class DropScript : MonoBehaviour
    {
        public Item item;

        void Start()
        {

        }

        void OnTriggerEnter(Collider c)
        {
            if (c.gameObject.tag != "Player") {
                return;
            }

            GetComponent<AudioSource>().Play();
            c.gameObject.SendMessage("OnPickUp", item, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }
    }

}

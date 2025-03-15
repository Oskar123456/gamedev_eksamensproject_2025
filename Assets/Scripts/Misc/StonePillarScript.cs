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
using Spells;
using Attacks;
using Player;

namespace Misc
{

    public class StonePillarScript : MonoBehaviour
    {
        public Material mat_opaque;
        public Material mat_transparent;

        public void Hide()
        {
            foreach (Transform t in transform) {
                Renderer renderer = t.gameObject.GetComponent<Renderer>();
                if (renderer == null) {
                    continue;
                }
                renderer.material = mat_transparent;
            }
        }

        public void UnHide()
        {
            foreach (Transform t in transform) {
                Renderer renderer = t.gameObject.GetComponent<Renderer>();
                if (renderer == null) {
                    continue;
                }
                renderer.material = mat_opaque;
            }
        }
    }

}

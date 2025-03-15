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
using UI;
using Loot;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FootStepsScript : MonoBehaviour
{
    public List<AudioClip> sounds;
    AudioSource audio_source;

    System.Random rng = new System.Random();

    void Start()
    {
        audio_source = GetComponent<AudioSource>();
    }

    public void PlayRandom()
    {
        audio_source.clip = sounds[rng.Next(sounds.Count)];
        audio_source.Play();
    }
}

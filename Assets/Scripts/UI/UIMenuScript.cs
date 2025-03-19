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

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class UIMenuScript : MonoBehaviour
    {
        GameObject screen_color;
        Image screen_color_img;

        Button play_button;
        TextMeshProUGUI status;

        public float fade_in = 1;
        float fade_in_left;

        AudioSource audio_source;
        public AudioClip[] audio_clips;

        void Awake()
        {
            audio_source = GetComponent<AudioSource>();
        }

        void Start()
        {
            /* level intro */
            screen_color = GameObject.Find("ScreenPanel");
            screen_color_img = screen_color.GetComponent<Image>();
            screen_color_img.color = Color.black;

            if (GameObject.Find("PlayButton") != null) {
                play_button = GameObject.Find("PlayButton").GetComponent<Button>();
                play_button.onClick.AddListener(PlayGame);
            }

            if (GameObject.Find("Status") != null) {
                status = GameObject.Find("Status").GetComponent<TextMeshProUGUI>();
                if (GameState.has_died) {
                    status.text = "YOU DIED...";
                    if (audio_source != null) {
                        audio_source.clip = audio_clips[1];
                        audio_source.Play();
                    }
                } else {
                    status.text = "Welcome...";
                }
            }

            GameState.has_died = false;

            fade_in_left = fade_in;

            if (GameObject.Find("Player") != null)
                Destroy(GameObject.Find("Player"));
        }

        void Update()
        {
            if (fade_in > 0) {
                screen_color_img.color = new Color(0, 0, 0, 1 - ((1 - fade_in_left) / fade_in));
                fade_in_left -= Time.deltaTime;
                if (fade_in_left <= 0) {
                    screen_color_img.color = new Color(1, 1, 1, 0);
                    screen_color.SetActive(false);
                }
            }
        }

        void PlayGame()
        {
            if (audio_source != null) {
                audio_source.clip = audio_clips[1];
                audio_source.Play();
            }
            SceneManager.LoadScene("Char_select_screen");
        }
    }
}

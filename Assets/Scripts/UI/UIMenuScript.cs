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
        public AudioClip[] audio_clips;

        GameObject screen_color;
        Image screen_color_img;
        AudioSource audio_source;

        Button play_button;
        // TextMeshProUGUI title;
        TextMeshProUGUI status;

        public float fade_in = 2;
        float fade_in_left;

        void Awake()
        {
        }

        void Start()
        {
            /* level intro */
            screen_color = GameObject.Find("ScreenPanel");
            screen_color_img = screen_color.GetComponent<Image>();
            screen_color_img.color = Color.black;

            play_button = GameObject.Find("PlayButton").GetComponent<Button>();
            play_button.onClick.AddListener(PlayGame);

            audio_source = GetComponent<AudioSource>();
            // title = GameObject.Find("Title").GetComponent<TextMeshProUGUI>();
            status = GameObject.Find("Status").GetComponent<TextMeshProUGUI>();
            /* game UI */
            if (GameState.has_died) {
                status.text = "YOU DIED...";
                audio_source.clip = audio_clips[1];
                audio_source.Play();
                fade_in_left = fade_in * 3;
            } else {
                status.text = "Welcome...";
                fade_in_left = fade_in;
            }

            GameState.has_died = false;

            if (GameObject.Find("Player") != null) {
                Destroy(GameObject.Find("Player"));
            }
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
            SceneManager.LoadScene("Town");
        }
    }
}

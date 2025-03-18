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
        // TextMeshProUGUI title;
        TextMeshProUGUI status;

        public float fade_in = 1;
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
            // title = GameObject.Find("Title").GetComponent<TextMeshProUGUI>();
            status = GameObject.Find("Status").GetComponent<TextMeshProUGUI>();
            /* game UI */
            if (GameState.has_died) {
                status.text = "YOU DIED...";
            } else {
                status.text = "Welcome...";
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
            SceneManager.LoadScene("Char_select_screen");
        }
    }
}

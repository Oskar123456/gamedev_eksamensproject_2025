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

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIEventTextScript : MonoBehaviour
{
    TextMeshProUGUI txt;
    RectTransform rtf;

    public Color txt_color;
    public float fade_in = 1;
    float fade_in_left;

    void Awake()
    {
        txt = GetComponent<TextMeshProUGUI>();
        rtf = GetComponent<RectTransform>();
    }

    void Start()
    {
        Destroy(gameObject, fade_in);
        fade_in_left = fade_in;
    }

    void Update()
    {
        if (fade_in > 0) {
            txt.color = new Color(txt_color.r, txt_color.g, txt_color.b, 1 - ((1 - fade_in_left) / fade_in));
            fade_in_left -= Time.deltaTime;
            rtf.anchoredPosition = new Vector2(0, ((1 - fade_in_left) / fade_in) * 100);
        }
    }
}

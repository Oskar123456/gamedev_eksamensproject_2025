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

public class SkillTreePlusScript : MonoBehaviour
{
    GameObject UI;

    void Start()
    {
        UI = GameObject.Find("UI");
        Button b = GetComponent<Button>();
        b.onClick.AddListener(() => {
                UI.SendMessage("PlaySwapSound");
                UI.SendMessage("ToggleSkillTree");
                });
    }
}

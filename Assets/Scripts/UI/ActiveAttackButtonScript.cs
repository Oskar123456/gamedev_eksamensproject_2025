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
using UnityEngine.UI;

public class ActiveAttackButtonScript : MonoBehaviour
{
    public GameObject ability_button_prefab;
    Button button;
    GameObject player;
    PlayerStats player_stats;

    bool open;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(Toggle);
    }

    void Update()
    {
        if (player == null || player_stats == null) {
            player = GameObject.Find("Player");
            player_stats = player.GetComponent<PlayerStats>();
        }
        // TODO: Cooldown fade
    }

    void Hide()
    {
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
        open = false;
    }

    void Toggle()
    {
        if (!open) {
            for (int i = 0; i < player_stats.learned_attacks.Count; i++) {
                Sprite icon = GameData.attack_sprites[player_stats.learned_spells[i].sprite_index];
                GameObject ability = Instantiate(ability_button_prefab, Vector3.zero, Quaternion.identity, transform);
                Image img = ability.GetComponent<Image>();
                img.sprite = icon;

                int ii = i;

                Button ability_button = ability.GetComponent<Button>();
                ability_button.onClick.AddListener(() => {
                        player.SendMessage("ChangeActiveAttack", ii);
                        Toggle();
                        });

                RectTransform rt = ability.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(0, (i + 1) * 150 + (i + 1) * 20);
            }
        } else {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
        }

        open = !open;
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectScript : MonoBehaviour
{
    public GameObject objectA; // Assign in Inspector (e.g., Mage Preview)
    public GameObject objectB; // Assign in Inspector (e.g., Warrior Preview)
    GameObject screen_color;
    Image screen_color_img;

    Button confirm_button;
    // TextMeshProUGUI title;
    TextMeshProUGUI status;

    public float fade_in = 1;
    float fade_in_left;

    public GameObject characterPrefab1;  // Assign in Inspector
    public GameObject characterPrefab2;  // Assign in Inspector

    public Button character1Button;
    public Button character2Button;
    public Button confirmButton;

    private string selectedCharacterName = "Player";
    private GameObject selectedPrefab;

    void Start()
    {
        
        character1Button.onClick.AddListener(() => SelectCharacter(characterPrefab1, "Mage"));
        character2Button.onClick.AddListener(() => SelectCharacter(characterPrefab2, "Warrior"));

        /* level intro */
        screen_color = GameObject.Find("ScreenPanel");
        screen_color_img = screen_color.GetComponent<Image>();
        screen_color_img.color = Color.black;

        confirm_button = GameObject.Find("Confirm").GetComponent<Button>();
        confirmButton.onClick.AddListener(ConfirmSelection);
        // title = GameObject.Find("Title").GetComponent<TextMeshProUGUI>();
        // status = GameObject.Find("Status").GetComponent<TextMeshProUGUI>();
        /* game UI */
        // if (GameState.has_died) {
        //     status.text = "YOU DIED...";
        // } else {
        //     status.text = "Welcome...";
        // }
        GameState.has_died = false;

        fade_in_left = fade_in;
         
        objectA.SetActive(false);
        objectB.SetActive(false);

        if (GameObject.Find("Player") != null)
            Destroy(GameObject.Find("Player"));
    }
    void SelectCharacter(GameObject prefab, string characterName)
    {
        selectedPrefab = prefab;
        selectedCharacterName = characterName;
        GameState.ChangePf(prefab);

        // Toggle between objectA and objectB
        if (prefab == characterPrefab1)
        {
            objectA.SetActive(true);
            objectB.SetActive(false);
        }
        else if (prefab == characterPrefab2)
        {
            objectA.SetActive(false);
            objectB.SetActive(true);
        }

        Debug.Log("Selected character: " + selectedCharacterName);
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

    void ConfirmSelection()
    {
        if (selectedPrefab != null)
        {
            GameState.Instance.player_prefab = selectedPrefab;
            GameState.ChangePf(selectedPrefab);
            PlayerPrefs.SetString("SelectedCharacter", selectedCharacterName);
            PlayerPrefs.Save();
            SceneManager.LoadScene("Town");
        }
        else
        {
            Debug.Log("No character selected!");
        }
    }
}

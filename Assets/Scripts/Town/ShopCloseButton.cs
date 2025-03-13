using UnityEngine;
using UnityEngine.UI;

public class ShopCloseButton : MonoBehaviour
{
    public NPCInteractable npcInteractable; // Reference to the NPCInteractable script

    void Start()
    {
        // Add a listener to the button's onClick event
        GetComponent<Button>().onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnCloseButtonClicked()
    {
        // Call the CloseShop method in the NPCInteractable script
        npcInteractable.CloseShop();
    }
}
using System.Collections.Generic;
using Player;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Loot;

public class ShopManager : MonoBehaviour
{

    public int[,] ShopItems = new int[5, 5];
    public TextMeshProUGUI CoinsText;
    private PlayerStats playerStats;


    void Start()
    {
        // Initialize shop items (IDs, prices, quantities)
        ShopItems[1, 1] = 1;
        ShopItems[1, 2] = 2;
        ShopItems[1, 3] = 3;
        ShopItems[1, 4] = 4;

        ShopItems[2, 1] = Random.Range(15, 55); // Price for item 1
        ShopItems[2, 2] = Random.Range(35, 70); // Price for item 2
        ShopItems[2, 3] = Random.Range(25, 90); // Price for item 3
        ShopItems[2, 4] = Random.Range(45, 100); // Price for item 4

        ShopItems[3, 1] = 0; // Quantity for item 1
        ShopItems[3, 2] = 0; // Quantity for item 2
        ShopItems[3, 3] = 0; // Quantity for item 3
        ShopItems[3, 4] = 0; // Quantity for item 4
    }

    void Update()
    {
        CheckForPlayer();
    }

    bool CheckForPlayer()
    {
        if (playerStats == null) {
            GameObject player = GameObject.Find("Player");
            if (player == null) {
                Debug.Log("player not found");
                return false;
            }
            playerStats = player.GetComponent<PlayerStats>();
        }

        CoinsText.text = "Coins: " + playerStats.gold;

        return playerStats != null;
    }

    public void Buy()
    {
        if (!CheckForPlayer()) {
            Debug.Log("player not found");
            return;
        }

        GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;

        int itemId = ButtonRef.GetComponent<ButtonInfo>().ItemId;

        if (playerStats.gold >= ShopItems[2, itemId])
        {
            playerStats.gold -= ShopItems[2, itemId];
            ShopItems[3, itemId]++;
            CoinsText.text = "Coins: " + playerStats.gold;
            ButtonRef.GetComponent<ButtonInfo>().QuantityTxt.text = ShopItems[3, itemId].ToString();

            Item boughtItem = CreateItemFromId(itemId); // Create item based on itemId
            AddItemToInventory(boughtItem); // Add the created item to the player's inventory

        }
    }
    // Create the item from its ID (You may have a different way to create these items)
    private Item CreateItemFromId(int itemId)
    {
        if (!CheckForPlayer()) {
            Debug.Log("player not found");
            return null;
        }

        switch (itemId)
        {
            case 1: return new Axe(); // Replace with actual item creation logic
            case 2: return new LeatherArmor();
            case 3: return new LeatherHelmet();
            case 4: return new HealthPotion();
            default: return null;
        }
    }
    // Add the item to the player's inventory
    private void AddItemToInventory(Item item)
    {
        if (!CheckForPlayer()) {
            Debug.Log("player not found");
            return;
        }

        // Check if inventory has space (you may want to define this condition)
        for (int i = 0; i < playerStats.inventory.Length; i++)
        {
            if (playerStats.inventory[i] == null)
            {
                playerStats.inventory[i] = item; // Add the item to the inventory
                break; // Stop once the item is added
            }

        }

    }
}

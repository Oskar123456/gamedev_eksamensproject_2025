using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ShopManager : MonoBehaviour
{
    public int[,] ShopItems =new int [5,5];
    public float coins;
    public TextMeshProUGUI CoinsText;
    
    
    void Start()
    {
        CoinsText.text = "Coins: " + coins;
        //IDs
        ShopItems[1, 1] = 1;
        ShopItems[1, 2] = 2;
        ShopItems[1, 3] = 3;
        ShopItems[1, 4] = 4;

        //price
        //make it so the price is random inbetween a price range
        ShopItems[2, 1] = Random.Range(15, 55);
        ShopItems[2, 2] = Random.Range(35, 70);;
        ShopItems[2, 3] = Random.Range(25, 90);;
        ShopItems[2, 4] = Random.Range(45, 100);;
        //Quantity
        ShopItems[3, 1] = 0;
        ShopItems[3, 2] = 0;
        ShopItems[3, 3] = 0;
        ShopItems[3, 4] = 0;
        
    }

   public void Buy()
   {
       GameObject ButtonRef = GameObject.FindGameObjectWithTag("Event").GetComponent<EventSystem>().currentSelectedGameObject;

       if (coins >=ShopItems[2, ButtonRef.GetComponent<ButtonInfo>().ItemId])
       {
           coins -= ShopItems[2, ButtonRef.GetComponent<ButtonInfo>().ItemId];
           ShopItems[3, ButtonRef.GetComponent<ButtonInfo>().ItemId]++;
           CoinsText.text = "Coins: " + coins;
            ButtonRef.GetComponent<ButtonInfo>().QuantityTxt.text=ShopItems[3,ButtonRef.GetComponent<ButtonInfo>().ItemId].ToString();
       }
   }
  
}

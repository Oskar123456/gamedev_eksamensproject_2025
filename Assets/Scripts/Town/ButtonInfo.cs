using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInfo : MonoBehaviour
{
    public int ItemId;
    public TextMeshProUGUI  PriceTxt;
    public TextMeshProUGUI  QuantityTxt;
    public GameObject Shopmanager;

    void Update()
    {
        PriceTxt.text = "Price: $" + Shopmanager.GetComponent<ShopManager>().ShopItems[2, ItemId].ToString();
        QuantityTxt.text =  Shopmanager.GetComponent<ShopManager>().ShopItems[3, ItemId].ToString();
    }
}

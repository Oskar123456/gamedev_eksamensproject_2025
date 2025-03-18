using UnityEngine;


public class NPCInteractable : MonoBehaviour
{
   private Animator animator;
   public Transform chatBubblePrefab;
   public GameObject ShopCanvas;
   public GameObject uiCanvas;
   public Camera mainCamera; // Reference to the main game camera
   public Camera shopCamera; // Reference to the shop camera


   public void Awake()
   {
     // animator = GetComponent<Animator>();

   }
   public void Interact()
   {
      // ChatBubble.Create(transform.transform,new Vector3(-1.5f,1.7f,-1.5f),"Vendor!",chatBubblePrefab);
      //animator.SetTrigger("talk");
      ShopCanvas.SetActive(true);
      uiCanvas.SetActive(false);
      // Switch to the shop camera
      mainCamera.gameObject.SetActive(false);
      shopCamera.gameObject.SetActive(true);
   }

   public void CloseShop()
   {
      // Disable the shop UI
      ShopCanvas.SetActive(false);
      uiCanvas.SetActive(true);

      // Switch back to the main camera
      shopCamera.gameObject.SetActive(false);
      mainCamera.gameObject.SetActive(true);
   }
}

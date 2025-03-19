using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class ChatBubble : MonoBehaviour
{
    public static void Create(Transform parent,Vector3 localPosition, string text,Transform chatBubblePrefab) {
        Transform chatbubbleTransform =Instantiate(chatBubblePrefab, parent);
        // Enable the chat bubble GameObject (it might be disabled in the prefab)
        chatbubbleTransform.gameObject.SetActive(true);
        chatbubbleTransform.localScale *= 0.25f;

        // Set the local position of the chat bubble (on top of the NPC)
        chatbubbleTransform.localPosition = localPosition;

        // Rotate the chat bubble by 25 degrees around the Z-axis (optional)
        chatbubbleTransform.localRotation = Quaternion.Euler(40, 180, 25);

        // Get the ChatBubble component and call Setup
        chatbubbleTransform.GetComponent<ChatBubble>().Setup(text);
    }
    private TextMeshPro textRenderer;
    private Camera mainCamera;
    private void Awake()
    {
        textRenderer = transform.Find("Text").GetComponent<TextMeshPro>();

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (mainCamera == null) {
            mainCamera = Camera.main;
            if (mainCamera == null) {
                return;
            }
        }
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
    }

    private void Start()
    {

    }

    private void Setup(string text)
    {


        // Make the text bigger
        textRenderer.fontSize = 6; // Adjust this value as needed

        // Make the text green
        textRenderer.color = Color.green;
        // Set the text
        textRenderer.SetText(text);

        // Update the mesh
        textRenderer.ForceMeshUpdate();


    }

}

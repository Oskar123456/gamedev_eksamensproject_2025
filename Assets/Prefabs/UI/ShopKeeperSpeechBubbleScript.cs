using UnityEngine;
using UnityEngine.UI;

public class ShopKeeperSpeechBubbleScript : MonoBehaviour
{
    Camera main_cam;
    Transform main_cam_trf;
    Transform trf;

    NPCInteractable npc_script;

    void Start()
    {
        trf = GetComponent<Transform>();
        npc_script = GameObject.Find("ShopKeeper").GetComponent<NPCInteractable>();
        Button b = transform.GetChild(0).GetComponent<Button>();
        b.onClick.AddListener(() => npc_script.Interact());
    }

    void Update()
    {
        if (main_cam == null || main_cam_trf == null) {
            main_cam = Camera.main;
            if (main_cam == null || !main_cam.isActiveAndEnabled) {
                return;
            }
            main_cam_trf = main_cam.GetComponent<Transform>();
        }

        Vector3 diff_vec = main_cam_trf.position - trf.position;
        diff_vec.y = 0;
        trf.rotation = Quaternion.LookRotation(diff_vec, Vector3.up);
    }
}

using UnityEngine;

public class PortalExitScript : MonoBehaviour
{
    GameObject game_controller;
    GameObject player;
    Transform player_trf;

    void Start()
    {
        game_controller = GameObject.Find("GameController");
        player = GameObject.Find("Player");
        player_trf = player.GetComponent<Transform>();
    }

    void Update()
    {
        if (player_trf.hasChanged) {
            transform.rotation = Quaternion.FromToRotation(Vector3.forward, transform.position - player_trf.position);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player")) {
            game_controller.SendMessage("OnPortal", Portal.Exit);
        }
    }
}

public enum Portal { Exit, Entrance }

using UnityEngine;
using UnityEngine.SceneManagement;

public class KillZoneScript : MonoBehaviour
{
    GameObject game_controller;

    void Start()
    {
        game_controller = GameObject.Find("GameController");
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player")) {
            game_controller.SendMessage("OnDeath");
        }
    }
}
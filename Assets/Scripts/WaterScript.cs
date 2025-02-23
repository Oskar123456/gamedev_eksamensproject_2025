using UnityEngine;
using UnityEngine.SceneManagement;

public class WaterScript : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.CompareTag("Player")) {
            SceneManager.LoadScene("Town");
        }
    }
}

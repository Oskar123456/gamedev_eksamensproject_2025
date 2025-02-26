using UnityEngine;

public class AudioDummyScript : MonoBehaviour
{
    public float ttl;

    void Start()
    {
        Destroy(gameObject, ttl);
    }

    void Update()
    {

    }
}

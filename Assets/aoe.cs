using UnityEngine;

public class aoe : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnHit(AttackInfo ai)
    {
        AttackStats attack_stats = ai.attack_stats;
        Debug.Log("enemy attacked at " + transform.position.ToString() + " for " + attack_stats.damage + " damage");
    }
}

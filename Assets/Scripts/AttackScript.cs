/*
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 * */

using System;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    AttackStats attack_stats;
    GameObject attacker;

    public void SetStats(AttackStats ats) { attack_stats = ats; }
    public AttackStats GetStats() { return attack_stats; }

    public void SetAttacker(GameObject a) { attacker = a; }
    public GameObject GetAttacker() { return attacker; }

    void Awake() { }
    void Start() { }
    void Update() { }

    // void OnTriggerEnter(Collider collider)
    // {
        // if (collider.gameObject == attacker) {
            // Debug.Log("collision with own attacker");
            // return;
        // }
        // collider.SendMessage("OnHit", new AttackInfo(attack_stats), SendMessageOptions.DontRequireReceiver);
    // }
}

public class AttackInfo
{
    public AttackStats attack_stats;

    public AttackInfo(AttackStats ast)
    {
        attack_stats = ast;
    }
}

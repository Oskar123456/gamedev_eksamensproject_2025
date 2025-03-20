using UnityEngine;

public class BossStats : MonoBehaviour
{
    public int hp;
    public int hp_max;
    public float attack_range;
    public float attack_speed;
    public float attack_cooldown;
    public int attack_damage;
    public int collision_damage;
    public float stun_lock;

    public int xp;

    void Awake()
    {
        // Initialiser bossens unikke stats
        hp_max = 20 + GameState.level * 20 ;  // Bossens maksimale HP
        hp = hp_max;
        attack_range = 4.0f;  // Boss kan angribe fra længere afstand
        attack_speed = 1.5f;
        attack_cooldown = 3.0f; // Længere cooldown end almindelige fjender
        attack_damage = 30; // Højere skade
        collision_damage = 20;
        stun_lock = 0.5f; // Hvor lang tid bossen er "stunned" når den rammes
        xp = 100;
        Debug.Log("🔥 Boss stats initialized!");
    }



}




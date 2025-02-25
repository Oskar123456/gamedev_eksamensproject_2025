using UnityEngine;

public class Buff : MonoBehaviour
{
    GameObject player;
    PlayerStats player_stats;
    AttackStats player_attack_stats;

    public float duration;
    public bool makes_invulnerable;

    float created_t;

    void Start()
    {
        created_t = Time.time;

        player = GameObject.Find("Player");
        player_stats = player.GetComponent<PlayerStats>();
        player_attack_stats = player.GetComponent<AttackStats>();

        if (makes_invulnerable)
            player_stats.invulnerable = true;
    }

    void Update()
    {
        if (Time.time - created_t > duration) {
            Destroy(gameObject);
            if (makes_invulnerable)
                player_stats.invulnerable = false;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag != "Enemy") {
            return;
        }

        Vector3 normal = Vector3.Normalize(collider.transform.position - transform.position) * 0.4f;

        collider.gameObject.SendMessage("OnPush", normal);
    }
}

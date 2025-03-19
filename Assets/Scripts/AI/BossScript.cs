using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Attacks;
using UnityEngine.UI;
using Loot;
using Spells;


// Script til at styre bossens adfærd
public class BossScript : MonoBehaviour
{
    private Animator animator; // Animator til at styre animationer
    private Transform player_trf; // Referencer til spillerens transform

    private Attack bossAttack;  // Bossens meleeAttack

    private Spell bossAttackAoe;  // Bossens AOE (Area of Effect) angreb

    public float moveSpeed = 6f;  // Gå-hastighed for bossen
    public float stopDistance = 5f;  // Stopper ved denne afstand
    public float attackCooldown = 2.5f;

    private float attackTime = 2.167f;

    private float attackTimeLeft = 0f;

    private float spellBossCooldown = 5f;

    private float spellBossCoolDownleft;

    private float spellCastime = 2.167f;

    private float spellCastimeLeft;

    private NavMeshAgent nma;

    private BossStats stats;

    public GameObject arm_attack_spell;
    public GameObject spine_attack_spell;

    public GameObject healthbar_prefab;
    GameObject healthbar;
    Slider healthbar_slider;
    Transform player_cam_trf;
    public GameObject portal_exit_prefab;

    private float hitTimeDurationLeft = 0f; // Timer to track the hit animation duration
    private float hitDuration = 1f; // Duration for which the hit animation should play


    void Start()
    {
         // Initialisering af angreb
        bossAttack = new MeleeChargeBossAttack();
        bossAttack.duration = 1f;
        bossAttack.scale = 1f;
        bossAttack.damage = 1;


        // Initialisering af meteor spell
        bossAttackAoe = new MeteorSpell();
        bossAttack.duration = 1f;
        bossAttack.scale = 1f;
        bossAttack.damage = 1;

        // Finder nødvendige komponenter
        animator = GetComponent<Animator>();
        nma = GetComponent<NavMeshAgent>();
        stats = GetComponent<BossStats>();

        player_cam_trf = GameObject.Find("Main Camera").GetComponent<Transform>();

        // Opretter Healthbar
            healthbar = Instantiate(healthbar_prefab, transform.position + Vector3.up * 5, Quaternion.identity, transform);
            healthbar_slider = healthbar.transform.GetChild(0).gameObject.GetComponent<Slider>();
            Transform upSlider = healthbar_slider.GetComponent<Transform>();
            upSlider.position += Vector3.up * 5;

            healthbar.SetActive(false);


        // Sætte bossens gå hastighed til moveSpeed
        nma.speed = moveSpeed;


    }

    void Update()
    {
         if(player_trf == null) {
            player_trf = GameObject.FindWithTag("Player")?.transform;
              Debug.Log(player_trf.position);
        }

        if (player_trf.hasChanged) {
              if (stats.hp < stats.hp_max) {
                 FixHealthBar();
              }
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player_trf.position);

        if(spellBossCoolDownleft <= 0 && distanceToPlayer >= 20 && attackTimeLeft <= 0 && spellCastimeLeft <= 0 && hitTimeDurationLeft <= 0  ) {
            nma.ResetPath();
            PerformMeteorSpellAttack();
        }
        else if(attackTimeLeft <= 0 && spellCastimeLeft <= 0 && hitTimeDurationLeft <= 0) {
           if (distanceToPlayer > stopDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {

         nma.ResetPath();
         PerformAttack();


        }
        } else {

        }
        attackTimeLeft -= Time.deltaTime;
        spellCastimeLeft -= Time.deltaTime;
        spellBossCoolDownleft -= Time.deltaTime;

        ChooseAnimation();
    }

     void ChooseAnimation()
{
    if (hitTimeDurationLeft > 0)  // Prioritize "gethit1" a timer that tells us if the boss is still in the hit animation state.
    {
        animator.Play("gethit1");
        hitTimeDurationLeft -= Time.deltaTime; // Count down hit time
        return; // Prevents any other animations from playing
    }
    else if (attackTimeLeft > 0)
    {
        animator.Play("attack1");
    }
    else if (spellCastimeLeft > 0) {

     animator.Play("attack1LSpike");


    }
    else if (nma.velocity.magnitude > 0)
    {
        animator.Play("walk2");
    }
    else
    {
        animator.Play("idle1");
    }
}


    void MoveTowardsPlayer()
    {
        if (player_trf == null) return;

        Vector3 direction = (player_trf.position - transform.position).normalized;
        direction.y = 0;  // Undgå at bossen flyver op eller ned

        nma.SetDestination(player_trf.position);


    }

    void OnHit(HitInfo hit_info)
{
    if (hit_info.parent.tag != "Player")
        return;

    // ✅ Add cooldown to prevent spam hits from Blizzard
    if (hitTimeDurationLeft > 0)
        return;

    TakeDamage(hit_info.damage);
    healthbar_slider.value = (float)stats.hp / stats.hp_max;
    healthbar.SetActive(true);

    if (nma.enabled)
        nma.ResetPath();

    // ✅ Apply hit animation cooldown to avoid being stuck
    hitTimeDurationLeft = hitDuration;

    if (stats.hp < 1) {
        hit_info.parent.SendMessage("OnRecXp", stats.xp, SendMessageOptions.DontRequireReceiver);
        Destroy(gameObject);
    }
}



    //Funktion til bossens meleeAttack
     void PerformAttack()
    {
       // Sikrer, at bossen ikke kan meleeAttack igen, mens han er ved at angribe.
        attackTimeLeft = attackTime;


       // Finder højre hånd og venstre hånd
       Transform rightHand = transform.Find("Character1_Ctrl_Reference/Character1_Ctrl_Hips/Character1_Ctrl_Spine/Character1_Ctrl_Spine1/Character1_Ctrl_Spine2/Character1_Ctrl_RightShoulder/Character1_Ctrl_RightArm/Character1_Ctrl_RightForeArm/Character1_Ctrl_RightHand");
       Transform leftHand = transform.Find("Character1_Ctrl_Reference/Character1_Ctrl_Hips/Character1_Ctrl_Spine/Character1_Ctrl_Spine1/Character1_Ctrl_Spine2/Character1_Ctrl_LeftShoulder/Character1_Ctrl_LeftArm/Character1_Ctrl_LeftForeArm/Character1_Ctrl_LeftHand");

       // Instantiere hændernes position og sætter spells effects på
       GameObject instance = Instantiate(GameData.attack_prefabs[bossAttack.prefab_index],
                    rightHand.position, rightHand.rotation, rightHand);


       GameObject instance1 = Instantiate(GameData.attack_prefabs[bossAttack.prefab_index],
                    leftHand.position, leftHand.rotation, leftHand);

       // Tildeler attack stats til hænderne
            AttackStats attack_statsRight = instance.GetComponent<AttackStats>();
            AttackStats attack_statsLeft = instance1.GetComponent<AttackStats>();

            attack_statsRight.damage = 10;
            attack_statsRight.scale = 1;
            attack_statsRight.duration = 2.167f;
            attack_statsRight.base_duration = 2.167f;
            attack_statsRight.damage_type = DamageType.Normal;
            attack_statsRight.attacker = gameObject;

            attack_statsLeft.damage = 1;
            attack_statsLeft.scale = 1;
            attack_statsLeft.duration = 2.167f;
            attack_statsLeft.base_duration = 2.167f;
            attack_statsLeft.damage_type = DamageType.Normal;
            attack_statsLeft.attacker = gameObject;

        }

    //Funktion til bossens MeteorSpell
    void PerformMeteorSpellAttack() {

             // Sikrer, at bossen ikke kan angribe igen, mens den er ved at kaste sin spell.
             spellCastimeLeft = spellCastime;

             // Sikrer, at bossen ikke kan bruge meteor-spellet igen, før cooldown er færdig.
             spellBossCoolDownleft = spellBossCooldown;


            // Skaber et nyt Meteor-spell objekt baseret på en prefab fra GameData.spell_prefabs.
             GameObject instanceMeteor = Instantiate(GameData.spell_prefabs[bossAttackAoe.prefab_index],
                    player_trf.position, player_trf.rotation);

                    // Juster størrelsen af Meteor-spellet
                    instanceMeteor.transform.localScale = new Vector3(instanceMeteor.transform.localScale.x * 5, instanceMeteor.transform.localScale.y, instanceMeteor.transform.localScale.z * 5);


            // Tildeler spell stats til hænderne
            SpellStats spell_stats = instanceMeteor.GetComponent<SpellStats>();
            spell_stats.damage = 10;
            spell_stats.scale = 5;
            spell_stats.duration = 4;
            spell_stats.base_duration = 4;
            spell_stats.damage_type = DamageType.Fire;
            spell_stats.caster = gameObject;

    }


    //Funktion sørger for, at bossens healthbar altid vender mod kameraet, så spilleren ser den.
     void FixHealthBar()
        {
            Vector3 p1 = player_cam_trf.position;
            Vector3 p2 = healthbar.transform.position;
            p1.y = 0; p2.y = 0;
            healthbar.transform.rotation = Quaternion.FromToRotation(Vector3.forward, p2 - p1);
        }



    // Funktion til når bossen tager skade
      public void TakeDamage(int damage)
    {
        stats.hp -= damage;
        Debug.Log($"🔥 Boss took {damage} damage! HP: {stats.hp}/{stats.hp_max}");

        attackTimeLeft = 0f;
        hitTimeDurationLeft = hitDuration; // Sætter hit animation timer


        if (stats.hp <= 0)
        {
            Die();
        }
    }


    // Funktion til at håndtere bossens død
    private void Die()
    {
        Debug.Log("💀 Boss is dead!");
        GameObject portal_exit = Instantiate(portal_exit_prefab, transform.position, Quaternion.identity);
        portal_exit.transform.position += Vector3.up * 3.5f;
        Destroy(gameObject); // Fjern bossen fra spillet

         //  Tilføjer 50% chance for at droppe loot
    if (Random.value < 0.5f)  // 50% chance (0.0 - 0.5)
    {
        Debug.Log("🎁 Boss dropped loot!");
        Item item_dropped = GameData.GenerateBossLoot();
        item_dropped.Drop(transform.position);
    }
    else
    {
        Debug.Log("❌ No loot dropped this time.");
    }

    // Fjern bossen fra spillet
    Destroy(gameObject);
 }
}

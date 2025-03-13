using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using Attacks;
using UnityEngine.UI;
using Loot;


public class BossScript : MonoBehaviour
{
    private Animator animator;
    private Transform player_trf;

    private Attack bossAttack;    

    public float moveSpeed = 6f;  // Gå-hastighed
    public float stopDistance = 5f;  // Stop ved denne afstand
    public float attackCooldown = 2.5f;

    private float attackTime = 2.167f;

    private float attackTimeLift = 0f;

    private NavMeshAgent nma;

    private BossStats stats;

    public GameObject arm_attack_spell;
    public GameObject spine_attack_spell;

    public GameObject healthbar_prefab;
    GameObject healthbar;
    Slider healthbar_slider;
    Transform player_cam_trf;
    public GameObject portal_exit_prefab;
    
    private float hitTimeLift = 0f; // Timer to track the hit animation duration 
    private float hitDuration = 1f; // Duration for which the hit animation should play


    void Start()
    {
        bossAttack = new MeleeChargeBossAttack(); 
        bossAttack.duration = 1f;
        bossAttack.scale = 1f;
        bossAttack.damage = 1;
        

        animator = GetComponent<Animator>();
        nma = GetComponent<NavMeshAgent>();
        stats = GetComponent<BossStats>();
         
        player_cam_trf = GameObject.Find("Main Camera").GetComponent<Transform>();

            healthbar = Instantiate(healthbar_prefab, transform.position + Vector3.up * 5, Quaternion.identity, transform);
            healthbar_slider = healthbar.transform.GetChild(0).gameObject.GetComponent<Slider>();
            Transform upSlider = healthbar_slider.GetComponent<Transform>();
            upSlider.position += Vector3.up * 5;
             
            healthbar.SetActive(false);
         


        nma.speed = moveSpeed; 

        player_trf = GameObject.FindWithTag("Player")?.transform;
        if (player_trf == null)
        {
            Debug.LogError("❌ Player transform not found!");
        }

        if (animator == null)
            Debug.LogError("❌ Animator component is missing on Boss!");

        
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
        if(attackTimeLift <= 0) {
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
            attackTimeLift -= Time.deltaTime;
        }
       

        ChooseAnimation();
    }
     
     void ChooseAnimation()
{
    if (hitTimeLift > 0)  // Prioritize "gethit1" a timer that tells us if the boss is still in the hit animation state.
    { 
        animator.Play("gethit1");
        hitTimeLift -= Time.deltaTime; // Count down hit time
        return; // Prevents any other animations from playing
    }
    else if (attackTimeLift > 0) 
    {
        animator.Play("attack1");
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

            TakeDamage(hit_info.damage);
            healthbar_slider.value = (float)stats.hp / stats.hp_max;
            healthbar.SetActive(true);
            
            /* TODO: refactor as "pushback()" or something */
            if (nma.enabled)
                nma.ResetPath();

            if (stats.hp < 1) {
                hit_info.parent.SendMessage("OnRecXp", stats.xp, SendMessageOptions.DontRequireReceiver);
                // death_effect = Instantiate(death_effect_prefab, transform.position + halfway_up_vec, Quaternion.identity);
               //death_effect.transform.localScale = death_effect.transform.localScale * death_effect_scale;
               // Destroy(death_effect, death_effect_delete_t);
                Destroy(gameObject);
                return;
            }
        }

/*   
    void PerformAttack()
{
    if (player_trf == null) return;

    float attackDistance = 30f; // Boss skal angribe, hvis spilleren er under 30f væk
    float projectileSpeed = 25f; // Juster hastigheden efter behov

    float distanceToPlayer = Vector3.Distance(transform.position, player_trf.position);
    if (distanceToPlayer > attackDistance) return; // Stop hvis spilleren er for langt væk

    attackTimeLift = attackTime; 
    isAttacking = true;


    // Find hænderne
    Transform rightHand = transform.Find("Character1_Ctrl_Reference/Character1_Ctrl_Hips/Character1_Ctrl_Spine/Character1_Ctrl_Spine1/Character1_Ctrl_Spine2/Character1_Ctrl_RightShoulder/Character1_Ctrl_RightArm/Character1_Ctrl_RightForeArm/Character1_Ctrl_RightHand");
    Transform leftHand = transform.Find("Character1_Ctrl_Reference/Character1_Ctrl_Hips/Character1_Ctrl_Spine/Character1_Ctrl_Spine1/Character1_Ctrl_Spine2/Character1_Ctrl_LeftShoulder/Character1_Ctrl_LeftArm/Character1_Ctrl_LeftForeArm/Character1_Ctrl_LeftHand");            

    if (rightHand == null || leftHand == null) return;

    // Beregn retning fra hver hånd mod spilleren
    Vector3 directionRight = (player_trf.position - rightHand.position).normalized;
    Vector3 directionLeft = (player_trf.position - leftHand.position).normalized;



    // Instansier projektiler fra hænderne
    GameObject projectileRight = Instantiate(GameData.attack_prefabs[bossAttack.prefab_index], 
                                             rightHand.position, Quaternion.identity);
    GameObject projectileLeft = Instantiate(GameData.attack_prefabs[bossAttack.prefab_index], 
                                            leftHand.position, Quaternion.identity);

    // Sørg for, at projektilerne har en Rigidbody og ikke er børn af bossen
    Rigidbody rbRight = projectileRight.GetComponent<Rigidbody>();
    Rigidbody rbLeft = projectileLeft.GetComponent<Rigidbody>();

    projectileRight.transform.parent = null; // Fjern som child af bossen
    projectileLeft.transform.parent = null;

    // Retter rotationen så projektilerne peger på spilleren
    projectileRight.transform.LookAt(player_trf.position);
    projectileLeft.transform.LookAt(player_trf.position);

    if (rbRight != null)
    {
        rbRight.useGravity = false; // Undgå at det falder ned
        rbRight.linearVelocity = directionRight * projectileSpeed;
    }

    if (rbLeft != null)
    {
        rbLeft.useGravity = false;
        rbLeft.linearVelocity = directionLeft * projectileSpeed;
    }

    // Tilføj angrebsstatistikker
    ApplyAttackStats(projectileRight);
    ApplyAttackStats(projectileLeft);

    isAttacking = false;
    nextAttackTime = Time.time + attackCooldown;
}


// Metode til at tilføje angrebsstatistikker til projektiler
void ApplyAttackStats(GameObject projectile)
{
    if (projectile == null) return;

    AttackStats attackStats = projectile.GetComponent<AttackStats>();
    if (attackStats != null)
    {
        attackStats.damage = bossAttack.damage;
        attackStats.scale = bossAttack.scale;
        attackStats.duration = bossAttack.duration;
        attackStats.base_duration = bossAttack.duration;
        attackStats.damage_type = DamageType.Normal;
        attackStats.attacker = gameObject; 
    }
}

*/   
    
    




     void PerformAttack()
    {
        attackTimeLift = attackTime; 
       
       
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
            
            attack_statsRight.damage = 1;
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


     void FixHealthBar()
        {
            Vector3 p1 = player_cam_trf.position;
            Vector3 p2 = healthbar.transform.position;
            p1.y = 0; p2.y = 0;
            healthbar.transform.rotation = Quaternion.FromToRotation(Vector3.forward, p2 - p1);
        }



      public void TakeDamage(int damage)
    {
        stats.hp -= damage;
        Debug.Log($"🔥 Boss took {damage} damage! HP: {stats.hp}/{stats.hp_max}");
       
        attackTimeLift = 0f; 
        hitTimeLift = hitDuration; // Setting hit animation timer
       

        if (stats.hp <= 0)
        {
            Die();
        }
    }



    private void Die()
    {
        Debug.Log("💀 Boss is dead!");
        GameObject portal_exit = Instantiate(portal_exit_prefab, transform.position, Quaternion.identity);
        portal_exit.transform.position += Vector3.up * 3.5f;
        Destroy(gameObject); // Fjern bossen fra spillet
 
        Item item_dropped = GameData.GenerateBossLoot();
                    item_dropped.Drop(transform.position);
    }

}  
  

    
    


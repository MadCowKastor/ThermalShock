using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, Attackable
{
    [Header("Enemy Health and Heat")]
    [Tooltip("The heat the enemy starts at when spawned.")]
    public float baseHeat = 0f;
    [Tooltip("The heat level at which the enemy will die.")]
    public float deathHeat = 0f;
    [Tooltip("The direction this enemy will die at. True means that if the enemy's current heat value is below the death heat, it will die. False means that if the enemy's heat is above the death heat value, then it will die. ")]
    public bool chilledDeath = false;
    [Space]
    [Tooltip("The enemy's current heat level. Changed in game. Automatically set to the base heat on start")]
    public float heatLevel = 0f;
    public float health = 100f;
    public float normalDamageMult = 1f;
    public float heatShockDamageMult = 0.01f;

    [Header("Damage")]
    [Tooltip("The damage this unit does in a melee attack")]
    public float meleeDamage;
    public float meleeHeatDamage;
    [Tooltip("Damage the ranged projectile will do if it hits.")]
    public float rangedDamage;

    [Tooltip("The prefab of the enemy's ranged attack.")]
    public GameObject rangedProjectilePrefab;

    [Tooltip("The prefab of the projectile spawned by ranged attacks.")]
    public GameObject rangedProjectile;
    [Tooltip("How fast the projectile moves")]
    public float rangedProjectileFlySpeed;
    [Tooltip("How long the projectile lasts for before it is automatically destroyed. Cleanup value.")]
    public float rangedProjectileLifetime;
    [Tooltip("Object that is enabled duing melee attacks.")]
    public GameObject meleeCollisionObject;

    [Header("AI control settings")]
    [Tooltip("AI control switch. Will hang back and shoot at the player (if line of sight avaliable). Will still do a melee attack if too close.")]
    public bool preferRangedAttack;
    [Tooltip("The range that the enemy will stop and try to shoot.")]
    public float engagementRange;
    [Tooltip("How close the enemy has to be before it will try to do a melee attack")]
    public float meleeStrikeRange;

    [Header("Attack Timers")]
    [Tooltip("The amount of time between wanting to attack, and actually launching the attack.")]
    public float meleeWindupTime;
    [Tooltip("Time that the enemy is dangerous to touch. I think having a small window is better than having them constantly damaging, as the player needs to get close to do thier own melee attacks.")]
    public float meleeDangerTime;
    [Tooltip("How long after the melee attack that this enemy will be unable to do another attack, of any kind.")]
    public float meleeCooldownTime;

    [Tooltip("The amount of time between wanting to attack, and actually launching the attack.")]
    public float rangedWindupTime;
    [Tooltip("How long after a ranged attack that this enemy will be unable to do another attack, of any kind.")]
    public float rangedCooldownTime;



    [Header("Movement")]
    [Tooltip("Movement speed (in meters per second) of the Enemy.")]
    public float moveSpeed = 0f;
    [Tooltip("Acceleration. How fast the enemy gets up to max speed, or slows down")]
    public float acceleration;
    [Tooltip("How quickly the enemy can turn. Low values make this enemy easier to dodge.")]
    public float turnSpeed;


    [Header("internal stuff")]
    [Tooltip("Is this enemy commited to preforming, or is currently attacking? This will switch to true, preventing other attacks. Occurs before the attack is seen, during the start of the windup phase.")]
    public bool commitedToRanged;
    [Tooltip("The same as above, but for melee attacks.")]
    public bool commitedToMelee;

    public Vector3 vectorToPlayer;
    public CharacterController charControl;
    public PlayerController playerCon;
    public NavMeshAgent navAgent;


    public void Hit(float heat, float damage)
    {
        //Take heat damage, and then check if dead.
        health -= damage * (normalDamageMult + (Mathf.Abs(heatLevel - heat) * heatShockDamageMult));
        //heatLevel += heat;
        AmIDead();
    }

    public float MeleeHit(float heat, float damage)
    {

        //Same as Hit but returns targets heat before hit.
        var preHeat = heatLevel;
        Hit(heat, damage);
        return preHeat;
    }

    // Start is called before the first frame update
    void Start()
    {
        meleeCollisionObject.GetComponent<EnemyMelee>().damage = meleeDamage;
        meleeCollisionObject.GetComponent<EnemyMelee>().heatDamage = meleeHeatDamage;


        navAgent = gameObject.GetComponent<NavMeshAgent>();
        navAgent.speed = moveSpeed;

        playerCon = FindObjectOfType<PlayerController>();
        charControl = gameObject.GetComponent<CharacterController>();
        //Setting the current heat level to the starting level.
        heatLevel = baseHeat;
    }

    // Update is called once per frame
    void Update()
    {
        vectorToPlayer = playerCon.transform.position - gameObject.transform.position;
        vectorToPlayer.Normalize();
        EnemyAIControl();
        DoAttack();
    }

    void Movement()
    {

        //charControl.Move(vectorToPlayer * moveSpeed * Time.deltaTime);
    }

    //Death checks and destruction of the object.
    void AmIDead()
    {
        if (chilledDeath)
        {
            if (heatLevel < deathHeat)
            {
                Debug.Log(gameObject.name + " died from natural causes. (Too cold) ");
                Destroy(gameObject);
            }
        }
        else
        {
            if (heatLevel > deathHeat)
            {
                Debug.Log(gameObject.name + " died from natural causes. (Too hot) ");
                Destroy(gameObject);
            }
        }
        if (health <= 0)
        {
            Debug.Log(gameObject.name + " died from grievious harm. ");
            Destroy(gameObject);
        }
    }


    void EnemyAIControl()
    {
        float distanceToPlayer = Vector3.Distance(gameObject.transform.position, playerCon.transform.position);
        if (distanceToPlayer > meleeStrikeRange && preferRangedAttack)
        {
            if (distanceToPlayer > engagementRange)
            {
                navAgent.destination = playerCon.transform.position;
            }
            else
            {

                // Check if the enemy can see the player when in range. For corners and other barriers.
                Ray lineOfSight = new Ray(gameObject.transform.position, (playerCon.transform.position - gameObject.transform.position).normalized);
                RaycastHit rayHit;
                if (Physics.Raycast(lineOfSight, out rayHit, engagementRange + 1f))
                {
                    //If the enemy cant see the player, continue moving. The pathfinding should eventually move the enemy into sight.
                    if (rayHit.collider.gameObject.name != "Player") { navAgent.destination = playerCon.transform.position; }
                    else
                    {
                        if (!commitedToMelee) { commitedToRanged = true; }
                    }
                }
            }
        }
        else
        {
            navAgent.destination = playerCon.transform.position;
            if (meleeStrikeRange >= Vector3.Distance(gameObject.transform.position, playerCon.transform.position))
            {
                commitedToMelee = true;
            }

        }
    }

    private float attackClock;
    private int attackStage;

    void DoAttack()
    {
        // Are we commited to doing an attack?
        if (commitedToMelee || commitedToRanged)
        {

            if (commitedToMelee)
            {
                attackClock += Time.deltaTime;
                switch (attackStage)
                {
                    case 0: //The ready state. Runs once per attack
                        commitedToMelee = true;
                        attackStage = 1;
                        break;
                    case 1: //The pre attack windup time. 
                        if (attackClock >= meleeWindupTime) { attackStage = 2; } //TURN ON WEAPONS
                        break;
                    case 2: //The acutal phase where the player is in danger.
                        if (attackClock >= meleeWindupTime + meleeDangerTime) { attackStage = 3; } //TURN OFF WEAPONS
                        break;
                    case 3:
                        if (attackClock >= meleeWindupTime + meleeDangerTime + meleeCooldownTime)
                        {
                            attackClock = 0f;
                            attackStage = 0;
                            commitedToMelee = false;
                        }
                        break;
                    default:
                        break;
                }
            }
            //End of melee attack logic.

            if (commitedToRanged && !commitedToMelee)
            {
                attackClock += Time.deltaTime;
                switch (attackStage)
                {
                    case 0: //The ready state. Runs once per attack
                        commitedToRanged = true;
                        attackStage = 1;
                        break;
                    case 1: //The pre attack windup time. 
                        if (attackClock >= rangedWindupTime)
                        {
                            // Spawn Projectile.
                            EnemyProjectile projectileSpawned = Instantiate<GameObject>(rangedProjectile, gameObject.transform.position, Quaternion.LookRotation( vectorToPlayer, Vector3.up) ).GetComponent<EnemyProjectile>();
                            projectileSpawned.damage = rangedDamage;
                            projectileSpawned.flySpeed = rangedProjectileFlySpeed;
                            projectileSpawned.deathTime = rangedProjectileLifetime;

                            attackStage = 2;
                        }
                        break;
                    case 2: //Cooldown..
                        if (attackClock >= rangedWindupTime + rangedCooldownTime)
                        {
                            attackClock = 0f;
                            commitedToRanged = false;
                            attackStage = 0;
                        }
                        break;
                    default:
                        break;
                }

            }
        } // end of DoAttack()


    }
}

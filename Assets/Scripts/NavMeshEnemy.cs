using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshEnemy : MonoBehaviour , Attackable
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

    [Header("Movement")]
    [Tooltip("Movement speed (in meters per second) of the Enemy.")]
    public float moveSpeed = 0f;

    [Header("internal stuff")]
    [Tooltip("The root object, for deletion and movement driving.")]
    public Vector3 vectorToPlayer;
    public CharacterController charControl;
    public PlayerController playerCon;
    public NavMeshAgent navAgent;

    public void Hit(float heat, float damage)
    {
        navAgent = gameObject.GetComponent<NavMeshAgent>();
        navAgent.speed = moveSpeed;
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
        Movement();
    }

    void Movement()
    {
        navAgent.destination = playerCon.gameObject.transform.position;
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
        } else
        {
            if (heatLevel > deathHeat)
            {
                Debug.Log(gameObject.name + " died from natural causes. (Too hot) ");
                Destroy(gameObject);
            }
        }
        if(health <= 0)
        {
            Debug.Log(gameObject.name + " died from grievious harm. ");
            Destroy(gameObject);
        }
    }
}

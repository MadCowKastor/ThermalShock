using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour , Attackable
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

    [Header("Movement")]
    [Tooltip("Movement speed (in meters per second) of the Enemy.")]
    public float moveSpeed = 0f;

    [Header("internal stuff")]
    [Tooltip("The root object, for deletion and movement driving.")]
    public GameObject rootObject;
    public Vector3 vectorToPlayer;
    public CharacterController charControl;

    public void Hit(float heat, bool isMelee)
    {
        //Take heat damage, and then check if dead.
        heatLevel += heat;
        AmIDead();
    }

    public float MeleeHit(float heat)
    {
        //Same as Hit but returns targets heat before hit.
        var preHeat = heatLevel;
        Hit(heat, true);
        return preHeat;
    }

    // Start is called before the first frame update
    void Start()
    {
        charControl = gameObject.GetComponent<CharacterController>();
        //Setting the current heat level to the starting level.
        heatLevel = baseHeat;
    }

    // Update is called once per frame
    void Update()
    {
        vectorToPlayer = FindObjectOfType<PlayerController>().transform.position - gameObject.transform.position;
        vectorToPlayer.Normalize();
        Movement();
    }

    void Movement()
    {
        charControl.Move(vectorToPlayer * moveSpeed * Time.deltaTime);
    }

    //Death checks and destruction of the object.
    void AmIDead()
    {
        if (chilledDeath)
        {
            if (heatLevel < deathHeat)
            {
                Debug.Log(rootObject.name + " died from natural causes. (Too cold) ");
                Destroy(rootObject);
            }
        } else
        {
            if (heatLevel > deathHeat)
            {
                Debug.Log(rootObject.name + " died from natural causes. (Too hot) ");
                Destroy(rootObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("How fast the player moves. Units are meters per second.")]
    public float moveSpeed = 0f;

    [Header("Health")]
    [Tooltip("The current heat level of the player.")]
    public float heat;
    [Tooltip("The hottest the player can reach before dying.")]
    public float heatMax;
    [Tooltip("The coldest the player can reach before dying.")]
    public float heatMin;
    [Tooltip("How fast the player naturally loses heat.")]
    public float heatLossRate;
    [Tooltip("The heat the player naturally resets to")]
    public float ambiantHeat;
    [Tooltip("Currently not implemented, using heat system as health.")]
    public float health;

    [Header("Attack - Gun")]
    [Tooltip("Is the gun running through its attack cycle. This includes cooldown.")]
    public bool gunAttacking;
    [Tooltip("The amount of heat the player is changed by.")]
    public float gunHeatGenerated;
    [Tooltip("The amount of time before the gun fires (after pushing the fire button).")]
    public float gunAttackDelay;
    [Tooltip("The amount of time after the gun has fired before the gun can be fired again.")]
    public float gunAttackCooldown;
    [Tooltip("The linked prefab of the projectile to spawn.")]
    public GameObject projectilePrefab;
    [Tooltip("The amount of heat change the projectile does when it hits something.")]
    public float projectileHeatDamage;
    [Tooltip("How fast the projectile will move.")]
    public float projectileSpeed;

    private float gunClock;
    private int gunState;

    [Header("Attack - Sword")]
    [Tooltip("Is the sword currently ready to be used?")]
    public bool swordAttacking;
    [Tooltip("The amount of heat the player recives from swinging the sword.")]
    public float swordHeatGenerated;
    [Tooltip("The time after the attack button is first pressed before the sword swing/actual attack occurs.")]
    public float swordAttackDelay;
    [Tooltip("How long the sword hangs in the air to collide with things and impart heat to them.")]
    public float swordAttackCutTime;
    [Tooltip("The amount of time after a swing that must pass before you can start swinging again.")]
    public float swordAttackCooldown;
    [Tooltip("The sword gameObject that checks if something has been hit.")]
    public GameObject swordObject;
    [Tooltip("The amount heat is changed on a hit target with the sword.")]
    public float swordHeatDamage;

    private float swordClock;
    private int swordState;

    [Space]
    [Header("Internal Stuff")]
    public GameObject helperObject;
    public GameObject otherHelperObject;
    private CharacterController charControl;
    Vector3 pos;
    Quaternion rot;

    // Start is called before the first frame update
    void Start()
    {
        charControl = gameObject.GetComponent<CharacterController>();

        gunAttacking = false;
        gunState = 0;

        swordAttacking = false;
        swordState = 0;
    }

    // Update is called once per frame
    void Update()
    {
        HeatUpdate();
        (pos, rot) = GetDirection();

        if (Input.GetButtonDown("Fire1") || Input.GetButton("Fire1") ) { gunAttacking = true; }
        if (Input.GetButtonDown("Fire2") || Input.GetButton("Fire2")) { swordAttacking = true; }
        GetDirection();
        GunAttack();
        SwordAttack();
        Movement();
    }

    void HeatUpdate()
    {
        if((heat > heatMax) || (heat < heatMin)){
            //die
        }
        heat += (ambiantHeat - heat) * heatLossRate * Time.deltaTime;
    }

    // Check and do movement based on inputs.
    void Movement()
    {
        Vector3 moveInput = Vector2.zero;
        moveInput.z = Input.GetAxisRaw("Vertical");
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.Normalize();
        //Debug.Log("Movement direction magnitude is " + moveInput.magnitude);
        charControl.Move(moveInput * moveSpeed * Time.deltaTime);
    }

    // calculates the angle/ direction the mouse is from the player character, to be fed into attacks.
    (Vector3, Quaternion) GetDirection()
    {
        Plane aimPlane = new Plane(Vector3.up, Vector3.up);
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 mouseHitPoint = Vector3.zero;
        float rayHitDist;
        if (aimPlane.Raycast(mouseRay, out rayHitDist) )
        {
            mouseHitPoint = mouseRay.GetPoint(rayHitDist);
        }
        
        mouseHitPoint = gameObject.transform.InverseTransformPoint(mouseHitPoint);
        mouseHitPoint.y = 0f;
        mouseHitPoint.Normalize();
        mouseHitPoint = mouseHitPoint * 2f;
        Quaternion lookAt = Quaternion.LookRotation(mouseHitPoint, Vector3.up);

        otherHelperObject.transform.SetPositionAndRotation(otherHelperObject.transform.position, lookAt);
        helperObject.transform.SetPositionAndRotation(mouseHitPoint + gameObject.transform.position, helperObject.transform.rotation);

        return (mouseHitPoint, lookAt);
    }


    // gun attack logic
    void GunAttack() {
        if (gunAttacking)
        {
            gunClock += Time.deltaTime;
            //Debug.Log("Gun's clock is running. Gun is attacking.");
            switch (gunState)
            {
                case 0:
                    if (gunClock >= gunAttackDelay) { gunState = 1; }
                    //Debug.Log("Gun is in state 0, the pre attack state.");
                    break;

                case 1:
                    //Debug.Log("Gun is in state 1, the projectile spawn/attack state.");
                    if (heat + gunHeatGenerated <= 100){
                        SpawnProjectile();
                        heat += gunHeatGenerated;
                    }
                    gunState = 2;
                    break;

                case 2:
                    //Debug.Log("Gun is in state 2, the cooldown state.");
                    if (gunClock >= gunAttackDelay + gunAttackCooldown) {
                        //Debug.Log("Gun is at the end of state 2, state, clock and attacking bool are reset."); 
                        gunState = 0; gunClock = 0f; gunAttacking = false; 
                    }
                    break;

                default:
                    //Debug.Log("Gun is in error state. This should not be happening.");
                    break;
            }


        }
    }

    void SwordAttack()
    {
        if (swordAttacking)
        {
            swordClock += Time.deltaTime;
            switch (swordState)
            {
                case 0:
                    if (swordClock >= swordAttackDelay) { swordState = 1; }
                    break;

                case 1:
                    swordState = 2;
                    swordObject.SetActive(true);
                    break;
                case 2:
                    if (swordClock >= swordAttackDelay + swordAttackCutTime)
                    {
                        swordObject.SetActive(false);
                        swordState = 3;
                    }
                        break;
                case 3:
                    if (swordClock >= swordAttackDelay + swordAttackCutTime + swordAttackCooldown)
                    {
                        swordState = 0; swordClock = 0f; swordAttacking = false;
                    }
                    break;

                default:
                    break;
            }


        }
    }

    // code for the player to fire a projectile.
    void SpawnProjectile()
    {
        //get Direction of mouse/second stick and create a projectile prefab moving in that direction.
        PlayerProjectile projectile = Instantiate<GameObject>(projectilePrefab, gameObject.transform.position + pos, rot).GetComponent<PlayerProjectile>() ;
        projectile.projectileHeat = projectileHeatDamage;
        projectile.flySpeed = projectileSpeed;
    }


    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Player has collided with " + collision.gameObject.name);
    }

}

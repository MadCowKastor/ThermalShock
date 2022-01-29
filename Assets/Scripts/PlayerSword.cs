using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{
    public PlayerController playerCon;
    public float swordHeat = 0f;
    public float swordDamage = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Attackable hitObject = other.GetComponent<Attackable>();
        if (other.name != "Player" && hitObject != null) 
        { 
<<<<<<< HEAD
            float targetHeatLevel = hitObject.MeleeHit(swordHeat); 
=======
            float targetHeat = hitObject.MeleeHit(swordHeat,swordDamage); 
>>>>>>> origin/main
            //if target heat > player heat, heat up player, else cool player.
            // if my math is right, we dont need an if statement here. Adding a negitive number still results in subtraction.
            // changes the players heat level by 80% of the difference between the players and targets heat level.
            playerCon.heat += 0.8f * (targetHeatLevel - playerCon.heat);
        }
    }


}

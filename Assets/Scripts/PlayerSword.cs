using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{

    public float swordHeat = 0f;
    public float swordDamage = 0f;
    public PlayerController playerCon;
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
        swordDamage = playerCon.swordDamage;
        swordHeat = playerCon.swordHeatGenerated;

        Attackable hitObject = other.GetComponent<Attackable>();
        if (other.tag == "Enemy" && hitObject != null) 
        { 
            float targetHeat = hitObject.MeleeHit(swordHeat,swordDamage);

            //if target heat > player heat, heat up player, else cool player.
            float heatDiff = targetHeat - playerCon.heat;
            heatDiff *= playerCon.absorbtionPercentage;
            playerCon.heat += heatDiff;
        }
    }


}

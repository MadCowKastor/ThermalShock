using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSword : MonoBehaviour
{

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
            float targetHeat = hitObject.MeleeHit(swordHeat,swordDamage); 
            //if target heat > player heat, heat up player, else cool player.
        }
    }


}

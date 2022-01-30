using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyMelee : MonoBehaviour
{
    public float damage, heatDamage;
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
        if(other.name == "Player")
        {
            PlayerController playerCon = other.GetComponent<PlayerController>();
            playerCon.health -= damage;
            playerCon.heat += heatDamage;
            gameObject.SetActive(false);
        }
    }
}

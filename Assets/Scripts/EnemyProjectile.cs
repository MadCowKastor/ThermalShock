using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{

    public float damage;
    public float heat;
    public float flySpeed;

    float deathClock;
    public float deathTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Translate(Vector3.forward * flySpeed * Time.deltaTime);
        deathClock += Time.deltaTime;
        if (deathClock >= deathTime) { Destroy(gameObject); }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player")
        {
            other.GetComponent<PlayerController>().TakeDamage(damage);
            Debug.Log("I, " + gameObject.name + ", hit the player: " + other.gameObject.name);
            Destroy(gameObject);
        }
    }

}

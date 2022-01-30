using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject spawnedPrefab;
    public int spawnsRemaining;
    public float spawnCooldown;
    public float spawnRange;
    private float spawnClock = 0f;
    
    public PlayerController playerCon;
    
    // Start is called before the first frame update
    void Start()
    {
        playerCon = FindObjectOfType<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {
        if (spawnClock < spawnCooldown) { spawnClock += Time.deltaTime; }
        else
        {
            if ((playerCon.transform.position - gameObject.transform.position).magnitude > spawnRange)
            {
                spawnClock = 0f;
                Instantiate<GameObject>(spawnedPrefab, gameObject.transform);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackController : MonoBehaviour
{
    public PlayerController player;
    private int healthPackIncrease = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void OnTriggerEnter2D(Collider2D other)
	{
        Debug.Log("Trigger enter");
		if (other.gameObject.CompareTag("PlayerBullet") || other.gameObject.CompareTag("Player"))
		{
            player.GetComponent<PlayerController>().IncreaseHealth(healthPackIncrease);
            Destroy(gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
        Debug.Log("Collision enter");
		if (other.gameObject.CompareTag("PlayerBullet") || other.gameObject.CompareTag("Player"))
		{
            player.GetComponent<PlayerController>().IncreaseHealth(healthPackIncrease);
            Destroy(gameObject);
		}
	}

    
}

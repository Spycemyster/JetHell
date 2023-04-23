using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class HealthPackController : MonoBehaviour
{
    public PlayerController player;
    private int healthPackIncrease = 1;
    private AudioSource healthSound;
    // Start is called before the first frame update
    void Start()
    {
        healthSound = gameObject.GetComponent<AudioSource>();
        
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
            healthSound.Play();
            Destroy(gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
        Debug.Log("Collision enter");
		if (other.gameObject.CompareTag("PlayerBullet") || other.gameObject.CompareTag("Player"))
		{
            player.GetComponent<PlayerController>().IncreaseHealth(healthPackIncrease);
            healthSound.Play();
            Destroy(gameObject);
		}
	}

    
}

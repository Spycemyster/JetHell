using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class HealthPackController : MonoBehaviour
{
    public PlayerController player;
    private int healthPackIncrease = 1;
    private AudioSource healthSound;

    private bool pickedUp = false;
    // Start is called before the first frame update
    void Start()
    {
        healthSound = gameObject.GetComponent<AudioSource>();
        
    }

	void OnTriggerEnter2D(Collider2D other)
	{
        if (!pickedUp)
        {
            Debug.Log("Trigger enter");
            if (other.gameObject.CompareTag("PlayerBullet") || other.gameObject.CompareTag("Player"))
            {
                PickUp();
            }

        }
	}

	void OnCollisionEnter2D(Collision2D other)
	{
        if (!pickedUp)
        {
            Debug.Log("Collision enter");
            if (other.gameObject.CompareTag("PlayerBullet") || other.gameObject.CompareTag("Player"))
            {
                PickUp();
            }
        }
	}

    public void PickUp()
    {
        pickedUp = true;
        player.GetComponent<PlayerController>().IncreaseHealth(healthPackIncrease);
        HideObject();
        healthSound.Play();
        Destroy(gameObject, 1f);
    }

    public void HideObject()
    {
        // Remove images
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            SpriteRenderer childSprite = child.GetComponent<SpriteRenderer>();
            if (childSprite != null)
            {
                childSprite.enabled = false;
            }
        }

        // Remove box colllider
        GetComponent<BoxCollider2D>().enabled = false;
    }

    
}

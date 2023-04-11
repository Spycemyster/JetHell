using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialPickupHandler : MonoBehaviour
{
    public PlayerController player;
    public int specialItem;

	void OnTriggerEnter2D(Collider2D other)
	{
        Debug.Log("Trigger enter");
		if (other.gameObject.CompareTag("PlayerBullet") || other.gameObject.CompareTag("Player"))
		{
            player.PlayerSetSpecial(specialItem);
            Destroy(gameObject);
		}
	}
}

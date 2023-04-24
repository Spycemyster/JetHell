using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]


public class SpecialPickupHandler : MonoBehaviour
{
    public PlayerController player;
    public int specialItem;
    private AudioSource ammoSound;
    private bool pickedUp = false;

    void Start() {

        ammoSound = gameObject.GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other)
	{
        if (!pickedUp)
        {
            Debug.Log("Trigger enter");
            if (other.gameObject.CompareTag("PlayerBullet") || other.gameObject.CompareTag("Player"))
            {
                //player.PlayerSetSpecial(specialItem);
                //Destroy(gameObject);
                PickUp();
            }
        }
    }

    public void PickUp()
    {
        pickedUp = true;
        player.PlayerSetSpecial(specialItem);
        ammoSound.Play();
        Destroy(gameObject, 1f);
    }

}

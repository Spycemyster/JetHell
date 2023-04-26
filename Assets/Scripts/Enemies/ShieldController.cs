using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public GameObject parent;
    private GameObject player;
    private PlayerController playerScript;

    private Rigidbody2D rb;

    private float rotationSpeed = 40f;
    private bool isInitialized = false;

    public void InitializeShield(GameObject parent, GameObject player)
    {
        this.player = player;
        playerScript = player.GetComponent<PlayerController>();

        this.parent = parent;
        isInitialized = true;

        rb = GetComponent<Rigidbody2D>();
    }
    
    void FixedUpdate()
    {
        if (!isInitialized) return;

        transform.position = parent.transform.position;

        MoveTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        Vector2 playerPos = player.transform.position;
        Vector2 shieldPos = transform.position;

        Vector2 playerDirection = playerPos - shieldPos;

        //Quaternion neededRotation = Quaternion.LookRotation((playerPos - shieldPos), new Vector3(0, 1, 0));
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, neededRotation, Time.deltaTime * rotationSpeed); 


        float shieldAngle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;
        float realAngle;
        // Compare to real angle
        /*if (shieldAngle - rb.rotation > shieldAngle + (360 - rb.rotation))
        {
            // should go negative
            realAngle = rb.rotation - (rb.rotation - Mathf.Max(shieldAngle + (360 - rb.rotation))) * 1f * Time.deltaTime;
        }
        else
        {
            // should go positive
            realAngle = rb.rotation + (rb.rotation + Mathf.Max(shieldAngle - rb.rotation)) * 1f * Time.deltaTime;
        }*/

        float moveAngle = ((shieldAngle) - (rb.rotation) + 540) % 360 - 180;
        realAngle = Mathf.Clamp(moveAngle, -30f, 30f) * Time.fixedDeltaTime;

        //float shieldMove = Mathf.Max(shieldAngle, 30) * Time.fixedDeltaTime;
        //float realAngle = (rb.rotation + shieldMove) % 360;
        rb.rotation = (rb.rotation + realAngle + 180) % 360 - 180;
        
    }

}

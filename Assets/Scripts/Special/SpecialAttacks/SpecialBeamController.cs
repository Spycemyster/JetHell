using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBeamController : MonoBehaviour, ISpecial
{
	public PlayerController Player
	{
		get;
		set;
	}
	public SpecialHandler specialHandler
	{
		get;
		set;
	}

    private int ammo;
    [SerializeField] private LineRenderer lr;
    private float defaultBeamDist = 100f;
    private bool firingLaser = false;
    private const float fireDuration = 1f;
    private float fireTime = 0f;


    public void SetSpecial()
    {
        ammo = 4;
		specialHandler.SetAmmo(ammo);
    }

    public void FireSpecial()
    {
        if (ammo == 0) return;
        ammo--;
		Debug.Log("Bullets left: " + ammo);
		specialHandler.SetAmmo(ammo);

        fireTime = 0f;
    }

    private void Update()
    {
        if (firingLaser)
        {
            fireTime += Time.deltaTime;

            if (fireTime <= fireDuration)
            {
                FireLaser();
            }
            else
            {
                StopRay();
                firingLaser = false;
            }
        }
    }

    public bool OutOfAmmo()
    {
        
        return ammo == 0;
    }

    private void FireLaser()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPosition = Player.transform.position;

        Vector2 direction = mousePosition - playerPosition;
		direction = direction.normalized;

        RaycastHit2D hit = Physics2D.Raycast(playerPosition, mousePosition);


        if (hit)
        {
            DrawRay(playerPosition, hit.point);
        }
        else
        {
            DrawRay(playerPosition, playerPosition + direction.normalized * defaultBeamDist);
        }
    }

    private void DrawRay(Vector2 startPos, Vector2 endPos)
    {
        if (!lr.enabled)
        {
            lr.enabled = true;
        }
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
    }

    private void StopRay()
    {
        lr.enabled = false;
    }
}

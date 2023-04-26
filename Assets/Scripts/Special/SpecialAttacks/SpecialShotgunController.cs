using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialShotgunController : MonoBehaviour, ISpecial
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
    [SerializeField] private GameObject m_bulletPrefab;
    private Vector2 playerBulletScale = new Vector2(.4f, .8f);
	private const float bulletSpeed = 23f;
	private const float bulletSpeedRange = 2f;
	private const float bulletSpreadRange = .3f;
	private const float shootForce = 14f;

	private float maxAmmoTime = 10f;
	private float ammoTime;

	private const float maxFireDelay = 1f;
	private float fireDelayTime = maxFireDelay;


    public void SetSpecial()
    {
        //ammo = 4;
		if (specialHandler)
		{
			Debug.Log("Special handler exists");
		}
		else
		{
			Debug.Log("Special handler does not exist");
		}
		ammoTime = maxAmmoTime;
		specialHandler.SetAmmo(ammoTime/maxAmmoTime);
    }

    public void FireSpecial()
    {
		if (fireDelayTime < maxFireDelay)
		{
			return;
		}

		fireDelayTime = 0f;
        
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPosition = Player.transform.position;

        Vector2 direction = mousePosition - playerPosition;
		direction = direction.normalized;

		Player.PlayerAddForce(-direction, shootForce);

		for (int i = 0; i < 15; i++)
		{
			GameObject bullet = Instantiate(m_bulletPrefab, Player.transform.position, Quaternion.identity);
			Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

			Vector2 offset = Random.insideUnitCircle * bulletSpreadRange;
			Vector2 bulletDirection = direction + offset;

        	float bulletAngle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg - 90f;

			float offsetBulletSpeed = bulletSpeed + Random.insideUnitCircle.x * bulletSpeedRange;
			bulletRb.velocity = bulletDirection.normalized * offsetBulletSpeed;
			bulletRb.rotation = bulletAngle;
			bullet.transform.localScale = playerBulletScale;

			// Enable box collision now so previous actions would not affect collision
			bullet.GetComponent<BoxCollider2D>().enabled = true;
		}
    }

    public bool OutOfAmmo()
    {
        return ammoTime <= 0;
    }

	private void Update()
	{
		if (!GameHandler.isResting)
		{
			ammoTime -= Time.deltaTime;
			specialHandler.SetAmmo(ammoTime/maxAmmoTime);
		}
		if (ammoTime <= 0)
		{
			specialHandler.RemoveSpecial();
		}
		if (fireDelayTime <= maxFireDelay)
		{
			fireDelayTime += Time.deltaTime;
		}
	}

    public void DestroyedValue(float value)
    {
        ammoTime = Mathf.Clamp(ammoTime + value, 0, maxAmmoTime);

		specialHandler.SetAmmo(ammoTime/maxAmmoTime);
    }
}

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
	private const float shootForce = 14f;


    public void SetSpecial()
    {
        ammo = 4;
    }

    public void FireSpecial()
    {
        if (ammo == 0) return;
        ammo--;
		Debug.Log("Bullets left: " + ammo);
        
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPosition = Player.transform.position;

        Vector2 direction = mousePosition - playerPosition;
		direction = direction.normalized;

		Player.PlayerAddForce(-direction, shootForce);

		for (int i = 0; i < 15; i++)
		{
			GameObject bullet = Instantiate(m_bulletPrefab, Player.transform.position, Quaternion.identity);
			Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

			Vector2 offset = Random.insideUnitCircle * 0.5f;
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
        return ammo == 0;
    }
}

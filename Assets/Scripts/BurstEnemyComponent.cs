using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstEnemyComponent : MonoBehaviour, IEnemy
{
	[SerializeField]
	public PlayerController Player
	{
		get;
		set;
	}

	private int m_health = 5;
	[SerializeField] private GameObject m_bulletPrefab;
	private const float FIRE_RATE = 2f;
	private const int BURST_COUNT = 3;
	private const float BURST_DELAY = 2f;

	private float m_fireTimer = 0f;
	private float m_delayTimer = 0f;
	private int m_burstCount = 0;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		m_fireTimer += Time.fixedDeltaTime;
		m_delayTimer += Time.fixedDeltaTime;
        if (m_delayTimer > BURST_DELAY && m_fireTimer > 1 / FIRE_RATE)
		{
			m_fireTimer = 0f;

			m_burstCount++;
			if (m_burstCount >= BURST_COUNT)
			{
				m_burstCount = 0;
				m_delayTimer = 0f;
			}
			else
			{
				ShootBullet();
			}
		}
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<PlayerBulletController>() != null)
		{
			if (--m_health <= 0)
			{
				Destroy(gameObject);
			}
			Destroy(other.gameObject);
		}
	}

	void ShootBullet()
	{
		GameObject bullet = Instantiate(m_bulletPrefab, transform.position, Quaternion.identity);
		bullet.transform.position = transform.position;
		TrackingBulletComponent trackingComponent = bullet.GetComponent<TrackingBulletComponent>();
		if (trackingComponent)
		{
			trackingComponent.Player = Player;
		}

		LinearBulletComponent linearComponent = bullet.GetComponent<LinearBulletComponent>();
		if (linearComponent)
		{
			Vector2 toPlayer = Player.transform.position - transform.position;
			toPlayer = toPlayer.normalized;
			float angle = Mathf.Atan2(toPlayer.y, toPlayer.x);
			linearComponent.Velocity = new Vector2(Mathf.Cos(angle + Random.Range(-Mathf.PI / 100, Mathf.PI / 100)), Mathf.Sin(angle + Random.Range(-Mathf.PI / 100, Mathf.PI / 100)));
		}

	}
}

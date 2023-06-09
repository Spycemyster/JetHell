using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]


public class BurstEnemyComponent : MonoBehaviour, IEnemy
{
	[SerializeField]
	public PlayerController Player
	{
		get;
		set;
	}

	private int m_maxHealth = 5;
	private int m_health = 5;
	[SerializeField] private GameObject m_bulletPrefab;
	private const float FIRE_RATE = 2f;
	private const int BURST_COUNT = 3;
	private const float BURST_DELAY = 2f;

	private float m_fireTimer = 0f;
	private float m_delayTimer = 0f;
	private int m_burstCount = 0;

	private EnemyHealthController m_enemyHealthScript;
	//sound
	private AudioSource burstSound;
	private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
		m_enemyHealthScript = GetComponentInChildren<EnemyHealthController>();

		//sound
		burstSound = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		m_fireTimer += Time.fixedDeltaTime;
		m_delayTimer += Time.fixedDeltaTime;
        if (m_delayTimer > BURST_DELAY && m_fireTimer > 1 / FIRE_RATE && !isDead)
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
		if (other.gameObject.GetComponent<PlayerBulletController>() != null && !isDead)
		{
			TakeDamage();
			if (m_health <= 0)
			{
				DestroyedByPlayer();
			}
			Destroy(other.gameObject);
		}
	}

	public void DestroyedByPlayer()
	{
		PlayerController playerScript = Player.GetComponent<PlayerController>();
		burstSound.Play();
		playerScript.AddKill();
		playerScript.DestroyedValue(.5f);
		isDead = true;
		HideObject();
		Destroy(gameObject, 1f);
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

	public int TakeDamage(int damage = 1)
	{
		m_health -= damage;
		
		m_enemyHealthScript.SetHealthEnemy((float)m_health/m_maxHealth);
		PlayerController.AddHit();

		return m_health;
	}

	public void HideObject()
	{
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunEnemyCompoennt : MonoBehaviour, IEnemy
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
	private const float DELAY_TIME = 1f;

	private float m_delayTimer = 0f;
	private Vector2 targetPoint;

	private bool m_isMoving = false;
	private Coroutine m_behaviorCoroutine = null;

	public AudioSource shotgunSound;

	private float moveSpeed = 5f;

	private LineRenderer lr;
	private bool lineOn;

	private EnemyHealthController m_enemyHealthScript;

	void Start()
	{
		m_behaviorCoroutine = StartCoroutine(Behavior());

		lr = GetComponent<LineRenderer>();
		lineOn = false;

		m_enemyHealthScript = GetComponentInChildren<EnemyHealthController>();
	}

	IEnumerator Behavior()
	{
		while (true)
		{
			m_isMoving = false;
			// find a point near the player
			if (Player)
			{
				targetPoint = Random.onUnitSphere * 5f + Player.transform.position;
			}
			yield return new WaitForSeconds(DELAY_TIME);
			// move towards it
			m_isMoving = true;

			while (Vector2.Distance(transform.position, targetPoint) > 0.1f)
			{
				yield return new WaitForEndOfFrame();
			}
			PrepareShoot();
			yield return new WaitForSeconds(DELAY_TIME);
			// shoot in player direction
			ShootBullet();
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<PlayerBulletController>() != null)
		{
			TakeDamage();
			if (m_health <= 0)
			{
				StopCoroutine(m_behaviorCoroutine);
				Destroy(gameObject);
			}

			Destroy(other.gameObject);
		}
	}

	void ShootBullet()
	{
		if (!Player)
			return;
		Vector2 direction = Player.transform.position - transform.position;
		direction = direction.normalized;

		for (int i = 0; i < 15; i++)
		{
			GameObject bullet = Instantiate(m_bulletPrefab, transform.position, Quaternion.identity) as GameObject;
			bullet.transform.position = transform.position;
			LinearBulletComponent linearComponent = bullet.GetComponent<LinearBulletComponent>();
			if (linearComponent)
			{
				// generate a direction vector that is randomly offset from the player direction
				Vector2 offset = Random.insideUnitCircle * 0.5f;
				Vector2 bulletDirection = direction + offset;
				bulletDirection = bulletDirection.normalized;
				linearComponent.Velocity = bulletDirection * 5f;
			}
		}
		StopLine();
	}

	void FixedUpdate()
	{
		if (m_isMoving)
		{
			transform.position = Vector2.MoveTowards(transform.position, targetPoint, moveSpeed * Time.fixedDeltaTime);
		}
	}

	void Update()
	{
		if (lineOn)
		{
			SetLinePosition();
		}
	}

	void PrepareShoot()
	{
		shotgunSound.Play();
		SetLinePosition();
		lineOn = true;
	}

	void SetLinePosition()
	{
		lr.enabled = true;
		lr.positionCount = 2;

		Vector3[] pos = new Vector3[2];
		pos[0] = Player.transform.position;
		pos[1] = transform.position;

		lr.SetPositions(pos);
	}

	void StopLine()
	{
		lineOn = false;
		lr.positionCount = 0;
		lr.enabled = false;
	}

	public int TakeDamage(int damage = 1)
	{
		m_health -= damage;

		m_enemyHealthScript.SetHealthEnemy((float)m_health/m_maxHealth);

		return m_health;
	}
}

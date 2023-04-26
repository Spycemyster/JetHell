using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]


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

	private float moveSpeed = 5f;

	private LineRenderer lr;
	private bool lineOn;
	private const float maxLineDist = 5f;

	private EnemyHealthController m_enemyHealthScript;

	//sound
	private AudioSource shotgunSound;
	private bool isDead = false;

	void Start()
	{
		m_behaviorCoroutine = StartCoroutine(Behavior());

		lr = GetComponent<LineRenderer>();
		lineOn = false;

		m_enemyHealthScript = GetComponentInChildren<EnemyHealthController>();

		//sound
		shotgunSound = gameObject.GetComponent<AudioSource>();
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
		if (other.gameObject.GetComponent<PlayerBulletController>() != null && !isDead)
		{
			TakeDamage();
			if (m_health <= 0)
			{
				StopCoroutine(m_behaviorCoroutine);
                DestroyedByPlayer();
			}

			Destroy(other.gameObject);
		}
	}

	public void DestroyedByPlayer()
	{
		PlayerController playerScript = Player.GetComponent<PlayerController>();
		shotgunSound.Play();
		playerScript.AddKill();
		playerScript.DestroyedValue(.5f);
		isDead = true;
		HideObject();
		Destroy(gameObject, 2f);
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
		SetLinePosition();
		lineOn = true;
	}

	void SetLinePosition()
	{
		lr.enabled = true;
		lr.positionCount = 2;

		Vector2 playerPos = Player.transform.position;
		Vector2 enemyPos = transform.position;

		Vector2 aimDirection = playerPos - enemyPos;
		Vector2 aimPoint = enemyPos + aimDirection.normalized * maxLineDist;

		Vector3[] pos = new Vector3[2];
		pos[0] = aimPoint;
		pos[1] = enemyPos;

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
		PlayerController.AddHit();

		return m_health;
	}

	public void HideObject()
	{
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;
		StopLine();
	}
}

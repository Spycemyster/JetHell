using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]


public class SniperController : MonoBehaviour, IEnemy
{
	[SerializeField]
	public PlayerController Player
	{
		get;
		set;
	}
	private int m_maxHealth = 1;
	private int m_health = 1;
	[SerializeField] private GameObject m_bulletPrefab;
	private const float DELAY_TIME = 1f;

	private float m_delayTimer = 0f;
	private Vector2 targetPoint;

	private bool m_isMoving = false;
	private Coroutine m_behaviorCoroutine = null;

	private float moveSpeed = 4f;

	private LineRenderer lr;
	private bool lineOn;
	private const float maxLineDist = 10f;

    private float avgTime = 4f;
    private const float variation = 1f;
    private const float shootTime = 1f;

	private EnemyHealthController m_enemyHealthScript;

    private Vector3 target;

	//sounds
	private AudioSource sniperSound;
	private bool isDead = false;

	void Start()
	{
		m_behaviorCoroutine = StartCoroutine(Behavior());

		lr = GetComponent<LineRenderer>();
		lineOn = false;

		m_enemyHealthScript = GetComponentInChildren<EnemyHealthController>();

		//sound
		sniperSound = gameObject.GetComponent<AudioSource>();
	}

	IEnumerator Behavior()
	{
		while (true)
		{
            yield return new WaitForSeconds(.2f); // wait for initialization
            // take aim
			PrepareShoot();
			yield return new WaitForSeconds(shootTime);
			// shoot in player direction
			ShootBullet();

            float waitTime = Random.Range(avgTime - variation, avgTime + variation);
            // wait for amount of time
			yield return new WaitForSeconds(waitTime);

		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<PlayerBulletController>() != null && !isDead)
		{
			TakeDamage();
			if (m_health <= 0)
			{
				if (m_behaviorCoroutine != null)
				{
					StopCoroutine(m_behaviorCoroutine);
				}
				DestroyedByPlayer();
			}

			Destroy(other.gameObject);
		}
	}

	public void DestroyedByPlayer()
	{
		PlayerController playerScript = Player.GetComponent<PlayerController>();
		sniperSound.Play();
		playerScript.AddKill();
		playerScript.DestroyedValue(.5f);
		isDead = true;
		HideObject();
		Destroy(gameObject, 1f);
	}

	void ShootBullet()
	{
		if (!Player)
			return;
		Vector2 direction = target - transform.position;
		direction = direction.normalized;

        GameObject bullet = Instantiate(m_bulletPrefab, transform.position, Quaternion.identity) as GameObject;
        bullet.transform.position = transform.position;
        LinearBulletComponent linearComponent = bullet.GetComponent<LinearBulletComponent>();
        if (linearComponent)
        {
            // generate a direction vector that is randomly offset from the player direction
            Vector2 bulletDirection = direction;
            bulletDirection = bulletDirection.normalized;
            linearComponent.Velocity = bulletDirection * 50f;
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
		if (Player) 
		{
			//target = Player.transform.position;

			Vector2 playerPos = Player.transform.position;
			Vector2 enemyPos = transform.position;

			Vector2 aimDirection = playerPos - enemyPos;
			target = enemyPos + aimDirection.normalized * maxLineDist;

			SetLinePosition();
			lineOn = true;
		}
		else 
		{
			Debug.Log("Player not initilized in sniper: " + name);
		}
	}

	void SetLinePosition()
	{
		lr.enabled = true;
		lr.positionCount = 2;

		Vector3[] pos = new Vector3[2];
		pos[0] = target;
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
		PlayerController.AddHit();

		return m_health;
	}

	public void SetWaitTime(float avgWaitTime)
	{
		avgTime = avgWaitTime;
	}

	public void HideObject()
	{
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;
		StopLine();
	}
}

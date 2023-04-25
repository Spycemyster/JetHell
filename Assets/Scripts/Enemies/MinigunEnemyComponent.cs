using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]


public class MinigunEnemyComponent : MonoBehaviour, IEnemy
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
	private const float DELAY_TIME = 4f;

	private float m_delayTimer = 0f;
	private Vector2 targetPoint;

	private bool m_isMoving = false;
	private Coroutine m_behaviorCoroutine = null;

	private float moveSpeed = 5f;

	private LineRenderer lr;
	private bool lineOn;
	private const float maxLineDist = 5f;

	private EnemyHealthController m_enemyHealthScript;

    private const float m_shootDelay = .1f;
    private float m_shootDelayTimer = 0f;
    private const float m_shootDuration = 2f;

    private bool m_isShooting = false;

	//sound
	private AudioSource minigunSound;
	private bool isDead = false;

	void Start()
	{
		m_behaviorCoroutine = StartCoroutine(Behavior());

		lr = GetComponent<LineRenderer>();
		lineOn = false;

		m_enemyHealthScript = GetComponentInChildren<EnemyHealthController>();

		//sound
		minigunSound = gameObject.GetComponent<AudioSource>();
	}

	IEnumerator Behavior()
	{
		while (true)
		{
			//PrepareShoot();

            float delay = DELAY_TIME + Random.Range(-1f, 1f);

			yield return new WaitForSeconds(DELAY_TIME);
			// shoot in player direction
			StartShooting();

            yield return new WaitForSeconds(m_shootDuration);
            StopShooting();
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
                DestroyedByPlayer();
			}

			Destroy(other.gameObject);
		}
	}

	public void DestroyedByPlayer()
	{
		PlayerController playerScript = Player.GetComponent<PlayerController>();
		minigunSound.Play();
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
		Vector2 direction = Player.transform.position - transform.position;
		direction = direction.normalized;

        GameObject bullet = Instantiate(m_bulletPrefab, transform.position, Quaternion.identity) as GameObject;
        bullet.transform.position = transform.position;
        LinearBulletComponent linearComponent = bullet.GetComponent<LinearBulletComponent>();
        if (linearComponent)
        {
            // generate a direction vector that is randomly offset from the player direction
            Vector2 offset = Random.insideUnitCircle * 0.3f;
            Vector2 bulletDirection = direction + offset;
            bulletDirection = bulletDirection.normalized;
            linearComponent.Velocity = bulletDirection * 5f;
        }
	}

    void StartShooting()
    {
        m_isShooting = true;
    }

    void StopShooting()
    {
        m_isShooting = false;
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
        if (m_isShooting)
        {
            m_shootDelayTimer += Time.deltaTime;
            if (m_shootDelayTimer > m_shootDelay)
            {
                m_shootDelayTimer = 0f;
                ShootBullet();
            }
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

	public int TakeDamage(int damage = 1)
	{
		m_health -= damage;

		m_enemyHealthScript.SetHealthEnemy((float)m_health/m_maxHealth);

		return m_health;
	}

	public void HideObject()
	{
		GetComponent<SpriteRenderer>().enabled = false;
		GetComponent<BoxCollider2D>().enabled = false;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpongeBossComponent : MonoBehaviour, IEnemy
{
	[SerializeField]
	public PlayerController Player
	{
		get;
		set;
	}
	private int m_maxHealth = 50;
	private int m_health = 50;
	[SerializeField] private GameObject m_bulletPrefab;
	[SerializeField] private GameObject m_deathExplosionPrefab;

	private float m_delayTimer = 0f;
	private Vector2 targetPoint;

	private Coroutine m_behaviorCoroutine = null;

	private float moveSpeed = 5f;

	private LineRenderer lr;
	private bool lineOn;
	private const float maxLineDist = 5f;

	private EnemyHealthController m_enemyHealthScript;

    private const float m_shootDelay = .1f;
    private float m_shootDelayTimer = 0f;
    private const float m_shootDuration = 2f;

    private Rigidbody2D rb;
    private Vector2 lastVelocity;
    private Vector2 initialVelocity = new Vector2(5f, 5f);

    private float initialScale = 3f;
    private float finalScale = 6f;
    private float initialSpeed;
    private float finalSpeed;

	private float initialFireDelay = 2f;
    private float finalFireDelay = .5f;

	void Start()
	{
		m_behaviorCoroutine = StartCoroutine(Behavior());

		lr = GetComponent<LineRenderer>();
		lineOn = false;

		m_enemyHealthScript = GetComponentInChildren<EnemyHealthController>();

        rb = GetComponent<Rigidbody2D>();

        rb.velocity = initialVelocity;

        initialSpeed = initialVelocity.magnitude;
        finalSpeed = initialSpeed * 3f;
	}

	void OnDestroy()
	{
		Instantiate(m_deathExplosionPrefab, transform.position, Quaternion.identity);
	}

	IEnumerator Behavior()
	{
		while (true)
		{
            float fireDelay = Mathf.Lerp(finalFireDelay, initialFireDelay, (float)m_health/m_maxHealth);
			yield return new WaitForSeconds(fireDelay);
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
                DestroyedByPlayer();
			}

			Destroy(other.gameObject);
		}
	}

    void OnCollisionEnter2D(Collision2D collider)
    {
        float speed = lastVelocity.magnitude;
        Vector2 direction = Vector3.Reflect(lastVelocity.normalized, collider.contacts[0].normal);

        rb.velocity = direction * lastVelocity.magnitude;

    }

	public void DestroyedByPlayer()
	{
		PlayerController playerScript = Player.GetComponent<PlayerController>();
		playerScript.AddKill();
		playerScript.DestroyedValue(.5f);
		Destroy(gameObject);
	}

	void ShootBullet()
	{
		if (!Player)
			return;
		Vector2 direction = Player.transform.position - transform.position;
		direction = direction.normalized;

        int count = 10;
        for (int i = 0; i < count; i++)
        {
            GameObject bullet = Instantiate(m_bulletPrefab, transform.position, Quaternion.identity) as GameObject;
            bullet.transform.position = transform.position;
            LinearBulletComponent linearComponent = bullet.GetComponent<LinearBulletComponent>();

            if (linearComponent)
            {
                var angle = i * (360f / count);
                var bulletDirection = Quaternion.Euler(0, 0, angle) * Vector3.up;
                linearComponent.Velocity = bulletDirection * 5f;
            }

        }
	}

	void FixedUpdate()
	{
        lastVelocity = rb.velocity;
	}

	void Update()
	{
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

        // Update speed and scale
        float newScale = Mathf.Lerp(finalScale, initialScale, (float)m_health/m_maxHealth);
        Vector2 scale = new Vector2(newScale, newScale);
        transform.localScale = scale;

        float newSpeed = Mathf.Lerp(finalSpeed, initialSpeed, (float)m_health/m_maxHealth);
        rb.velocity = rb.velocity.normalized * newSpeed;
        lastVelocity = rb.velocity;

		return m_health;
	}
}

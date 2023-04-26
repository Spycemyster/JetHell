using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FinalBossComponent : MonoBehaviour, IEnemy
{
	[SerializeField]
	public PlayerController Player
	{
		get;
		set;
	}

	private int m_maxHealth = 50;
	private int m_health = 50;
	private const float DELAY_TIME = 1f;

	private float m_delayTimer = 0f;
	private Vector2 targetPoint;

	private bool m_isMoving = false;
	private Coroutine m_behaviorCoroutine = null;

	private float moveSpeed = 4f;

    private float avgTime = 2.5f;
    private const float variation = .5f;
    private const float shootTime = 1f;

	private EnemyHealthController m_enemyHealthScript;

    private Vector3 target;

	//sounds
	private AudioSource deathSound;
	private bool isDead = false;

    [SerializeField] private GameObject attackIndicatorPrefab;
    private HashSet<GameObject> attackIndicators = new HashSet<GameObject>();
    private float attackRadius = 2f;
    private float attackTime = 1.5f;
    private bool m_isAttacking = false;

    private Rigidbody2D rb;

    [SerializeField] GameObject shieldPrefab;
    private GameObject currentShield;

    [SerializeField] GameObject m_bulletPrefab;

	void Start()
	{
		m_behaviorCoroutine = StartCoroutine(Behavior());

		m_enemyHealthScript = GetComponentInChildren<EnemyHealthController>();

		//sound
		//deathSound = gameObject.GetComponent<AudioSource>();

        Invoke("InstantiateShield", .05f);
	}

    public void InstantiateShield()
    {
        currentShield = Instantiate(shieldPrefab, transform.position, Quaternion.identity);
        currentShield.GetComponent<ShieldController>().InitializeShield(gameObject, Player.gameObject);
    }

	IEnumerator Behavior()
	{
		while (true)
		{
            yield return new WaitForSeconds(.2f); // wait for initialization
            // shoot every 2 seconds, loop every 8

            ShootBullet();
			yield return new WaitForSeconds(Random.Range(avgTime - variation, avgTime + variation));
            ShootBullet();
			yield return new WaitForSeconds(Random.Range(avgTime - variation, avgTime + variation));

            ShootBullet();

            yield return new WaitForSeconds(2f);

            // take aim
			PrepareAttack();
			yield return new WaitForSeconds(.5f);
			PrepareAttack();
			yield return new WaitForSeconds(.5f);
			PrepareAttack();
			yield return new WaitForSeconds(2f);

            ShootBullet();
			yield return new WaitForSeconds(Random.Range(avgTime - variation, avgTime + variation));

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
		//deathSound.Play();
		playerScript.AddKill();
		playerScript.DestroyedValue(.5f);
		isDead = true;
		HideObject();
		Destroy(gameObject, 1f);
        
        playerScript.IncreaseHealth(2);
	}
    
    public void PrepareAttack()
    {
        // Create attack indicator
        GameObject newAttack = Instantiate(attackIndicatorPrefab, Player.transform.position, Quaternion.identity);
        attackIndicators.Add(newAttack);
        AttackIndicatorController attackIndicatorScript = newAttack.GetComponent<AttackIndicatorController>();

        attackIndicatorScript.InitializeIndicator(attackRadius, attackTime, true, Attack, gameObject, followParent:false);
        m_isAttacking = true;
    }

    public void Attack(GameObject attackIndicator)
    {
        Debug.Log("Melee attack");
        AttackIndicatorController attackIndicatorScript = attackIndicator.GetComponent<AttackIndicatorController>();

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(attackIndicator.transform.position, attackIndicatorScript.attackRadius);
        foreach (Collider2D collider in hitColliders)
        {
            GameObject collided = collider.gameObject;
            if (collided.CompareTag("Player"))
            {
                collided.GetComponent<PlayerController>().TakeDamage(1f);
            }
        }

        // Remove indicator
        attackIndicators.Remove(attackIndicator);
        Destroy(attackIndicator);
        m_isAttacking = false;
    }

	void FixedUpdate()
	{
		if (m_isMoving)
		{
			//transform.position = Vector2.MoveTowards(transform.position, targetPoint, moveSpeed * Time.fixedDeltaTime);
            rb.velocity = (Player.transform.position - transform.position).normalized * moveSpeed; //Vector2.MoveTowards(transform.position, Player.transform.position, moveSpeed);
		}
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
		GetComponent<CircleCollider2D>().enabled = false;
        foreach (GameObject attackIndicator in attackIndicators)
        {
            Destroy(attackIndicator);
        }
	}

    void OnDestroy()
    {
        if (currentShield)
        {
            Destroy(currentShield);
        }
        foreach (GameObject attackIndicator in attackIndicators)
        {
            Destroy(attackIndicator);
        }
    }

	void ShootBullet()
	{
		if (!Player)
			return;
		Vector2 direction = Player.transform.position - transform.position;
		direction = direction.normalized;

        int count = 30;
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
}

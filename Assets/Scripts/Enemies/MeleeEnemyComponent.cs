using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]


public class MeleeEnemyComponent : MonoBehaviour, IEnemy
{
	[SerializeField]
	public PlayerController Player
	{
		get;
		set;
	}

	private int m_maxHealth = 4;
	private int m_health = 4;
	private const float DELAY_TIME = 1f;

	private float m_delayTimer = 0f;
	private Vector2 targetPoint;

	private bool m_isMoving = false;
	private Coroutine m_behaviorCoroutine = null;

	private float moveSpeed = 4f;

    private float avgTime = 4f;
    private const float variation = 1f;
    private const float shootTime = 1f;

	private EnemyHealthController m_enemyHealthScript;

    private Vector3 target;

	//sounds
	private AudioSource deathSound;
	private bool isDead = false;

    [SerializeField] private GameObject attackIndicatorPrefab;
	[SerializeField] private GameObject m_deathExplosionPrefab;
    private HashSet<GameObject> attackIndicators = new HashSet<GameObject>();
    private float attackRadius = 3f;
    private float attackTime = 1.5f;

	void Start()
	{
		m_behaviorCoroutine = StartCoroutine(Behavior());

		m_enemyHealthScript = GetComponentInChildren<EnemyHealthController>();

		//sound
		deathSound = gameObject.GetComponent<AudioSource>();
	}

	void OnDestroy()
	{
		Instantiate(m_deathExplosionPrefab, transform.position, Quaternion.identity);
	}

	IEnumerator Behavior()
	{
		while (true)
		{
            yield return new WaitForSeconds(.2f); // wait for initialization
            // take aim
			PrepareAttack();
			yield return new WaitForSeconds(shootTime);
			// shoot in player direction
			//Attack();

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
		deathSound.Play();
		playerScript.AddKill();
		playerScript.DestroyedValue(.5f);
		isDead = true;
		HideObject();
		Destroy(gameObject, 1f);
	}
    
    public void PrepareAttack()
    {
        // Create attack indicator
        GameObject newAttack = Instantiate(attackIndicatorPrefab, transform.position, Quaternion.identity, transform);
        attackIndicators.Add(newAttack);
        AttackIndicatorController attackIndicatorScript = newAttack.GetComponent<AttackIndicatorController>();

        attackIndicatorScript.InitializeIndicator(attackRadius, attackTime, true, Attack);
    }

    public void Attack(GameObject attackIndicator)
    {
        Debug.Log("Melee attack");
        AttackIndicatorController attackIndicatorScript = attackIndicator.GetComponent<AttackIndicatorController>();

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, attackIndicatorScript.attackRadius);
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
    }

	void FixedUpdate()
	{
		if (m_isMoving)
		{
			transform.position = Vector2.MoveTowards(transform.position, targetPoint, moveSpeed * Time.fixedDeltaTime);
		}
	}

	public int TakeDamage(int damage = 1)
	{
		m_health -= damage;

		m_enemyHealthScript.SetHealthEnemy((float)m_health/m_maxHealth);

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
        foreach (GameObject attackIndicator in attackIndicators)
        {
            Destroy(attackIndicator);
        }
	}
}

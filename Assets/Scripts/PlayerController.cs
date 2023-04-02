using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	public float Health => m_health;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private const float shootDelay = .2f;
    private float shootTimer = 0f;

    private const float shootForce = 4f;
    private const float maxVelocity = 7f;

    private const float bulletSpeed = 17f;
	private float m_health = 5f;
    private const float maxHealth = 5f;

    private float invicibilityTime = 0f;
    private const float invicibilityTimeMax = 1f;

    [SerializeField] private GameObject playerBulletPrefab;
    private Vector2 playerBulletScale = new Vector2(.4f, 1.2f); //new Vector2(.2f, .7f);

    [SerializeField] private GameObject healthObject;
    private HealthController healthScript;

    private bool isDead = false;

    private Color origColor;
    private Color flashColor = new Color(.8f, .8f, .8f);
    private float flashTime;
    private const float flashTimeMax = .1f;
    private bool isFlashed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        healthScript = healthObject.GetComponent<HealthController>();

        healthScript.SetHealth((int)m_health);

        sr = GetComponent<SpriteRenderer>();
        origColor = sr.color;
        isFlashed = false;
        flashTime = flashTimeMax;
        invicibilityTime = invicibilityTimeMax;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }

        if (Input.GetMouseButton(0))
        {
            if (shootTimer > shootDelay && !isDead)
            {
                ShootBullet();
            }
        }

        shootTimer += Time.deltaTime;
        if (isInvincible())
        {
            invicibilityTime += Time.deltaTime;
            flashTime += Time.deltaTime;
            if (flashTime > flashTimeMax)
            {
                flashTime = 0f;
                if (isFlashed)
                {
                    sr.color = origColor;
                    SetOpacity();
                } else
                {
                    sr.color = flashColor;
                    SetOpacity();
                }
                isFlashed = !isFlashed;
            }
        } else 
        {
            isFlashed = false;
            sr.color = origColor;
            SetOpacity();
        }
    }

	public void TakeDamage(float delta)
	{
		m_health -= delta;
        healthScript.SetHealth((int)m_health);
        SetOpacity();
		if (m_health <= 0f)
		{
			// die
			//Destroy(gameObject);
            FailState();
		}
        invicibilityTime = 0f;
        flashTime = flashTimeMax;
	}

    public void IncreaseHealth(int health)
    {
        m_health += health;
        healthScript.SetHealth((int)m_health);
    }

    public bool isInvincible()
    {
        return invicibilityTime < invicibilityTimeMax;
    }

    public void ShootBullet()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 playerPos = transform.position;

        Vector2 direction = playerPos - mousePosition;
        direction = direction.normalized;

        rb.AddForce(direction * shootForce, ForceMode2D.Impulse);

        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }

        //Debug.Log("vel: " + rb.velocity.magnitude);

        shootTimer = 0f;

        Vector2 bulletDirection = -direction;

        float playerBulletAngle = Mathf.Atan2(bulletDirection.y, bulletDirection.x) * Mathf.Rad2Deg - 90f;

        GameObject playerBullet = Instantiate(playerBulletPrefab, transform.position, Quaternion.identity);
        Rigidbody2D playerBulletRb = playerBullet.GetComponent<Rigidbody2D>();

        playerBulletRb.velocity = bulletDirection * bulletSpeed;
        playerBulletRb.rotation = playerBulletAngle;
        playerBullet.transform.localScale = playerBulletScale;

        
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void FailState()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        isDead = true;

        Invoke("RestartLevel", 3f);
    }

    public void SetOpacity()
    {
        Color playerColor = sr.color;
        float opacity = .2f + (m_health/maxHealth)*4f/5f;
        Color newPlayerColor = new Color(playerColor.r, playerColor.g, playerColor.b, opacity);
        sr.color = newPlayerColor;
    }
}

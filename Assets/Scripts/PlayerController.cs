using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	public delegate void PlayerEvent(PlayerController player);
	public PlayerEvent OnHurt, OnDeath;
	public float Health => m_health;
    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private const float shootDelay = .2f;
    private float shootTimer = 0f;

    private const float shootForce = 6f; // 4f;
    private const float maxVelocity = 9f; // 7f;

    private const float bulletSpeed = 40f; //27f;  // 17f;
	private float m_health = 5f;
    private const float maxHealth = 5f;

    private float invicibilityTime = 0f;
    private const float invicibilityTimeMax = 1f;

    [SerializeField] private GameObject playerBulletPrefab;
    private Vector2 playerBulletScale = new Vector2(.4f, .8f); //new Vector2(.4f, 1.2f);

    [SerializeField] private GameObject healthObject;
    private HealthController healthScript;

    private bool isDead = false;

    private Color origColor;
    private Color flashColor = new Color(.8f, .8f, .8f);
    private float flashTime;
    private const float flashTimeMax = .1f;
    private bool isFlashed;

    public bool m_isRestarting = false;

    [SerializeField] private GameObject killCountObject;
    private KillCountController killScript;
    private int kills = 0;

    [SerializeField] private SpecialHandler specialHandler;

    private float friction = 5f;
    private bool isSlowMode = false;

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

        // Temp testing code
        if (specialHandler)
        {
            specialHandler.SetSpecial(0);
        }

        if (killCountObject)
        {
            killScript = killCountObject.GetComponent<KillCountController>();
            killScript.SetKills(kills);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Input.GetKey(KeyCode.LeftShift))
        {
            ExitGame();
        }

        if (PauseMenuController.gameIsPaused)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GoToLevel(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GoToLevel(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            GoToLevel(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            GoToLevel(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            GoToLevel(4);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            GoToLevel(5);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            NextLevel();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            StopMusic();
        }
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            PlayMusic();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            isSlowMode = !isSlowMode;
        }

        if (Input.GetMouseButton(1))
        {
            if (specialHandler)
            {
                specialHandler.FireSpecial();
            }
            else
            {
                Debug.Log("Special handler not set");
            }
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


        if (isSlowMode)
        {
            //rb.velocity -= (rb.velocity * friction * Time.deltaTime);
            rb.velocity = rb.velocity.normalized * Mathf.Max(rb.velocity.magnitude - friction * Time.deltaTime, 0);
        }
    }

	public void TakeDamage(float delta)
	{
		OnHurt?.Invoke(this);
		m_health -= delta;
        healthScript.SetHealth((int)m_health);
        SetOpacity();
		if (m_health <= 0f)
		{
			// die
			//Destroy(gameObject);
			OnDeath?.Invoke(this);
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

        // Enable box collision now so previous actions would not affect collision
        playerBullet.GetComponent<BoxCollider2D>().enabled = true;

        
    }

    public void PlayerAddForce(Vector2 direction, float shootForce)
    {
        rb.AddForce(direction * shootForce, ForceMode2D.Impulse);

        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
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


    // ZW - Tests if player reached destination
	void OnTriggerEnter2D(Collider2D other)
	{
        if (other.CompareTag("Destination"))
        {
            Debug.Log("Player beat level");
            NextLevel();
        }
	}

    public void NextLevel()
    {
        Debug.Log("Next level");
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            m_isRestarting = true;
            Invoke("RestartLevel", 1f);
        }
    }

    public void GoToLevel(int idx)
    {
        if (SceneManager.sceneCountInBuildSettings > idx)
        {
            SceneManager.LoadScene(idx);
        }
    }

    public void PlayerSetSpecial(int idx)
    {
        specialHandler.SetSpecial(idx);
    }

    public void PlayerSpawnSpecial(int idx)
    {
        specialHandler.SpawnSpecialPickup(idx);
    }

    public void AddKill()
    {
        kills++;
        if (killScript)
        {
            killScript.SetKills(kills);
        }
        Debug.Log("Kills: " + kills);
    }

    public void StopMusic()
    {
        GameObject musicObject = GameObject.FindGameObjectWithTag("Music");
        if (musicObject) 
        {
            musicObject.GetComponent<MusicController>().StopMusic();
        }
    }

    public void PlayMusic()
    {
        GameObject musicObject = GameObject.FindGameObjectWithTag("Music");
        if (musicObject) 
        {
            musicObject.GetComponent<MusicController>().PlayMusic();
        }
    }

    public void DestroyedValue(float value)
    {
        if (specialHandler)
        {
            specialHandler.DestroyedValue(value);
        }
    }
}

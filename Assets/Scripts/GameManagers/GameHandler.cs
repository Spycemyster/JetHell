using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
	private enum GamePhase
	{
		REST_PHASE_BEFORE_SURVIVAL,
		SURVIVAL_PHASE,
		REST_PHASE_BEFORE_MINIGAME,
		MINIGAME_PHASE,
	}
	public Vector2 TopLeft = new Vector2(-30, 15);
	public Vector2 BottomRight = new Vector2(30, -15);
	public PlayerController Player;
	public Camera MainCamera;

	[SerializeField] private GameObject[] m_enemyPrefabs;

	[SerializeField] private AudioSource[] audioSources;
	[SerializeField] private GameObject[] minigamePrefabs;
	[SerializeField] private GameObject[] bossPrefabs;

	[SerializeField] private GameObject healthPackPrefab;
	[SerializeField] private GameObject survivalEnemiesParent;
	[SerializeField] private GameObject m_bulletContainer;
	[SerializeField] private TMP_Text phaseText;
	[SerializeField] private float restPhaseDuration = 2.0f;
	[SerializeField] private float minigameRestPhaseDuration = 2.0f;
	[SerializeField] private float survivalPhaseDuration = 20.0f;
	private int m_roundIndex = 0;

	private GamePhase m_gamePhase = GamePhase.REST_PHASE_BEFORE_SURVIVAL;
	private float m_spawnTimer = 0f;
	private float m_gameTime = 0f;
	private Minigame m_currentMinigame;
	private int enemiesSpawned = 0;
	private const int healthPackRate = 5;
	private Coroutine m_gameLoopCoroutine;
	private bool m_currentMinigameFinished = false;

	private List<int> m_randomBag;
	private int m_bagIndex = 0;
	private int m_bossIndex = 0;

	public static bool isResting = false;
	[SerializeField] private GameObject endGameScreen;

	private void Start()
	{
		Time.timeScale = 1f;
		m_spawnTimer = 2.0f;
		Player.OnHurt += OnPlayerHurt;
		Player.OnDeath += OnPlayerDie;
		m_gameLoopCoroutine = StartCoroutine(RunGamePhaseLoop());

		m_randomBag = new List<int>();
		for (int i = 0; i < minigamePrefabs.Length; i++)
		{
			m_randomBag.Add(i);
		}
		
		Shuffle<int>(m_randomBag);
		for (int i = 0; i < minigamePrefabs.Length; i++)
		{
			Debug.Log(m_randomBag[i]);
		}

	}

	private void OnPlayerDie(PlayerController player)
	{
		// slow down time for a second and shake the camera while it happens
		StartCoroutine(SlowAndShake(0.7f, 0.5f));
	}

	private void OnPlayerHurt(PlayerController player)
	{
		// slow down time for a second and shake the camera while it happens
		StartCoroutine(SlowAndShake(0.4f, 0.1f));
	}

	private IEnumerator RunGamePhaseLoop() {

		while (true)
		{
			switch (m_gamePhase)
			{
				case GamePhase.REST_PHASE_BEFORE_SURVIVAL:
					phaseText.enabled = true;
					phaseText.text = $"Survival Phase {m_roundIndex}";
					isResting = true;
					yield return new WaitForSeconds(restPhaseDuration);
					isResting = false;
					m_gamePhase = GamePhase.SURVIVAL_PHASE;
					phaseText.enabled = false;
					break;
				case GamePhase.SURVIVAL_PHASE:
					//survivalPhaseDuration = 1f; // Debugging line
					yield return new WaitForSeconds(survivalPhaseDuration);
					m_gamePhase = GamePhase.REST_PHASE_BEFORE_MINIGAME;
					break;
				case GamePhase.REST_PHASE_BEFORE_MINIGAME:
					phaseText.enabled = true;
					if (IsBossIncoming())
					{
						phaseText.text = "Boss Incoming...";
					}
					else
					{
						phaseText.text = "Minigame Incoming...";
					}
					isResting = true;
					yield return new WaitForSeconds(minigameRestPhaseDuration);
					isResting = false;
					phaseText.enabled = false;
					ClearAllSurvivalEnemies();
					//ClearAllEnemyBullets();


					GetAndSetRandomMinigame();
					m_gamePhase = GamePhase.MINIGAME_PHASE;
					break;
				case GamePhase.MINIGAME_PHASE:
					while (!m_currentMinigameFinished)
					{
						yield return new WaitForSeconds(1.0f);
					}
					m_gamePhase = GamePhase.REST_PHASE_BEFORE_SURVIVAL;
					break;
			}
		}
	}

	private void ClearAllSurvivalEnemies()
	{
		var enemies = survivalEnemiesParent.GetComponentsInChildren<Transform>();
		var survivalEnemyTransform = survivalEnemiesParent.transform;
		for (int i = enemies.Length - 1; i >= 0; i--)
		{
			if (enemies[i] == survivalEnemyTransform)
			{
				continue;
			}
			Destroy(enemies[i].gameObject);
		}
	}

	private void ClearAllEnemyBullets()
	{
		List<GameObject> toDestroy = new List<GameObject>();
		for (int i = 0; i < transform.parent.childCount; i++)
		{
			GameObject potentialBullet = transform.parent.GetChild(i).gameObject;
			if (potentialBullet.CompareTag("EnemyBullet"))
			{
				toDestroy.Add(potentialBullet);
			}
		}

		Debug.Log("Bullet size: " + toDestroy.Count);
		foreach (GameObject bullet in toDestroy)
		{
			Destroy(bullet);
		}
	}

	private void GetAndSetRandomMinigame()
	{
		m_currentMinigameFinished = false;
		GameObject level = GetPsuedoRandomLevel();
		m_currentMinigame = Instantiate(level).GetComponent<Minigame>();
		m_currentMinigame.InitializeLevel(Player, MainCamera, Mathf.Lerp(40.0f, 25.0f, (float)m_roundIndex / 10.0f));
		m_currentMinigame.OnCompleteMinigame += OnCompleteMinigame;
		m_currentMinigame.OnFailMinigame += OnFailMinigame;
	}

	private GameObject GetPsuedoRandomLevel()
	{
		if (m_bagIndex >= minigamePrefabs.Length)
		{
			// Reshuffle
			Shuffle<int>(m_randomBag);
			m_bagIndex = 0;
		}
		if (m_roundIndex % 4 == 3)
		{
			// Return boss level
			if (bossPrefabs != null && bossPrefabs.Length > 0)
			{
				GameObject bossLevel = bossPrefabs[m_bossIndex % bossPrefabs.Length];
				m_bossIndex++;
				return bossLevel;
			}
		}
		int level = m_randomBag[m_bagIndex];
		m_bagIndex++;
		return minigamePrefabs[level];
	}

	private bool IsBossIncoming()
	{
		if (bossPrefabs.Length > 0 && m_roundIndex % 4 == 3)
		{
			return true;
		}
		return false;
	}

	private bool BeatGame()
	{
		return m_bossIndex >= 3 || m_roundIndex >= 12;
	}

	public void GoToEndScreen(string descriptionStr)
	{
		if (endGameScreen != null)
		{
			EndMenuController endScript = endGameScreen.GetComponent<EndMenuController>();

			phaseText.enabled = false;

			Debug.Log("Accuracy: " + Player.GetAccuracy());
			endScript.RevealScreen(descriptionStr, Player.GetKills(), Player.GetAccuracy());
		}
	}

	private void OnFailMinigame()
	{
		m_currentMinigameFinished = true;
		DetachAndDestroyCurrentMinigame();
	}

	private void OnCompleteMinigame()
	{
		m_currentMinigameFinished = true;
		DetachAndDestroyCurrentMinigame();
	}

	private void DetachAndDestroyCurrentMinigame()
	{
		m_roundIndex++;
		m_currentMinigame.OnCompleteMinigame -= OnCompleteMinigame;
		m_currentMinigame.OnFailMinigame -= OnFailMinigame;
		Destroy(m_currentMinigame.gameObject);
	}

	private IEnumerator SlowAndShake(float time, float shakeAmount)
	{
		float timeElapsed = 0f;
		float originalTimeScale = 1f;
		Vector3 originalCameraPosition = MainCamera.transform.position;

		while (timeElapsed < time)
		{
			while (PauseMenuController.gameIsPaused) yield return null;
			timeElapsed += Time.deltaTime;
			//Time.timeScale = Mathf.Lerp(originalTimeScale, 0.1f, timeElapsed / time);
			Time.timeScale = Mathf.Lerp(originalTimeScale, 0.3f, timeElapsed / time);
			Time.fixedDeltaTime = 0.02f * Time.timeScale;
			MainCamera.transform.position = originalCameraPosition + Random.insideUnitSphere * shakeAmount;
			yield return null;
		}

		Time.timeScale = originalTimeScale;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;
		MainCamera.transform.position = originalCameraPosition;
	}

	
	void FixedUpdate()
	{
		if (!Player || Player.Health <= 0 || m_gamePhase != GamePhase.SURVIVAL_PHASE)
		{
			return;
		}

		m_gameTime += Time.fixedDeltaTime;
		m_spawnTimer += Time.fixedDeltaTime;

		//if (m_spawnTimer > Mathf.Lerp(2f, 1f, m_gameTime / 125f))
		if (m_spawnTimer > Mathf.Lerp(4.5f, 2f, m_gameTime / 125f))
		{
			m_spawnTimer = 0;
			SpawnRandomEnemy();

			enemiesSpawned++;
			if (enemiesSpawned % healthPackRate == 1)
			{
				SpawnHealthPack();
			}

			if (enemiesSpawned % 10 == 6)
			{
				Player.PlayerSpawnSpecial(0);
			}
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Plus))
		{
			GetPsuedoRandomLevel();
			m_roundIndex++;
			Debug.Log("round: " + m_roundIndex);
		}
		if (Input.GetKeyDown(KeyCode.Minus))
		{
			GoToEndScreen("You died!");
		}
	}

	private void SpawnRandomEnemy()
	{
		int randomIndex = Random.Range(0, m_enemyPrefabs.Length);
		GameObject enemy = Instantiate(m_enemyPrefabs[randomIndex], transform.position, Quaternion.identity, survivalEnemiesParent.transform);
		enemy.transform.position = new Vector2(Random.Range(TopLeft.x, BottomRight.x), Random.Range(BottomRight.y, TopLeft.y));
		enemy.GetComponent<IEnemy>().Player = Player;
	}

	private void SpawnHealthPack()
	{
		GameObject health = Instantiate(healthPackPrefab, transform.position, Quaternion.identity, survivalEnemiesParent.transform);
		health.transform.position = new Vector2(Random.Range(TopLeft.x, BottomRight.x), Random.Range(BottomRight.y, TopLeft.y));
		health.GetComponent<HealthPackController>().player = Player;
	}



	public void Shuffle<T>(IList<T> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = Random.Range(0, n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}
}

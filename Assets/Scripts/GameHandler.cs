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

	[SerializeField] private GameObject healthPackPrefab;
	[SerializeField] private GameObject survivalEnemiesParent;
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
	private void Start()
	{
		Time.timeScale = 1f;
		m_spawnTimer = 2.0f;
		Player.OnHurt += OnPlayerHurt;
		Player.OnDeath += OnPlayerDie;
		m_gameLoopCoroutine = StartCoroutine(RunGamePhaseLoop());
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
					yield return new WaitForSeconds(restPhaseDuration);
					m_gamePhase = GamePhase.SURVIVAL_PHASE;
					phaseText.enabled = false;
					break;
				case GamePhase.SURVIVAL_PHASE:
					yield return new WaitForSeconds(survivalPhaseDuration);
					m_gamePhase = GamePhase.REST_PHASE_BEFORE_MINIGAME;
					break;
				case GamePhase.REST_PHASE_BEFORE_MINIGAME:
					phaseText.enabled = true;
					phaseText.text = "Minigame Incoming...";
					yield return new WaitForSeconds(minigameRestPhaseDuration);
					phaseText.enabled = false;
					ClearAllSurvivalEnemies();
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

	private void GetAndSetRandomMinigame()
	{
		m_currentMinigameFinished = false;
		GameObject level = minigamePrefabs[Random.Range(0, minigamePrefabs.Length)];
		m_currentMinigame = Instantiate(level).GetComponent<Minigame>();
		m_currentMinigame.InitializeLevel(Player, MainCamera, Mathf.Lerp(40.0f, 25.0f, (float)m_roundIndex / 10.0f));
		m_currentMinigame.OnCompleteMinigame += OnCompleteMinigame;
		m_currentMinigame.OnFailMinigame += OnFailMinigame;
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
}

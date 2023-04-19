using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
	public delegate void LevelEvent();
	public LevelEvent OnCompleteLevel;
	public LevelEvent OnFailLevel;
	public Vector2 TopLeft = new Vector2(-30, 15);
	public Vector2 BottomRight = new Vector2(30, -15);
	public Camera MainCamera;
	public PlayerController Player;
	[SerializeField] private Vector3 initialPlayerPosition = new Vector3(-7, -2, 10);

	[SerializeField] private GameObject[] m_enemyPrefabs;

	[SerializeField] private AudioSource[] audioSources;

	[SerializeField] private GameObject healthPackPrefab;

	[SerializeField] private GameObject m_enemyContainer;
	[SerializeField] private TMP_Text timerText;

	[SerializeField] private bool m_killAllEnemies = false;

	[SerializeField] private bool m_customSniperDelay = false;
	[SerializeField] private float m_customSniperDelayValue = 4f;
	[SerializeField] private GameObject m_healthContainer;

	private float m_spawnTimer = 0f;
	private float m_gameTime = 0f;
	private bool m_spawning = false;

	private int enemiesSpawned = 0;
	private const int healthPackRate = 5;

	private int initialEnemies = 0;
	private float m_timer;

	public void InitializeLevel(PlayerController player, float timer, Camera mainCamera)
	{
		MainCamera = mainCamera;
		Player = player;
		m_timer = timer;
		player.transform.position = initialPlayerPosition;
	}

	private void Start()
	{
		Time.timeScale = 1f;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;
		m_spawnTimer = 2.0f;

		initialEnemies = m_enemyContainer.transform.childCount;
		// Game start, initialize preexisting enemies
		for (int i = 0; i < m_enemyContainer.transform.childCount; i++)
		{
			m_enemyContainer.transform.GetChild(i).GetComponent<IEnemy>().Player = Player;

			SniperController sniperScript = m_enemyContainer.transform.GetChild(i).GetComponent<SniperController>();
			if (sniperScript != null && m_customSniperDelay)
			{
				sniperScript.SetWaitTime(m_customSniperDelayValue);
			}
		}

		if (m_healthContainer)
		{
			for (int i = 0; i < m_healthContainer.transform.childCount; i++)
			{
				m_healthContainer.transform.GetChild(i).GetComponent<HealthPackController>().player = Player;
			}
		}

		Player.OnHurt += OnPlayerHurt;
		Player.OnDeath += OnPlayerDie;
	}

	private void OnDestroy()
	{
		Player.OnHurt -= OnPlayerHurt;
		Player.OnDeath -= OnPlayerDie;
	}
	
	void FixedUpdate()
	{
		if (!Player || Player.Health <= 0)
		{
			return;
		}

		m_gameTime += Time.fixedDeltaTime;
		m_spawnTimer += Time.fixedDeltaTime;

		if (m_spawnTimer > Mathf.Lerp(4.5f, 2f, m_gameTime / 125f) && m_spawning)
		{
			m_spawnTimer = 0;
			SpawnRandomEnemy();

			enemiesSpawned++;
			if (enemiesSpawned % healthPackRate == 1)
			{
				SpawnHealthPack();
			}
		}

		if (m_killAllEnemies && m_enemyContainer.transform.childCount == 0 && !Player.m_isRestarting)
		{
			Debug.Log("Killed all enemies");
			//Player.NextLevel();
			OnCompleteLevel?.Invoke();
			return;
		}

		if (m_timer >= 0)
		{
			timerText.text = m_timer.ToString("0.0");
			m_timer -= Time.fixedDeltaTime;
			if (m_timer <= 0)
			{
				Debug.Log("Fail level invoke");
				Player.TakeDamage(10f);
				OnFailLevel?.Invoke();
			}
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

	private void SpawnRandomEnemy()
	{
		int randomIndex = Random.Range(0, m_enemyPrefabs.Length);
		GameObject enemy = Instantiate(m_enemyPrefabs[randomIndex], transform.position, Quaternion.identity);
		enemy.transform.position = new Vector2(Random.Range(TopLeft.x, BottomRight.x), Random.Range(BottomRight.y, TopLeft.y));
		enemy.GetComponent<IEnemy>().Player = Player;
	}

	private void SpawnHealthPack()
	{
		GameObject health = Instantiate(healthPackPrefab, transform.position, Quaternion.identity);
		health.transform.position = new Vector2(Random.Range(TopLeft.x, BottomRight.x), Random.Range(BottomRight.y, TopLeft.y));
		health.GetComponent<HealthPackController>().player = Player;
	}
}

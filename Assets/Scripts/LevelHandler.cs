using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
	public Vector2 TopLeft = new Vector2(-30, 15);
	public Vector2 BottomRight = new Vector2(30, -15);
	public Camera MainCamera;
	public PlayerController Player;

	[SerializeField] private GameObject[] m_enemyPrefabs;

	[SerializeField] private AudioSource[] audioSources;

	[SerializeField] private GameObject healthPackPrefab;

	[SerializeField] private GameObject m_enemyContainer;

	[SerializeField] private bool m_killAllEnemies = false;

	[SerializeField] private bool m_customSniperDelay = false;
	[SerializeField] private float m_customSniperDelayValue = 4f;

	private float m_spawnTimer = 0f;
	private float m_gameTime = 0f;
	private bool m_spawning = false;

	private int enemiesSpawned = 0;
	private const int healthPackRate = 5;

	private int initialEnemies = 0;


	private void Start()
	{
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

		Player.OnHurt += OnPlayerHurt;
		Player.OnDeath += OnPlayerDie;
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
			Player.NextLevel();
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
		float originalTimeScale = Time.timeScale;
		Vector3 originalCameraPosition = MainCamera.transform.position;

		while (timeElapsed < time)
		{
			timeElapsed += Time.deltaTime;
			Time.timeScale = Mathf.Lerp(originalTimeScale, 0.1f, timeElapsed / time);
			MainCamera.transform.position = originalCameraPosition + Random.insideUnitSphere * shakeAmount;
			yield return null;
		}

		Time.timeScale = originalTimeScale;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
	public Vector2 TopLeft = new Vector2(-30, 15);
	public Vector2 BottomRight = new Vector2(30, -15);
	public PlayerController Player;
	public Camera MainCamera;

	[SerializeField] private GameObject[] m_enemyPrefabs;

	[SerializeField] private AudioSource[] audioSources;

	[SerializeField] private GameObject healthPackPrefab;

	private float m_spawnTimer = 0f;
	private float m_gameTime = 0f;

	private int enemiesSpawned = 0;
	private const int healthPackRate = 5;

	private void Start()
	{
		Time.timeScale = 1f;
		m_spawnTimer = 2.0f;
		Player.OnHurt += OnPlayerHurt;
		Player.OnDeath += OnPlayerDie;
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

	
	void FixedUpdate()
	{
		if (!Player || Player.Health <= 0)
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

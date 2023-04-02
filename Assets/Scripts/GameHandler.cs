using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHandler : MonoBehaviour
{
	public Vector2 TopLeft = new Vector2(-30, 15);
	public Vector2 BottomRight = new Vector2(30, -15);
	public PlayerController Player;

	[SerializeField] private GameObject[] m_enemyPrefabs;

	[SerializeField] private AudioSource[] audioSources;

	[SerializeField] private GameObject healthPackPrefab;

	private float m_spawnTimer = 0f;
	private float m_gameTime = 0f;

	private int enemiesSpawned = 0;
	private const int healthPackRate = 5;

	private void Start()
	{
		m_spawnTimer = 2.0f;
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
		}
	}

	private void SpawnRandomEnemy()
	{
		int randomIndex = Random.Range(0, m_enemyPrefabs.Length);
		GameObject enemy = Instantiate(m_enemyPrefabs[randomIndex], transform.position, Quaternion.identity);
		enemy.transform.position = new Vector2(Random.Range(TopLeft.x, BottomRight.x), Random.Range(BottomRight.y, TopLeft.y));
		enemy.GetComponent<IEnemy>().Player = Player;

		if (randomIndex == 1)
		{
			enemy.GetComponent<ShotgunEnemyCompoennt>().shotgunSound = audioSources[0];
		}
	}

	private void SpawnHealthPack()
	{
		GameObject health = Instantiate(healthPackPrefab, transform.position, Quaternion.identity);
		health.transform.position = new Vector2(Random.Range(TopLeft.x, BottomRight.x), Random.Range(BottomRight.y, TopLeft.y));
		health.GetComponent<HealthPackController>().player = Player;
	}
}

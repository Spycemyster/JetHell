using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingBulletComponent : MonoBehaviour
{
	public PlayerController Player;
	[SerializeField] private GameObject m_explosionPrefab;
	[SerializeField] private float m_speed = 5f;

	private Rigidbody2D rb;

	void Start()
	{
		StartCoroutine(DestroyAfterTime(35f));

		rb = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		if (!Player)
			return;
		
		Vector2 direction = Player.transform.position - transform.position;
		direction = direction.normalized;
		//transform.position += (Vector3)direction * m_speed * Time.deltaTime;
		rb.velocity = direction * m_speed;
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<PlayerBulletController>() != null)
		{
			Instantiate(m_explosionPrefab, transform.position, Quaternion.identity);
			other.gameObject.GetComponent<PlayerBulletController>().DealDamage(1);
			Destroy(gameObject);
		}
		else if (other.gameObject.GetComponent<PlayerController>() != null)
		{
			Instantiate(m_explosionPrefab, transform.position, Quaternion.identity);
			PlayerController playerScript = other.gameObject.GetComponent<PlayerController>();
			if (!playerScript.isInvincible()) {
				playerScript.TakeDamage(1f);
				Destroy(gameObject);
			}
		}
		else if (other.gameObject.CompareTag("Wall"))
		{
			Instantiate(m_explosionPrefab, transform.position, Quaternion.identity);
			Destroy(gameObject);
			//DestroyAfterTime(.05f);
		}
	}

	IEnumerator DestroyAfterTime(float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(gameObject);
	}
}

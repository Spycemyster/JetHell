using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearBulletComponent : MonoBehaviour
{
	public Vector2 Velocity;

	[SerializeField] private GameObject m_explosionPrefab;
	private Rigidbody2D rb;
	
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterTime(10f));

		rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		rb.velocity = Velocity;
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<PlayerBulletController>() != null)
		{
			Instantiate(m_explosionPrefab, transform.position, Quaternion.identity);
			other.gameObject.GetComponent<PlayerBulletController>().DealDamage(1);
			//Destroy(gameObject);
			DestroyedByPlayer();
		}
		else if (other.gameObject.GetComponent<PlayerController>() != null)
		{			
			PlayerController playerScript = other.gameObject.GetComponent<PlayerController>();
			if (!playerScript.isInvincible()) {
				Instantiate(m_explosionPrefab, transform.position, Quaternion.identity);
				playerScript.TakeDamage(1f);
			}
			Destroy(gameObject);
		}
		else if (other.gameObject.CompareTag("Wall"))
		{
			Instantiate(m_explosionPrefab, transform.position, Quaternion.identity);
			Destroy(gameObject);
		}
	}
	public void DestroyedByPlayer()
	{
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		player.GetComponent<PlayerController>().DestroyedValue(.1f);
		Destroy(gameObject);
	}

	private IEnumerator DestroyAfterTime(float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(gameObject);
	}
}

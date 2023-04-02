using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingBulletComponent : MonoBehaviour
{
	public PlayerController Player;
	[SerializeField] private float m_speed = 5f;

	void Start()
	{
		StartCoroutine(DestroyAfterTime(35f));
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		if (!Player)
			return;
		
		Vector2 direction = Player.transform.position - transform.position;
		direction = direction.normalized;
		transform.position += (Vector3)direction * m_speed * Time.deltaTime;
    }

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.GetComponent<PlayerBulletController>() != null)
		{
			other.gameObject.GetComponent<PlayerBulletController>().DealDamage(1);
			Destroy(gameObject);
		}
		else if (other.gameObject.GetComponent<PlayerController>() != null)
		{
			PlayerController playerScript = other.gameObject.GetComponent<PlayerController>();
			if (!playerScript.isInvincible()) {
				playerScript.TakeDamage(1f);
				Destroy(gameObject);
			}
		}
	}

	IEnumerator DestroyAfterTime(float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(gameObject);
	}
}

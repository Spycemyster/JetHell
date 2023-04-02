using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearBulletComponent : MonoBehaviour
{
	public Vector2 Velocity;
	
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyAfterTime(10f));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += (Vector3)Velocity * Time.deltaTime;
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

	private IEnumerator DestroyAfterTime(float time)
	{
		yield return new WaitForSeconds(time);
		Destroy(gameObject);
	}
}

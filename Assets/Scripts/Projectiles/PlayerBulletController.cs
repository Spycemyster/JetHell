using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletController : MonoBehaviour
{
	[SerializeField] private GameObject m_explosionPrefab;
	private int m_health = 2;
    private float timeAlive;
    private const float timeAliveMax = 5f;
	public void DealDamage(int delta)
	{
		m_health -= delta;
		if (m_health <= 0)
		{
			Destroy(gameObject);
		}
	}

    // Start is called before the first frame update
    void Start()
    {
        timeAlive = 0f;
    }

	void OnDestroy()
	{
		Instantiate(m_explosionPrefab, transform.position, Quaternion.identity);
	}

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;

        if (timeAlive > timeAliveMax)
        {
            Destroy(gameObject);
        }
    }

	void OnTriggerEnter2D(Collider2D other)
    {
		if (other.gameObject.CompareTag("Wall"))
		{
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletController : MonoBehaviour
{
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

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;

        if (timeAlive > timeAliveMax)
        {
            Destroy(gameObject);
        }
    }
}

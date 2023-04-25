using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
	[SerializeField] private GameObject m_deathExplosionPrefab;
	[SerializeField] private AudioClip[] m_deathSounds;
    private Vector3 m_localScale;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        m_localScale = transform.localScale;

        sr = GetComponent<SpriteRenderer>();
    }

	/*void OnDestroy()
	{
		Instantiate(m_deathExplosionPrefab, transform.position, Quaternion.identity);
		AudioSource.PlayClipAtPoint(m_deathSounds[Random.Range(0, m_deathSounds.Length)], transform.position);
	}*/

    void OnKilled()
    {
        Instantiate(m_deathExplosionPrefab, transform.position, Quaternion.identity);
        if (m_deathSounds.Length > 0)
        {
		    AudioSource.PlayClipAtPoint(m_deathSounds[Random.Range(0, m_deathSounds.Length)], transform.position);
        }
    }

    public void SetHealthEnemy(float healthPercentage)
    {
        float clamped = Mathf.Clamp(healthPercentage, 0, 1f);
        m_localScale.x = clamped;
        
        if (clamped < 1f && !sr.enabled)
        {
            sr.enabled = true;
        }
        transform.localScale = m_localScale;

        if (healthPercentage == 0)
        {
            OnKilled();
        }
    }
}

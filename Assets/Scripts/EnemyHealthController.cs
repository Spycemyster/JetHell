using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{
    private Vector3 m_localScale;
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        m_localScale = transform.localScale;

        sr = GetComponent<SpriteRenderer>();
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
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthController : MonoBehaviour
{
    [SerializeField] private GameObject healthPrefab;

    private int health;
    private int maxHealth = 10;
    private Vector2 originPoint = new Vector2(-106, 0);
    private Vector3 healthBitScale = new Vector3(24, 8, 1); //new Vector3(24, 12, 1);
    private float offset = 30f;

    private GameObject[] healthBits = new GameObject[1];
    private bool healthIsZero = false;

    // Start is called before the first frame update
    void Start()
    {
    }
    
    public void SetHealth(int setHealth)
    {
        health = setHealth;
        if (health <= 0)
        {
            healthIsZero = true;
        }
        DisplayHealth();
    }

    public void DisplayHealth()
    {
        if (healthBits != null)
        {
            foreach (GameObject obj in healthBits)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }
        if (healthIsZero) return;

        healthBits = new GameObject[health];
        Vector2 healthPos = originPoint;
        for (int i = 0; i < health; i++)
        {
            healthBits[i] = Instantiate(healthPrefab, healthPos, Quaternion.identity, transform);
            healthBits[i].transform.localPosition = healthPos;
            healthBits[i].transform.localScale = healthBitScale;
            healthPos += new Vector2(offset, 0);
            healthBits[i].SetActive(true);
        }
    }


}

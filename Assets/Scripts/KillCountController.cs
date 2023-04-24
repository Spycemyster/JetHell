using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KillCountController : MonoBehaviour
{
    [SerializeField] private GameObject killTickPrefab;

    private int killCount;
    private Vector2 originPoint = new Vector2(0, 0);
    private Vector3 healthBitScale = new Vector3(10, 10, 1);
    private float offset = 12f;
    private float extraOffset = 2f;
    private int extraOffsetCount = 5;

    private GameObject[] killTicks = new GameObject[1];
    private bool killIsZero = true;

    private TextMeshProUGUI killsText;

    // Start is called before the first frame update
    void Start()
    {
        killsText = GetComponent<TextMeshProUGUI>();
    }

    public void SetKills(int kills)
    {
        killCount = kills;
        killIsZero = (killCount == 0);
        DisplayKillsDiscrete();

    }

    public void DisplayKills()
    {
        if (killTicks != null)
        {
            foreach (GameObject obj in killTicks)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }
        if (killIsZero) return;

        killTicks = new GameObject[killCount];
        Vector2 healthPos = originPoint;
        for (int i = 0; i < killCount; i++)
        {
            killTicks[i] = Instantiate(killTickPrefab, healthPos, Quaternion.identity, transform);
            killTicks[i].transform.localPosition = healthPos;
            killTicks[i].transform.localScale = healthBitScale;

            float finalOffset = offset;
            if (i % extraOffsetCount == extraOffsetCount - 1)
            {
                finalOffset += extraOffset;
            }
            healthPos += new Vector2(finalOffset, 0);
            killTicks[i].SetActive(true);
        }
    }

    public void DisplayKillsDiscrete()
    {
        killsText.text = "Kills: " + killCount;
    }


}

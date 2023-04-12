using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] private GameObject ammoPrefab;

    private int itemCount;
    private int maxHealth = 10;
    private Vector2 originPoint = new Vector2(0, 0);
    private Vector3 bitScale = new Vector3(15f, 15f, 1f);
    private Vector2 offset = new Vector2(0, 20f);

    private GameObject[] ammoBits = new GameObject[1];
    private bool isZero = false;

    // Start is called before the first frame update
    void Start()
    {
    }
    
    public void SetAmmo(int setAmmo)
    {
        itemCount = setAmmo;
        if (itemCount <= 0)
        {
            isZero = true;
        }
        DisplayAmmo();
    }

    public void DisplayAmmo()
    {
        if (!ammoPrefab)
        {
            return;
        }
        
        if (ammoBits != null)
        {
            foreach (GameObject obj in ammoBits)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
        }
        if (isZero) return;

        ammoBits = new GameObject[itemCount];
        Vector2 pos = originPoint;
        for (int i = 0; i < itemCount; i++)
        {
            ammoBits[i] = Instantiate(ammoPrefab, pos, Quaternion.identity, transform);
            ammoBits[i].transform.localPosition = pos;
            ammoBits[i].transform.localScale = bitScale;
            pos += offset;
            ammoBits[i].SetActive(true);
        }
    }
}

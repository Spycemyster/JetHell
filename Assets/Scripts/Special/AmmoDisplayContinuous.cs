using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDisplayContinuous : MonoBehaviour
{
    private float currentAmmoPercentage;
    private float maxLength;

    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        maxLength = transform.localScale.x;
    }
    
    public void SetAmmo(float setAmmo)
    {
        currentAmmoPercentage = Mathf.Clamp(setAmmo, 0, 1f);
        DisplayAmmo();
    }

    public void DisplayAmmo()
    {
        Vector3 scale = transform.localScale;
        scale.x = maxLength * currentAmmoPercentage;
        transform.localScale = scale;
    }
}

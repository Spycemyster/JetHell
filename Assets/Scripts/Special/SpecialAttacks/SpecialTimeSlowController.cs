using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTimeSlowController : MonoBehaviour, ISpecial
{
	public PlayerController Player
	{
		get;
		set;
	}
	public SpecialHandler specialHandler
	{
		get;
		set;
	}

    private int ammo;

    private bool isActive = false;
    private const float fireDuration = 5f;
    private float fireTime = 0f;
    private float reducedTimeScale = .5f;


    public void SetSpecial()
    {
        ammo = 2;
    }

    public void FireSpecial()
    {
        fireTime = 0f;
        isActive = true;
        Time.timeScale = reducedTimeScale;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;
    }

    public bool OutOfAmmo()
    {
        
        return ammo == 0;
    }

    public void Update()
    {
        if (isActive)
        {
            fireTime += Time.deltaTime;
            if (fireTime > fireDuration)
            {
                Time.timeScale = 1f;
		        Time.fixedDeltaTime = 0.02f * Time.timeScale;
            }
        }
    }

    public void DestroyedValue(float value)
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialBeamController : MonoBehaviour, ISpecial
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


    public void SetSpecial()
    {

    }

    public void FireSpecial()
    {

    }

    public bool OutOfAmmo()
    {
        
        return ammo == 0;
    }
}

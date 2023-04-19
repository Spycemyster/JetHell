using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpecial
{
    PlayerController Player {get; set;}
    SpecialHandler specialHandler {get; set;}
    public void SetSpecial();
    public void FireSpecial();
    public bool OutOfAmmo();
    public void DestroyedValue(float value);
}

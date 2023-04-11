using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialHandler : MonoBehaviour
{
    [SerializeField] private PlayerController Player;
	[SerializeField] public GameObject[] m_specials;

    public enum Specials
    {
        NONE=-1, SHOTGUN=0, BEAM=1
    }

    private GameObject m_currentSpecial;
    private ISpecial m_specialScript;

    void Start()
    {
        foreach (GameObject specialObject in m_specials)
        {
            ISpecial specialScript = specialObject.GetComponent<ISpecial>();
            specialScript.Player = Player;
        }
    }

    public GameObject GetSpecial(int idx)
    {
        return m_specials[idx];
    }

    public void SetSpecial(int idx)
    {
        m_currentSpecial = m_specials[idx];
        m_specialScript = m_currentSpecial.GetComponent<ISpecial>();

        m_specialScript.SetSpecial();
    }

    public void FireSpecial()
    {
        m_specialScript.FireSpecial();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialHandler : MonoBehaviour
{
    [SerializeField] private PlayerController Player;
	[SerializeField] public GameObject[] m_specials;

    [SerializeField] public GameObject m_specialPickupPrefab;
    [SerializeField] public GameObject m_ammoDisplay;

	public Vector2 TopLeft = new Vector2(-30, 15);
	public Vector2 BottomRight = new Vector2(30, -15);

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
            specialScript.specialHandler = this;
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

    public void SpawnSpecialPickup(int idx)
    {
		GameObject health = Instantiate(m_specialPickupPrefab, transform.position, Quaternion.identity);
		health.transform.position = new Vector2(Random.Range(TopLeft.x, BottomRight.x), Random.Range(BottomRight.y, TopLeft.y));
		SpecialPickupHandler specialPickupHandler = health.GetComponent<SpecialPickupHandler>();
        specialPickupHandler.player = Player;
        specialPickupHandler.specialItem = idx;
    }

    public void SetAmmo(int ammo)
    {
        if (m_ammoDisplay)
        {
            m_ammoDisplay.GetComponent<AmmoDisplay>().SetAmmo(ammo);
        }
    }
}

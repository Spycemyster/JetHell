using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialHandler : MonoBehaviour
{
    [SerializeField] private PlayerController Player;
	[SerializeField] public GameObject[] m_specials;

    [SerializeField] public GameObject m_specialPickupPrefab;
    [SerializeField] public GameObject m_ammoDisplay;

    [SerializeField] private bool m_spawnPickup = false;
    private const float m_spawnDelay = 5f;
    private float m_spawnDelayTimer = 0f;

	public Vector2 TopLeft = new Vector2(-30, 15);
	public Vector2 BottomRight = new Vector2(30, -15);

    private float currentAmmoPercentage;

    public enum Specials
    {
        NONE=-1, SHOTGUN=0, BEAM=1
    }

    private GameObject m_currentSpecial;
    private ISpecial m_specialScript;
    private bool m_hasSpecial;

    void Start()
    {
        foreach (GameObject specialObject in m_specials)
        {
            ISpecial specialScript = specialObject.GetComponent<ISpecial>();
            specialScript.Player = Player;
            specialScript.specialHandler = this;
        }
    }

    void Update()
    {
        if (m_spawnPickup)
        {
            m_spawnDelayTimer += Time.deltaTime;

            if (m_spawnDelayTimer > m_spawnDelay)
            {
                m_spawnDelayTimer = 0f;
                SpawnSpecialPickup();
            }
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
        m_specialScript.specialHandler = this;

        m_specialScript.SetSpecial();

        m_hasSpecial = true;
    }

    public void FireSpecial()
    {
        if (m_hasSpecial)
        {
            m_specialScript.FireSpecial();
        }
    }

    public void SpawnSpecialPickup(int idx = 0)
    {
		GameObject health = Instantiate(m_specialPickupPrefab, transform.position, Quaternion.identity);
		health.transform.position = new Vector2(Random.Range(TopLeft.x, BottomRight.x), Random.Range(BottomRight.y, TopLeft.y));
		SpecialPickupHandler specialPickupHandler = health.GetComponent<SpecialPickupHandler>();
        specialPickupHandler.player = Player;
        specialPickupHandler.specialItem = idx;
    }

    public void SetAmmo(float ammo)
    {
        if (m_ammoDisplay)
        {
            //m_ammoDisplay.GetComponent<AmmoDisplay>().SetAmmo(ammo);
            m_ammoDisplay.GetComponent<AmmoDisplayContinuous>().SetAmmo(ammo);
        }
    }

    public void RemoveSpecial()
    {
        m_currentSpecial = null;
        m_specialScript = null;
        m_hasSpecial = false;
    }

    public void DestroyedValue(float value)
    {
        if (m_specialScript != null)
        {
            m_specialScript.DestroyedValue(value);
        }
    }
}

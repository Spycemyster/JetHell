using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackIndicatorController : MonoBehaviour
{
    public GameObject parent;

    public delegate void AttackEvent(GameObject attackIndicator);
    public AttackEvent Attack;

    [SerializeField] public GameObject attackArea;
    public float attackRadius;

    [SerializeField] private GameObject attackLoad;

    private float attackTimeWait;
    private bool doSendAttack;
    private float timeElapsed = 0f;

    public void InitializeIndicator(float attackRadius, float attackTime, bool sendAttack, AttackEvent attack, GameObject parent)
    {
        attackLoad.transform.localScale = Vector2.zero;
        transform.localScale = new Vector2(attackRadius*2, attackRadius*2);

        this.attackRadius = attackRadius;
        this.attackTimeWait = attackTime;
        this.doSendAttack = sendAttack;
        Attack = attack;

        this.parent = parent;
    }

    public void SetLoad(float percentage)
    {
        attackLoad.transform.localScale = new Vector2(percentage, percentage);
    }
    
    void FixedUpdate()
    {
        if (doSendAttack)
        {
            timeElapsed += Time.fixedDeltaTime;

            SetLoad(timeElapsed / attackTimeWait);
            if (timeElapsed > attackTimeWait)
            {
                Attack(gameObject);
            }
        }

        transform.position = parent.transform.position;
    }

}

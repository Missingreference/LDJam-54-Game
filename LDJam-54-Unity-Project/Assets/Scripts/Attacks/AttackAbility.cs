using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class AttackAbility : MonoBehaviour
{
    public DamageTrigger damageTrigger { get; private set; }
    public float attackSpeed { get; set; } = 2.0f;
    public Vector2 target { get; private set; } = Vector2.zero;

    public float currentAttackTime { get; private set; } = 0.0f;

    public Action onAttackComplete;

    private bool m_IsAttacking = false;

    protected virtual void Awake()
    {
        GameObject triggerObject = new GameObject("Damage Trigger");
        triggerObject.transform.SetParent(transform);

        damageTrigger = triggerObject.AddComponent<DamageTrigger>();
        damageTrigger.gameObject.SetActive(false);
        damageTrigger.transform.localPosition = Vector3.zero;
        damageTrigger.transform.localEulerAngles = Vector3.zero;
        damageTrigger.transform.localScale = Vector3.one;

    }

    protected virtual void Start()
    {

    }

    
    public void Attack(Vector2 target)
    {
        if(currentAttackTime > 0.0f)
        {
            Reset();
        }

        this.target = target;
        m_IsAttacking = true;
        InitiateAttack();
    }

    protected virtual void InitiateAttack()
    {

    }
    
    protected virtual void Update()
    {
        if(currentAttackTime >= 1.0f)
        {
            //Attack complete
            Reset();

            onAttackComplete?.Invoke();
        }

        if(!m_IsAttacking)
        {
            return;
        }

        currentAttackTime += attackSpeed * Time.deltaTime;

        //Delay attack finish one frame
        if(currentAttackTime >= 1.0f)
        {
            currentAttackTime = 1.0f;
        }

        AnimateAttack();
    }

    protected virtual void AnimateAttack()
    {

    }

    public void Reset()
    {
        damageTrigger.gameObject.SetActive(false);
        //Does not sync correctly with Collider
        /*
        damageTrigger.transform.localPosition = Vector3.zero;
        damageTrigger.transform.localEulerAngles = Vector3.zero;
        damageTrigger.transform.localScale = Vector3.one;
        */

        damageTrigger.ClearDamagedActors();

        currentAttackTime = 0.0f;
        target = Vector2.zero;

        m_IsAttacking = false;

        OnReset();
    }

    protected virtual void OnReset()
    {

    }
}

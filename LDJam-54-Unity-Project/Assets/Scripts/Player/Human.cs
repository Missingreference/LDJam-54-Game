using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Elanetic.Tools;

public class Human : Actor
{
    //Hover
    public float spriteHoverSpeed = 0.25f;
    public float spriteHoverDistance = 0.05f;

    //Attack
    public float attackSwipeSpeed { get; set; } = 7.0f;
    public float attackSwipeWidth { get; set; } = 0.5f;
    public float attackSwipeDistance { get; set; } = 1.0f;
    public float attackSwipeRotation { get; set; } = 80.0f;

    //Damage Flash
    public float damageFlashTime { get; private set; } = 0.2f;
    public int damageFlashCount { get; private set; } = 3;

    //Colliders
    public BoxCollider2D bodyTrigger { get; private set; }
    public BoxCollider2D movementCollider { get; private set; }


    private Sprite m_MainSprite;

    private float m_HoverTimer = 0.0f;
    private float m_DamageFlashTimer = 0.0f;
    private float m_DamageFlashCount = 0;

    private List<DamageTrigger> m_AttackTriggers = new List<DamageTrigger>();
    private List<DamageTrigger> m_PooledAttackTriggers = new List<DamageTrigger>();
    private List<float> m_AttackSwipeTimers = new List<float>();
    private List<float> m_AttackDirections = new List<float>();

    protected override void Awake()
    {
        base.Awake();

        gameObject.layer = 7; //Player Movement

        m_MainSprite = Resources.Load<Sprite>("Sprites/Character/Character1");
        spriteRenderer.sprite = m_MainSprite;
        spriteRenderer.gameObject.layer = 6; //Player Body

        bodyTrigger = spriteRenderer.gameObject.AddComponent<BoxCollider2D>();
        bodyTrigger.isTrigger = true;
        bodyTrigger.size = (spriteRenderer.bounds.extents * 2.0f) * 0.85f;

        movementCollider = gameObject.AddComponent<BoxCollider2D>();
        movementCollider.isTrigger = false;
        movementCollider.size = (spriteRenderer.bounds.extents * 2.0f) * 0.9f;
    }


    protected override void Update()
    {
        AnimateHover();

        if(m_AttackTriggers.Count > 0)
        {
            AnimateAttack();
        }

        if(m_DamageFlashCount > 0)
        {
            AnimateDamageFlash();
        }
    }

    private void AnimateHover()
    {
        m_HoverTimer += spriteHoverSpeed * Time.deltaTime;

        if(m_HoverTimer > 1.0f)
        {
            m_HoverTimer %= 1.0f;
        }

        float percent;
        if(m_HoverTimer < 0.5f)
        {
            percent = m_HoverTimer * 2.0f;
        }
        else
        {
            percent = 1.0f - ((m_HoverTimer - 0.5f) * 2.0f);
        }

        float height = Easing.Sinusoidal.InOut(spriteHoverDistance * -0.25f, spriteHoverDistance * 0.75f, percent);

        spriteRenderer.transform.localPosition = new Vector3(spriteRenderer.transform.localPosition.x, height, spriteRenderer.transform.localPosition.z);
    }

    private void AnimateDamageFlash()
    {
        m_DamageFlashTimer += Time.deltaTime;
        if(m_DamageFlashTimer >= damageFlashTime)
        {
            m_DamageFlashTimer -= damageFlashTime;
            m_DamageFlashCount--;
            if(m_DamageFlashCount == 0)
            {
                m_DamageFlashTimer = 0.0f;
            }
        }

        if(m_DamageFlashCount > 0)
        {
            float percent = m_DamageFlashTimer / damageFlashTime;
            if(percent < 0.5f)
            {
                percent *= 1.0f;
                spriteRenderer.color = Easing.Linear.InOut(Color.white, Color.red, percent);
            }
            else
            {
                percent *= 0.5f;
                spriteRenderer.color = Easing.Linear.InOut(Color.red, Color.white, percent);
            }
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    private void AnimateAttack()
    {
        for(int i = 0; i < m_AttackSwipeTimers.Count; i++)
        {
            if(m_AttackSwipeTimers[i] >= 1.0f)
            {
                m_AttackSwipeTimers.RemoveAt(i);
                DamageTrigger trigger = m_AttackTriggers[i];
                trigger.gameObject.SetActive(false);
                m_AttackTriggers.RemoveAt(i);
                m_AttackDirections.RemoveAt(i);
                m_PooledAttackTriggers.Add(trigger);
                trigger.onDealDamage -= DealDamage;
                i--;
                continue;
            }

            float time = m_AttackSwipeTimers[i];
            time += attackSwipeSpeed * Time.deltaTime;
            if(time >= 1.0f) //Delay removal a frame
            {
                time = 1.0f;
            }
            m_AttackSwipeTimers[i] = time;
            DamageTrigger damageTrigger = m_AttackTriggers[i];

            float direction = m_AttackDirections[i];

            if(direction < 0)
            {
                float startAngle = direction + (attackSwipeRotation * 0.5f);
                damageTrigger.gameObject.transform.localEulerAngles = new Vector3(0.0f, 0.0f, startAngle - (time * attackSwipeRotation));
            }
            else
            {
                float startAngle = direction - (attackSwipeRotation * 0.5f);
                damageTrigger.gameObject.transform.localEulerAngles = new Vector3(0.0f, 0.0f, startAngle + (time * attackSwipeRotation));
            }



        }


    }

    public override void TakeDamage(int damage)
    {
        m_DamageFlashCount = damageFlashCount;
    }

    public void Attack()
    {
        DamageTrigger damageTrigger;
        BoxCollider2D trigger;
        if(m_PooledAttackTriggers.Count > 0)
        {   
            damageTrigger = m_PooledAttackTriggers[m_PooledAttackTriggers.Count - 1];
            damageTrigger.gameObject.SetActive(true);
            trigger = (BoxCollider2D)damageTrigger.trigger;
            damageTrigger.damagedActors.Clear();

            m_PooledAttackTriggers.RemoveAt(m_PooledAttackTriggers.Count - 1);
        }
        else
        {
            GameObject damageTriggerObject = new GameObject("Damage Trigger");
            damageTriggerObject.transform.parent = transform;
            damageTriggerObject.transform.localPosition = Vector3.zero;
            damageTriggerObject.transform.localEulerAngles = Vector3.zero;
            damageTriggerObject.transform.localScale = Vector3.one;

            damageTrigger = damageTriggerObject.AddComponent<DamageTrigger>();
            trigger = damageTrigger.CreateTrigger<BoxCollider2D>();

            damageTrigger.trigger.callbackLayers = 1 << 8;
        }

        m_AttackTriggers.Add(damageTrigger);
        m_AttackSwipeTimers.Add(0.0f);
        if(spriteRenderer.flipX)
        {
            m_AttackDirections.Add(90.0f);
        }
        else
        {
            m_AttackDirections.Add(-90.0f);
        }
        trigger.size = new Vector2(attackSwipeWidth, attackSwipeDistance);
        trigger.offset = new Vector2(0, trigger.size.y * 0.5f);

        damageTrigger.onDealDamage += DealDamage;


    }

    protected override void OnMove()
    {
        rigidbody.AddForce(moveDirection * Time.fixedDeltaTime * moveSpeed, ForceMode2D.Force);

        if(moveDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if(moveDirection.x > 0)
        {
            spriteRenderer.flipX = false;
        }
    }


    private void DealDamage(Actor target)
    {
        target.TakeDamage(5);
    }
}

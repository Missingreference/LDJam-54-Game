using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Elanetic.Tools;

public abstract class Enemy : Actor
{
    public float damageFlashTime { get; private set; } = 0.25f;
    public int damageFlashCount { get; private set; } = 1;


    protected Actor m_Enemy;


    private float m_DamageFlashTimer = 0.0f;
    private float m_DamageFlashCount = 0;

    protected override void Awake()
    {
        base.Awake();

        gameObject.layer = 9;
        spriteRenderer.gameObject.layer = 8;
    }

    protected virtual void Start()
    {
        FindEnemy();
    }

    protected override void OnMove()
    {
        base.OnMove();
    }

    public override void TakeDamage(int damage)
    {
        PlayHitSound();
        base.TakeDamage(damage);

        if(m_DamageFlashCount > 0)
        {
            m_DamageFlashTimer = 0.0f;
        }

        m_DamageFlashCount = damageFlashCount;
    }

    protected override void Update()
    {
        base.Update();

        if(m_DamageFlashCount > 0)
        {
            AnimateDamageFlash();
        }
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

    protected override void OnDeath()
    {
        m_DamageFlashCount = 0;
    }

    protected void FindEnemy()
    {
        m_Enemy = FindObjectOfType<Human>();
    }

}

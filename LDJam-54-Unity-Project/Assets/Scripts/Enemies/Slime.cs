using Elanetic.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{

    public float damageFlashTime { get; private set; } = 0.25f;
    public int damageFlashCount { get; private set; } = 1;

    public float moveWaitTime { get; private set; } = 1.0f;

    public CircleCollider2D movementCollider;
    public CircleCollider2D bodyCollider;

    private float m_DamageFlashTimer = 0.0f;
    private float m_DamageFlashCount = 0;

    private float m_MoveWaitTimer = 0.0f;

    private Sprite m_Sprite;

    private Actor m_Enemy;

    private DamageTrigger m_DamageTrigger;

    protected override void Awake()
    {
        base.Awake();

        gameObject.layer = 9;
        m_Sprite = Resources.Load<Sprite>("Sprites/Enemy/Slime_Jim");
        spriteRenderer.sprite = m_Sprite;
        spriteRenderer.gameObject.layer = 8;

        moveSpeed = 5.0f;

        GameObject damageTriggerObject = new GameObject("Damage Trigger");
        damageTriggerObject.transform.parent = transform;
        damageTriggerObject.transform.localPosition = Vector3.zero;
        damageTriggerObject.transform.localEulerAngles = Vector3.zero;
        damageTriggerObject.transform.localScale = Vector3.one;

        movementCollider = gameObject.AddComponent<CircleCollider2D>();
        movementCollider.isTrigger = false;
        movementCollider.radius = 0.2f;

        bodyCollider = spriteRenderer.gameObject.AddComponent<CircleCollider2D>();
        bodyCollider.isTrigger = false;
        bodyCollider.radius = 0.225f;

        m_DamageTrigger = damageTriggerObject.AddComponent<DamageTrigger>();

        CircleCollider2D damageCollider = m_DamageTrigger.CreateTrigger<CircleCollider2D>();
        damageCollider.radius = 0.225f;
        damageCollider.callbackLayers = 1 << 6;

        m_DamageTrigger.persistentDamage = true;
        m_DamageTrigger.persistentDamageTime = 0.85f;
        m_DamageTrigger.onDealDamage += DealDamage;


    }

    public override void TakeDamage(int damage)
    {
        if(m_DamageFlashCount > 0)
        {
            m_DamageFlashTimer = 0.0f;
        }

        m_DamageFlashCount = damageFlashCount;

        base.TakeDamage(damage);
    }

    protected override void Update()
    {
        base.Update();

        if(m_DamageFlashCount > 0)
        {
            AnimateDamageFlash();
        }

        m_MoveWaitTimer += Time.deltaTime;
    }

    protected override void FixedUpdate()
    {
        //Basic AI, move towards enemy
        if(m_MoveWaitTimer > moveWaitTime)
        {
            if(m_Enemy == null || !m_Enemy.isAlive)
            {
                FindEnemy();
            }

            if(m_Enemy != null && m_Enemy.isAlive)
            {
                Move(m_Enemy.transform.position - transform.position);
            }
            else
            {
                Move(Vector2.zero);
            }
        }

        base.FixedUpdate();
    }

    private void FindEnemy()
    {
        m_Enemy = FindObjectOfType<Human>();
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

    protected override void OnMove()
    {
        if(m_MoveWaitTimer >= moveWaitTime)
        {
            rigidbody.AddForce(moveDirection * moveSpeed, ForceMode2D.Impulse);
            m_MoveWaitTimer -= moveWaitTime;
        }
    }

    private void DealDamage(Actor target)
    {
        target.TakeDamage(5);
    }

    protected override void OnDeath()
    {
        m_DamageFlashCount = 0;
        m_DamageTrigger.onDealDamage -= DealDamage;
        Destroy(m_DamageTrigger.gameObject);
        spriteRenderer.color = Color.black;
    }

}

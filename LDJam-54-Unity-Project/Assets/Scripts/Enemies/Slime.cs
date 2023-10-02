using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Elanetic.Tools;

public class Slime : Enemy
{
    static private Sprite[] m_IdleSprites;
    static private Sprite[] m_MoveSprites;
    static private Sprite[] m_ReverseMoveSprites;


    public float moveWaitTime { get; private set; } = 2.5f;

    public CircleCollider2D movementCollider { get; private set; }
    public CircleCollider2D bodyTrigger { get; private set; }

    public float idleAnimationTime { get; private set; } = 1.0f;
    public float rollAnimationTime { get; private set; } = 1.0f;

    private bool m_IsIdling = true;
    private bool m_AttackingLeft = true;
    private float m_AnimationTimer = 0.0f;

    private float m_MoveWaitTimer = 0.0f;

    private Sprite m_Sprite;

    private DamageTrigger m_DamageTrigger;

    protected override void Awake()
    {
        base.Awake();

        if(m_IdleSprites == null)
        {
            m_IdleSprites = Resources.LoadAll<Sprite>("Sprites/Enemy/Basic Slime/Slime_Jim_Idle_Sheet");
            m_MoveSprites = Resources.LoadAll<Sprite>("Sprites/Enemy/Basic Slime/Slime_Jim_Move_Sheet");
            m_ReverseMoveSprites = Resources.LoadAll<Sprite>("Sprites/Enemy/Basic Slime/Slime_Jim_Reverse_Move_Sheet");
        }
        //m_Sprite = Resources.Load<Sprite>("Sprites/Enemy/Slime_Jim");

        spriteRenderer.sprite = m_Sprite;

        moveSpeed = 10.0f;

        GameObject damageTriggerObject = new GameObject("Damage Trigger");
        damageTriggerObject.transform.parent = transform;
        damageTriggerObject.transform.localPosition = Vector3.zero;
        damageTriggerObject.transform.localEulerAngles = Vector3.zero;
        damageTriggerObject.transform.localScale = Vector3.one;

        movementCollider = gameObject.AddComponent<CircleCollider2D>();
        movementCollider.isTrigger = false;
        movementCollider.radius = 0.2f;
        movementCollider.offset = new Vector2(0.0f, 0.2f);


        rigidbody.drag = 5.0f;

        bodyTrigger = spriteRenderer.gameObject.AddComponent<CircleCollider2D>();
        bodyTrigger.isTrigger = true;
        bodyTrigger.radius = 0.225f;
        bodyTrigger.offset = new Vector2(0.0f, 0.2f);

        m_DamageTrigger = damageTriggerObject.AddComponent<DamageTrigger>();

        CircleCollider2D damageCollider = m_DamageTrigger.CreateTrigger<CircleCollider2D>();
        damageCollider.radius = 0.225f;
        damageCollider.callbackLayers = 1 << 6;

        m_DamageTrigger.gameObject.layer = 10;
        m_DamageTrigger.persistentDamage = true;
        m_DamageTrigger.persistentDamageTime = 0.85f;
        m_DamageTrigger.onDealDamage += DealDamage;


        float rand = Random.Range(-8f, 8f);
        moveSpeed += rand;
        rand = Random.Range(-1f, 0.8f);
        moveWaitTime += rand;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Update()
    {
        if(!isAlive) return;

        base.Update();

        m_MoveWaitTimer += Time.deltaTime;

        m_AnimationTimer += Time.deltaTime;

        const float prepareTime = 1.5f;

        if((m_IsIdling && m_MoveWaitTimer <= prepareTime) || m_Enemy == null | !m_Enemy.isAlive)
        {
            m_AnimationTimer %= idleAnimationTime;
            int frame = (int)((m_AnimationTimer / idleAnimationTime) * m_IdleSprites.Length);

            spriteRenderer.sprite = m_IdleSprites[frame];
        }
        else
        {
            if(m_AnimationTimer >= rollAnimationTime)
            {
                m_AnimationTimer = 0.0f;
                m_IsIdling = true;
                spriteRenderer.sprite = m_IdleSprites[0];
            }
            else
            {
                int frame;
                if(m_IsIdling)
                {

                    if(m_Enemy != null && m_Enemy.isAlive)
                    {
                        m_AttackingLeft = (m_Enemy.transform.position - transform.position).normalized.x < 0.0f;
                    }

                    float perc = ((m_MoveWaitTimer - prepareTime) / (moveWaitTime - prepareTime));
                    frame = (int)(4 * perc);
                    if(frame > 4) frame = 4;
                }
                else
                {
                    float percent = Easing.Ease(0.0f, 1.0f, (m_AnimationTimer / rollAnimationTime), Easing.EasingFunction.Linear);
                    frame = (int)(percent * m_MoveSprites.Length);
                }


                /*
                if(m_IsIdling && frame > 4)
                {
                    frame = 4;
                }
                */

                if(frame > m_MoveSprites.Length-1)
                {
                    frame = m_MoveSprites.Length-1;
                }

                if(m_AttackingLeft)
                {
                    spriteRenderer.sprite = m_ReverseMoveSprites[frame];
                }
                else
                {
                    spriteRenderer.sprite = m_MoveSprites[frame];
                }
            }
        }
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
                Move((m_Enemy.transform.position - transform.position).normalized);
            }
            else
            {
                Move(Vector2.zero);
                m_MoveWaitTimer = 0.0f;
            }
        }

        base.FixedUpdate();
    }

    protected override void OnMove()
    {
        if(m_MoveWaitTimer >= moveWaitTime)
        {
            rigidbody.AddForce(moveDirection * moveSpeed, ForceMode2D.Impulse);

            m_IsIdling = false;
            m_AnimationTimer = (5.0f / (float)m_MoveSprites.Length);
            if(moveDirection.x < 0.0f)
            {
                spriteRenderer.sprite = m_ReverseMoveSprites[5];
                m_AttackingLeft = true;
            }
            else
            {
                spriteRenderer.sprite = m_MoveSprites[5];
                m_AttackingLeft = false;
            }

            m_MoveWaitTimer -= moveWaitTime;
        }
    }

    private void DealDamage(Actor target)
    {
        target.TakeDamage(5);
    }

    protected override void OnDeath()
    {
        base.OnDeath();

        m_DamageTrigger.onDealDamage -= DealDamage;
        Destroy(m_DamageTrigger.gameObject);
        spriteRenderer.color = Color.black;

        Destroy(gameObject);
    }

}

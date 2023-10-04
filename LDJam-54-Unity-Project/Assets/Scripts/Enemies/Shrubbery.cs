using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Elanetic.Tools;

public class Shrubbery : Enemy
{

    static private Sprite[] m_MoveSprites;
    static private Sprite[] m_AttackSprites;

    public float moveAnimationTime { get; private set; } = 0.5f;
    public float attackAnimationTime { get; private set; } = 0.85f;

    public float attackCoolDownTime { get; private set; } = 3.0f;

    public CircleCollider2D movementCollider { get; private set; }
    public BoxCollider2D bodyTrigger { get; private set; }

    private float m_AnimationTimer = 0.0f;
    private bool m_IsIdling = true;
    private bool m_AttackingLeft = true;
    private float m_CooldownTimer = 0.0f;

    private DamageTrigger m_DamageTrigger;
    private CircleCollider2D m_AdditionalDamageTrigger;

    protected override void Awake()
    {
        base.Awake();



        if(m_MoveSprites == null)
        {
            m_MoveSprites = Resources.LoadAll<Sprite>("Sprites/Enemy/Shrubbery/Shrubbery_Move_Sheet");
            m_AttackSprites = Resources.LoadAll<Sprite>("Sprites/Enemy/Shrubbery/Shrubbery_Attack_Sheet");
        }

        bodyTrigger = spriteRenderer.gameObject.AddComponent<BoxCollider2D>();
        bodyTrigger.isTrigger = true;
        bodyTrigger.offset = new Vector2(0.0f, 0.2f);
        bodyTrigger.size = new Vector2(0.47f, 0.47f);

        movementCollider = gameObject.AddComponent<CircleCollider2D>();
        movementCollider.isTrigger = false;
        movementCollider.radius = 0.2f;
        movementCollider.offset = new Vector2(0.0f, 0.2f);

        GameObject damageTriggerObject = new GameObject("Damage Trigger");
        damageTriggerObject.transform.parent = transform;
        damageTriggerObject.transform.localPosition = Vector3.zero;
        damageTriggerObject.transform.localEulerAngles = Vector3.zero;
        damageTriggerObject.transform.localScale = Vector3.one;


        m_DamageTrigger = damageTriggerObject.AddComponent<DamageTrigger>();
        CircleCollider2D damageCollider = m_DamageTrigger.CreateTrigger<CircleCollider2D>();
        damageCollider.radius = 0.225f;
        damageCollider.offset = new Vector2(0.0f, 0.2f);
        damageCollider.callbackLayers = 1 << 6;
        m_DamageTrigger.persistentDamage = true;
        m_DamageTrigger.persistentDamageTime = 0.85f;
        m_DamageTrigger.onDealDamage += DealDamage;
        m_DamageTrigger.gameObject.layer = 10;

        moveSpeed = 35.0f;
        rigidbody.drag = 25.0f;

        m_AdditionalDamageTrigger = m_DamageTrigger.gameObject.AddComponent<CircleCollider2D>();
        m_AdditionalDamageTrigger.isTrigger = true;
        m_AdditionalDamageTrigger.radius = 0.2f;
        m_AdditionalDamageTrigger.enabled = false;


    }

    private void OnEnable()
    {
        m_IsIdling = true;
        m_CurrentFrame = 0;
        m_AttackingLeft = true;
        m_CooldownTimer = 0.0f;

        float rand = Random.Range(-8f, 8f);
        moveSpeed += rand;

        FindEnemy();
    }

    private int m_CurrentFrame = -1;

    protected override void Update()
    {
        //TODO Movement / physics based code should be added to another script
        //if(!isAlive) return;

        base.Update();

        m_AnimationTimer += Time.deltaTime;
        if(m_CooldownTimer > 0)
        {
            m_CooldownTimer -= Time.deltaTime;
        }

        if(m_IsIdling)
        {
            if(m_AnimationTimer >= moveAnimationTime)
            {
                m_AnimationTimer %= moveAnimationTime;
            }

            int frame = Mathf.FloorToInt((m_AnimationTimer / moveAnimationTime) * m_MoveSprites.Length);

            if(m_CurrentFrame != frame)
            {
                m_CurrentFrame = frame;
                spriteRenderer.sprite = m_MoveSprites[frame];
            }
        }
        else
        {
            if(m_AnimationTimer >= attackAnimationTime)
            {
                m_AnimationTimer %= moveAnimationTime;
                m_IsIdling = true;
                int frame = Mathf.FloorToInt((m_AnimationTimer / moveAnimationTime) * m_MoveSprites.Length);
                //Force frame change
                m_CurrentFrame = -1;
                spriteRenderer.sprite = m_MoveSprites[frame];
                m_AdditionalDamageTrigger.enabled = false;
            }
            else
            {
                int frame = Mathf.FloorToInt((m_AnimationTimer / attackAnimationTime) * m_AttackSprites.Length);
                if(frame != m_CurrentFrame)
                {
                    m_CurrentFrame = frame;

                    if(frame >= 9 && frame <= 12)
                    {
                        m_AdditionalDamageTrigger.enabled = true;
                    }
                    else
                    {
                        m_AdditionalDamageTrigger.enabled = false;
                    }

                    spriteRenderer.sprite = m_AttackSprites[frame];
                }
            }

        }
    }

    static private Vector2 m_LeftTargetPosition;
    static private Vector2 m_RightTargetPosition;
    static private int m_LastTargetUpdateFrame = -1;

    protected override void FixedUpdate()
    {
        //TODO Movement / physics based code should be added to another script
        //if(!isAlive) return;

        //Move to the left or right of enemy
        /*if(m_Enemy == null || !m_Enemy.isAlive)
        {
            FindEnemy();
        }
        */

        if(!m_Enemy.isAlive) return;

        if(m_IsIdling)//m_Enemy != null && m_Enemy.isAlive && )
        {
            //Choose target position
            if(Time.frameCount > m_LastTargetUpdateFrame)
            {
                const float targetDistantPosition = 1.0f;
                RaycastHit2D leftHit = Physics2D.Raycast(m_Enemy.spriteRenderer.bounds.center, Vector2.left, targetDistantPosition, (1 << 6));
                RaycastHit2D rightHit = Physics2D.Raycast(m_Enemy.spriteRenderer.bounds.center, Vector2.right, targetDistantPosition, (1 << 6));


                Vector2 leftPos = new Vector2(m_Enemy.spriteRenderer.bounds.center.x - targetDistantPosition, m_Enemy.spriteRenderer.bounds.center.y);
                if(leftHit.collider != null)
                {
                    leftPos = leftHit.point;
                }
                if(transform.position.x < m_Enemy.spriteRenderer.bounds.center.x)
                {
                    float xDistance = m_Enemy.spriteRenderer.bounds.center.x - transform.position.x;
                    if(xDistance < targetDistantPosition)
                    {
                        leftPos = new Vector2(transform.position.x, leftPos.y);
                    }
                }
                Vector2 rightPos = new Vector2(m_Enemy.spriteRenderer.bounds.center.x + targetDistantPosition, m_Enemy.spriteRenderer.bounds.center.y);
                if(rightHit.collider != null)
                {
                    rightPos = rightHit.point;
                }
                if(transform.position.x > m_Enemy.spriteRenderer.bounds.center.x)
                {
                    float xDistance = transform.position.x - m_Enemy.spriteRenderer.bounds.center.x;
                    if(xDistance < targetDistantPosition)
                    {
                        rightPos = new Vector2(transform.position.x, rightPos.y);
                    }
                }

                m_LeftTargetPosition = leftPos;
                m_RightTargetPosition = rightPos;
            }

            Vector2 targetPos;
            if(Vector2.Distance(m_LeftTargetPosition, transform.position) < Vector2.Distance(m_RightTargetPosition, transform.position))
            {
                targetPos = m_LeftTargetPosition;
            }
            else
            {
                targetPos = m_RightTargetPosition;
            }


            if(Vector2.Distance(targetPos, transform.position) < 0.2f)
            {
                Move(Vector2.zero);
                if(m_CooldownTimer <= 0.0f)
                    Attack();
            }
            else
            {
                Move((targetPos - (Vector2)transform.position).normalized);
            }


        }
        else
        {
            Move(Vector2.zero);
        }

        base.FixedUpdate();
    }
    protected override void OnMove()
    {
        rigidbody.AddForce(moveDirection * moveSpeed, ForceMode2D.Force);
    }

    public void Attack()
    {
        /*
        if(m_Enemy == null || !m_Enemy.isAlive)
        {
            FindEnemy();
        }

        if(m_Enemy == null || !m_Enemy.isAlive) return;
        */

        m_IsIdling = false;
        //Force frame change
        m_CurrentFrame = -1;

        if(m_Enemy.transform.position.x < transform.position.x)
        {
            //Attack left
            m_AttackingLeft = true;
            spriteRenderer.flipX = true;

            m_AdditionalDamageTrigger.offset = new Vector2(-1.0f, 0.2f);
        }
        else
        {
            //Attack right
            m_AttackingLeft = false;
            spriteRenderer.flipX = false;
            m_AdditionalDamageTrigger.offset = new Vector2(1.0f, 0.2f);
        }

        m_CooldownTimer = attackCoolDownTime;
    }

    private void DealDamage(Actor actor)
    {
        actor.TakeDamage(5);
    }

    protected override void OnDeath()
    {
        Destroy(gameObject);
    }
}

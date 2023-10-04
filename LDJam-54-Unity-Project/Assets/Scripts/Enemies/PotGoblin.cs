using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class PotGoblin : Enemy
{

    static private Sprite[] m_IdleSprites;
    static private Sprite[] m_AttackSprites;
    static private Sprite m_RockSprite;

    public float idleAnimationTime { get; private set; } = 0.5f;
    public float attackAnimationTime { get; private set; } = 0.5f;

    public float attackTime { get; set; } = 3.0f;

    public BoxCollider2D bodyTrigger { get; private set; }

    private float m_AttackTimer = 0.0f;
    private float m_AnimationTimer = 0.0f;
    private bool m_IsIdling = true;

    private Vector2 m_CurrentThrowDirection = Vector2.zero;

    private int m_CurrentAttackFrame = 0;



    protected override void Awake()
    {
        base.Awake();

        float rand = Random.Range(0.2f, 0.8f);
        attackTime += rand;

        moveSpeed = 0.0f;

        if(m_IdleSprites == null)
        {
            m_IdleSprites = Resources.LoadAll<Sprite>("Sprites/Enemy/Pot Goblin/Pot_Goblin_Idle_Sheet");
            m_AttackSprites = Resources.LoadAll<Sprite>("Sprites/Enemy/Pot Goblin/Pot_Goblin_Attack_Sheet");
            Sprite[] attackSpritesFix = new Sprite[m_AttackSprites.Length + 2];
            for(int i = 0; i < m_AttackSprites.Length; i++)
            {
                attackSpritesFix[i] = m_AttackSprites[i];
            }

            attackSpritesFix[4] = m_AttackSprites[1];
            attackSpritesFix[5] = m_AttackSprites[0];

            m_AttackSprites = attackSpritesFix;

            m_RockSprite = Resources.Load<Sprite>("Sprites/Enemy/Pot Goblin/Pot_Goblin_Rock");
        }

        bodyTrigger = spriteRenderer.gameObject.AddComponent<BoxCollider2D>();
        bodyTrigger.isTrigger = true;
        bodyTrigger.offset = new Vector2(0.0f, 0.2f);
        bodyTrigger.size = new Vector2(0.47f, 0.47f);

    }

    private int m_CurrentFrame = -1;

    protected override void Update()
    {
        //TODO Movement / physics based code should be added to another script
        //if(!isAlive) return;

        base.Update();

        m_AttackTimer += Time.deltaTime;
        m_AnimationTimer += Time.deltaTime;

        if(m_AttackTimer >= attackTime)
        {
            Throw();
            m_AttackTimer = 0.0f;
        }

        if(m_IsIdling)
        {
            if(m_AnimationTimer >= idleAnimationTime)
            {
                m_AnimationTimer %= idleAnimationTime;
            }

            int frame = Mathf.FloorToInt((m_AnimationTimer / idleAnimationTime) * (m_IdleSprites.Length));
            if(frame != m_CurrentFrame)
            {
                m_CurrentFrame = frame;
                spriteRenderer.sprite = m_IdleSprites[frame];
            }
        }
        else
        {
            if(m_AnimationTimer >= attackAnimationTime)
            {
                m_AnimationTimer = 0.0f;
                m_IsIdling = true;
                //Force frame switch
                m_CurrentFrame = -1;
            }

            int frame = (int)((m_AnimationTimer / attackAnimationTime) * (m_AttackSprites.Length - 1));

                if(m_CurrentAttackFrame < 3 && frame >= 3)
                {
                    ThrowProjectile();
                }
                m_CurrentAttackFrame = frame;

            if(frame != m_CurrentFrame)
            {
                spriteRenderer.sprite = m_AttackSprites[frame];
            }

        }
    }


    protected override void OnDeath()
    {
        Destroy(gameObject);
    }

    public void Throw()
    {
        if(m_Enemy == null || !m_Enemy.isAlive)
        {
            FindEnemy();
        }

        if(m_Enemy == null || !m_Enemy.isAlive)
        {
            return;
        }

        m_IsIdling = false;

        if(m_Enemy.transform.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

        m_CurrentThrowDirection = (m_Enemy.spriteRenderer.bounds.center - transform.position).normalized;

        m_AnimationTimer = 0.0f;
    }

    static List<PotGoblinProjectile> m_ProjectilePool = new List<PotGoblinProjectile>();
    private void ThrowProjectile()
    {
        PotGoblinProjectile projectile;
        if(m_ProjectilePool.Count == 0)
        {
            //Create
            GameObject projectileObject = new GameObject("Pot Goblin Projectile");
            projectileObject.SetActive(false); //This ensures that PotGoblinProjectile.OnEnable is called after PotGoblinProjectile.direction is set
            projectile = projectileObject.AddComponent<PotGoblinProjectile>();
        }
        else
        {
            int lastIndex = m_ProjectilePool.Count - 1;
            projectile = m_ProjectilePool[lastIndex];
            m_ProjectilePool.RemoveAt(lastIndex);

            SceneManager.MoveGameObjectToScene(projectile.gameObject, SceneManager.GetActiveScene());
        }

        projectile.transform.position = transform.position;
        projectile.direction = m_CurrentThrowDirection;
        projectile.gameObject.SetActive(true);
        projectile.onHit += OnProjectileHit;
    }

    //Called when hitting an actor or hitting a wall
    private void OnProjectileHit(PotGoblinProjectile projectile)
    {
        DontDestroyOnLoad(projectile.gameObject);
        projectile.gameObject.SetActive(false);
        m_ProjectilePool.Add(projectile);
        projectile.onHit -= OnProjectileHit;

    }

    public class PotGoblinProjectile : MonoBehaviour
    {

        public SpriteRenderer spriteRenderer;
        public CircleCollider2D trigger;
        public DamageTrigger damageTrigger;
        public new Rigidbody2D rigidbody;
        public float speed = 5.0f;
        public Vector2 direction = Vector2.zero;
        public float rotateSpeed = 1000.0f;

        public int damage = 5;

        public Action<PotGoblinProjectile> onHit;

        private void Awake()
        {
            gameObject.layer = 10;

            damageTrigger = gameObject.AddComponent<DamageTrigger>();
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = m_RockSprite;
            trigger = damageTrigger.CreateTrigger<CircleCollider2D>();

            damageTrigger.onDealDamage += DealDamage;
            trigger.callbackLayers = (1 << 6) | 1;

            rigidbody = gameObject.AddComponent<Rigidbody2D>();

            rigidbody.gravityScale = 0.0f;
            rigidbody.drag = 0.0f;
            rigidbody.angularDrag = 0.0f;
        }

        private void OnEnable()
        {
            rigidbody.velocity = direction * speed;
            rigidbody.angularVelocity = rotateSpeed;
        }

        /*
        void FixedUpdate()
        {
            transform.position += (Vector3)direction * speed * Time.fixedDeltaTime;
            if(direction.x < 0.0f)
            {
                transform.localEulerAngles += new Vector3(0.0f, 0.0f, rotateSpeed * Time.fixedDeltaTime);
            }
            else
            {
                transform.localEulerAngles -= new Vector3(0.0f, 0.0f, rotateSpeed * Time.fixedDeltaTime);
            }
        }
        */

        void OnTriggerEnter2D(Collider2D collision)
        {
            onHit?.Invoke(this);
        }

        private void DealDamage(Actor actor)
        {
            actor.TakeDamage(damage);
        }
    }
}



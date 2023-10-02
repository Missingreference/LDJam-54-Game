using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Elanetic.Tools;
using Unity.VisualScripting;

public class Human : Actor
{
    public FacingDirection facingDirection = FacingDirection.Right;

    //Hover
    public float spriteHoverSpeed = 0.5f;
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


    //Weapon
    public HumanWeapon weapon { get; private set; }

    //Events
    public Action onAttack;
    public Action onFacingDirectionChanged;


    private Sprite m_MainSprite;
    private Sprite[] m_IdleSpriteSheet;

    private float m_HoverTimer = 0.0f;
    private float m_DamageFlashTimer = 0.0f;
    private float m_DamageFlashCount = 0;

    //public List<DamageTrigger> attackTriggers = new List<DamageTrigger>();
    //private List<DamageTrigger> m_PooledAttackTriggers = new List<DamageTrigger>();
    //private List<float> m_AttackSwipeTimers = new List<float>();
    //private List<float> m_AttackDirections = new List<float>();

    //Ability triggers
    public HumanSlashTrigger slashTrigger;

    protected override void Awake()
    {
        base.Awake();
        //SetHealth(1);
        //moveSpeed = 1.0f;

        gameObject.layer = 7; //Player Movement

        m_MainSprite = Resources.Load<Sprite>("Sprites/Character/Character1");
        m_IdleSpriteSheet = Resources.LoadAll<Sprite>("Sprites/Character/Character_Idle_Sheet");
        spriteRenderer.sprite = m_MainSprite;
        spriteRenderer.gameObject.layer = 6; //Player Body

        bodyTrigger = spriteRenderer.gameObject.AddComponent<BoxCollider2D>();
        bodyTrigger.isTrigger = true;
        bodyTrigger.offset = new Vector2(0.0f, 0.35f);
        bodyTrigger.size = (spriteRenderer.bounds.extents * 2.0f) * 0.85f;

        movementCollider = gameObject.AddComponent<BoxCollider2D>();
        movementCollider.isTrigger = false;
        movementCollider.offset = new Vector2(0.0f, 0.35f);
        movementCollider.size = (spriteRenderer.bounds.extents * 2.0f) * 0.9f;

        GameObject weaponObject = new GameObject("Weapon");
        weaponObject.transform.parent = transform;
        weapon = weaponObject.AddComponent<HumanWeapon>();

        //Ability triggers
        GameObject slashTriggerObject = new GameObject("Slash Trigger");
        slashTriggerObject.transform.parent = transform;
        slashTriggerObject.transform.localPosition = Vector3.zero;
        slashTriggerObject.transform.localEulerAngles = Vector3.zero;
        slashTriggerObject.transform.localScale = Vector3.one;
        slashTrigger = slashTriggerObject.AddComponent<HumanSlashTrigger>();
        slashTrigger.onTrigger += 
    }



    protected override void Update()
    {
        AnimateHover();

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

        //Choose Sprite
        percent = m_HoverTimer;
        spriteRenderer.sprite = m_IdleSpriteSheet[(int)(m_IdleSpriteSheet.Length * percent)];
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

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        m_DamageFlashCount = damageFlashCount;
    }

    protected override void OnMove()
    {
        rigidbody.AddForce(moveDirection * moveSpeed, ForceMode2D.Force);

        if(moveDirection.x < 0)
        {
            SetFacingDirection(FacingDirection.Left);
        }
        else if(moveDirection.x > 0)
        {
            SetFacingDirection(FacingDirection.Right);
        }
    }

    public void SetFacingDirection(FacingDirection direction) 
    {
        if(direction != facingDirection)
        {
            if(direction == FacingDirection.Left)
            {
                spriteRenderer.flipX = true;
            }
            else
            {
                spriteRenderer.flipX = false;
            }

            facingDirection = direction;
            onFacingDirectionChanged?.Invoke();
        }
    }


    private void DealDamage(Actor target)
    {
        target.TakeDamage(5);
    }
}

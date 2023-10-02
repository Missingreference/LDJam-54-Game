using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Elanetic.Tools;

public class SmiteAbility : AttackAbility
{
    static private Sprite[] m_Sprites;

    public float damageRadius { get; private set; } = 0.5f;

    public CircleCollider2D trigger;
    public SpriteRenderer spriteRenderer { get; set; }

    protected override void Awake()
    {
        base.Awake();

        attackSpeed = 1.0f;

        trigger = damageTrigger.CreateTrigger<CircleCollider2D>();

        GameObject spriteRendererObject = new GameObject("Sprite");
        spriteRendererObject.transform.parent = transform;
        spriteRenderer = spriteRendererObject.AddComponent<SpriteRenderer>();

        if(m_Sprites == null)
            m_Sprites = Resources.LoadAll<Sprite>("Sprites/Effects/Smite_Sheet");

        damageTrigger.onDealDamage += (Actor actor) =>
        {
            actor.TakeDamage(5);
        };
        trigger.callbackLayers = 1 << 8;
    }

    protected override void InitiateAttack()
    {
        trigger.radius = damageRadius;


        spriteRenderer.gameObject.SetActive(true);
        spriteRenderer.transform.position = target;
    }


    protected override void Update()
    {
        base.Update();
    }

    protected override void AnimateAttack()
    {
        //Animate sprites
        int frame = (int)(m_Sprites.Length * Easing.Ease(0.0f, 1.0f, currentAttackTime, Easing.EasingFunction.Linear));

        if(frame < 0) frame = 0;
        else if(frame >= m_Sprites.Length) frame = m_Sprites.Length - 1;

        spriteRenderer.sprite = m_Sprites[frame];

        if(frame >= 6 && frame <= 8)
        {
            damageTrigger.gameObject.SetActive(true);
            damageTrigger.transform.position = target;  
        }
        else
        {
            damageTrigger.gameObject.SetActive(false);
        }
    }

    protected override void OnReset()
    {
        spriteRenderer.gameObject.SetActive(false);
    }

}

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Elanetic.Tools;

public class PoisonAbility : AttackAbility
{
    static private Sprite[] m_Sprites;

    public CircleCollider2D trigger;
    public SpriteRenderer spriteRenderer;

    private Vector2 m_DirectionVector = Vector2.zero;
    private float m_DirectionAngle = 0.0f;

    protected override void Awake()
    {
        base.Awake();

        attackSpeed = 0.55f;

        if(m_Sprites == null)
            m_Sprites = Resources.LoadAll<Sprite>("Sprites/Effects/Poison_Puddle_Sheet");

        trigger = damageTrigger.CreateTrigger<CircleCollider2D>();

        GameObject spriteRendererObject = new GameObject("Sprite");
        spriteRendererObject.transform.parent = transform;
        spriteRenderer = spriteRendererObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = -1;
    }

    protected override void InitiateAttack()
    {



        //m_DirectionVector = ((Vector2)transform.position - target).normalized;
        //m_DirectionAngle = Vector2ToDegrees(m_DirectionVector);

        trigger.radius = 2.0f;

        damageTrigger.gameObject.SetActive(true);

        spriteRenderer.gameObject.SetActive(true);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

    protected override void AnimateAttack()
    {
        //Animate collider
        //damageTrigger.transform.localPosition = Easing.Ease(Vector2.zero, destination, currentAttackTime, Easing.EasingFunction.Linear);



        int frame = (int)(m_Sprites.Length * Easing.Ease(0.0f, 1.0f, currentAttackTime, Easing.EasingFunction.Linear));

        if(frame < 0) frame = 0;
        else if(frame >= m_Sprites.Length) frame = m_Sprites.Length - 1;

        spriteRenderer.sprite = m_Sprites[frame];
    }

    protected override void OnReset()
    {
        spriteRenderer.gameObject.SetActive(false);
    }
}

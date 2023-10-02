using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Elanetic.Tools;

public class ThrowKnifeAbility : AttackAbility
{

    static private Sprite m_Sprite;

    public BoxCollider2D trigger;
    public SpriteRenderer spriteRenderer { get; set; }

    private Vector2 m_DirectionVector = Vector2.zero;
    private float m_DirectionAngle = 0.0f;

    protected override void Awake()
    {
        base.Awake();

        trigger = damageTrigger.CreateTrigger<BoxCollider2D>();
        GameObject spriteRendererObject = new GameObject("Sprite");
        spriteRendererObject.transform.parent = transform;
        spriteRenderer = spriteRendererObject.AddComponent<SpriteRenderer>();

        if(m_Sprite == null)
            m_Sprite = Resources.Load<Sprite>("Sprites/Character/Weapon/Sword");


        spriteRenderer.sprite = m_Sprite;

        damageTrigger.onDealDamage += (Actor actor) =>
        {
            actor.TakeDamage(5);
        };
    }

    protected override void InitiateAttack()
    {
        m_DirectionVector = ((Vector2)transform.position - target).normalized;
        m_DirectionAngle = Vector2ToDegrees(m_DirectionVector);

        trigger.size = new Vector2(0.3f, 0.7f);
        trigger.offset = new Vector2(0.0f, 0.5f);

        damageTrigger.gameObject.SetActive(true);

        spriteRenderer.gameObject.SetActive(true);

        spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, m_DirectionAngle + 135.0f);// + 90.0f + 180.0f + 90.0f);
        damageTrigger.transform.localEulerAngles = new Vector3(0, 0, m_DirectionAngle + 90.0f);
    }

    protected override void AnimateAttack()
    {
        //Animate collider
        damageTrigger.transform.localPosition = Easing.Ease(Vector2.zero, target, currentAttackTime, Easing.EasingFunction.Linear);


        //Animate sprites
        spriteRenderer.transform.position = trigger.bounds.center;
    }

    protected override void OnReset()
    {
        spriteRenderer.gameObject.SetActive(false);
    }
    private Vector2 DegreesToVector2(float degrees)
    {
        return new Vector2(Mathf.Cos(degrees * Mathf.Deg2Rad), Mathf.Sin(degrees * Mathf.Deg2Rad));
    }
    private float Vector2ToDegrees(Vector2 direction)
    {
        if(direction.y < 0.0f)
            return -(Mathf.Acos(direction.x) * Mathf.Rad2Deg);
        return Mathf.Acos(direction.x) * Mathf.Rad2Deg;
    }

}

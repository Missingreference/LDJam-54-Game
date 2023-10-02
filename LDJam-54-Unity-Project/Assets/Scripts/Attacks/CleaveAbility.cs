using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Elanetic.Tools;

public class CleaveAbility : AttackAbility
{

    static private Sprite[] m_Sprites;

    public Vector2 cleaveSize { get; set; } = new Vector2(2.5f, 0.4f);
    public float cleaveDistance { get; set; } = 3.0f;

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

        if(m_Sprites == null)
            m_Sprites = Resources.LoadAll<Sprite>("Sprites/Effects/Cleave_Sheet");

        damageTrigger.onDealDamage += (Actor actor) =>
        {
            actor.TakeDamage(5);
        };
    }

    protected override void InitiateAttack()
    {
        m_DirectionVector = ((Vector2)transform.position - target).normalized;
        m_DirectionAngle = Vector2ToDegrees(m_DirectionVector);

        trigger.size = cleaveSize;
        trigger.offset = new Vector2(0, trigger.size.y * 0.5f);

        damageTrigger.gameObject.SetActive(true);

        spriteRenderer.gameObject.SetActive(true);

        spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, m_DirectionAngle + 90.0f + 180.0f);
        damageTrigger.transform.localEulerAngles = new Vector3(0, 0, m_DirectionAngle + 90.0f);
    }

    protected override void AnimateAttack()
    {
        //Animate collider
        Vector2 destination = m_DirectionVector * cleaveDistance;
        damageTrigger.transform.localPosition = Easing.Ease(Vector2.zero, destination, currentAttackTime, Easing.EasingFunction.Linear);


        //Animate sprites
        int frame = (int)(m_Sprites.Length * Easing.Ease(0.0f, 1.0f, currentAttackTime, Easing.EasingFunction.Linear));

        if(frame < 0) frame = 0;
        else if(frame >= m_Sprites.Length) frame = m_Sprites.Length - 1;

        spriteRenderer.sprite = m_Sprites[frame];
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

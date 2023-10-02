using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Elanetic.Tools;

public class SlashAbility : AttackAbility
{
    static private Sprite[] m_Sprites;

    public float slashWidth { get; set; } = 0.5f;
    public float slashDistance { get; set; } = 1.0f;
    public float slashRange { get; set; } = 80.0f;

    public bool slashClockwise = true;

    public BoxCollider2D trigger;
    public SpriteRenderer spriteRenderer { get; set; }

    public float m_Direction { get; private set; } = 0.0f;

    protected override void Awake()
    {
        base.Awake();

        trigger = damageTrigger.CreateTrigger<BoxCollider2D>();
        GameObject spriteRendererObject = new GameObject("Sprite");
        spriteRendererObject.transform.parent = transform;
        spriteRenderer = spriteRendererObject.AddComponent<SpriteRenderer>();

        if(m_Sprites == null)
        m_Sprites = Resources.LoadAll<Sprite>("Sprites/Effects/Slash_Sheet");

        damageTrigger.onDealDamage += (Actor actor) =>
        {
            actor.TakeDamage(5);
        };
    }

    protected override void InitiateAttack()
    {
        m_Direction = Vector2ToDegrees(((Vector2)transform.position - target).normalized);

        trigger.size = new Vector2(slashWidth, slashDistance);
        trigger.offset = new Vector2(0, trigger.size.y * 0.5f);

        damageTrigger.gameObject.SetActive(true);

        spriteRenderer.gameObject.SetActive(true);

        if(slashClockwise)
        {
            spriteRenderer.flipX = false;
            spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, m_Direction);
        }
        else
        {
            spriteRenderer.flipX = true;
            spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, m_Direction - 180);
        }
    }


    protected override void Update()
    {
        base.Update();
    }

    protected override void AnimateAttack()
    {
        //Animate swing
        if(slashClockwise)
        {
            float startAngle = m_Direction + (slashRange * 0.5f);
            damageTrigger.transform.localEulerAngles = new Vector3(0.0f, 0.0f, startAngle - (currentAttackTime * slashRange) - 90.0f);
        }
        else
        {
            float startAngle = m_Direction - (slashRange * 0.5f);
            damageTrigger.transform.localEulerAngles = new Vector3(0.0f, 0.0f, startAngle + (currentAttackTime * slashRange) - 90.0f);
        }

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

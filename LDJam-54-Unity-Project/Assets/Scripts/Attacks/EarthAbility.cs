using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Elanetic.Tools;

public class EarthAbility : AttackAbility
{
    static private Sprite[] m_Sprites;

    public Vector2 attackSize = new Vector2(1.5f, 2.0f);
    public float attackDistance = 0.5f;

    //public float rockSpacing = 0.25f;

    public BoxCollider2D trigger;
    public SpriteRenderer spriteRenderer;

    private Vector2 m_DirectionVector = Vector2.zero;
    private float m_DirectionAngle = 0.0f;

    /*public List<CircleCollider2D> triggers { get; private set; } = new List<CircleCollider2D>();
    public List<SpriteRenderer> spriteRenderers { get; private set; } = new List<SpriteRenderer>();

    private List<CircleCollider2D> m_TriggerPool = new List<CircleCollider2D>();
    private List<SpriteRenderer> m_SpriteRendererPool = new List<SpriteRenderer>();

    */
    protected override void Awake()
    {
        base.Awake();

        if(m_Sprites == null)
            m_Sprites = Resources.LoadAll<Sprite>("Sprites/Effects/Shatter_Sheet");

        trigger = damageTrigger.CreateTrigger<BoxCollider2D>();
        GameObject spriteRendererObject = new GameObject("Sprite");
        spriteRendererObject.transform.parent = transform;
        spriteRenderer = spriteRendererObject.AddComponent<SpriteRenderer>();

        damageTrigger.onDealDamage += (Actor actor) =>
        {
            actor.TakeDamage(5);
        };

        trigger.callbackLayers = 1 << 8;
    }

    protected override void InitiateAttack()
    {



        m_DirectionVector = (target - (Vector2)transform.position).normalized;
        m_DirectionAngle = Vector2ToDegrees(m_DirectionVector);

        trigger.size = attackSize;

        damageTrigger.gameObject.SetActive(true);

        spriteRenderer.gameObject.SetActive(true);

        if(m_DirectionVector.x < 0.0f)
        {
            spriteRenderer.transform.localPosition = new Vector3(-attackDistance, 0.0f, 0.0f);
            spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, 180.0f);
            spriteRenderer.flipY = true;
            trigger.offset = new Vector2(-1f, 0.0f);// trigger.size.y * 0.5f);
        }
        else
        {
            spriteRenderer.transform.localPosition = new Vector3(attackDistance, 0.0f, 0.0f);
            spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, 180.0f -180.0f);
            spriteRenderer.flipY = false;
            trigger.offset = new Vector2(1f, 0.0f);// trigger.size.y * 0.5f);
        }

        //spriteRenderer.transform.localEulerAngles = new Vector3(0, 0, m_DirectionAngle + 90.0f + 180.0f);
        //damageTrigger.transform.localEulerAngles = new Vector3(0, 0, m_DirectionAngle + 90.0f);

        /*
        float distance = Vector2.Distance(transform.position, target);

        int numInstances = Mathf.FloorToInt(distance / rockSpacing) + 1;

        int missingCount = numInstances - spriteRenderers.Count;

        Vector2 direction = ((Vector2)transform.transform.position - target).normalized;

        for(int i = 0; i < missingCount; i++)
        {
            //Create renderer
            GameObject spriteObject = new GameObject("Rock Sprite");
            spriteObject.transform.parent = transform;
            SpriteRenderer sRenderer = spriteObject.AddComponent<SpriteRenderer>();
            spriteRenderers.Add(sRenderer);

            spriteObject.transform.position += (Vector3)direction * (rockSpacing * (i + 1));
        }

        for(int i = missingCount; i < numInstances; i++)
        {
            SpriteRenderer sRenderer = m_SpriteRendererPool[i];
            spriteRenderers.Add(sRenderer);
            m_SpriteRendererPool.RemoveAt(i);
            sRenderer.gameObject.SetActive(true);

            sRenderer.transform.position += (Vector3)direction * (rockSpacing * (i + 1));
        }
        */
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

    }

    protected override void AnimateAttack()
    {
        //Animate collider
        Vector2 destination;
        if(m_DirectionVector.x < 0.0f)
        {
            destination = Vector2.left * attackDistance;
        }
        else
        {
            destination = Vector2.right * attackDistance;
        }
        damageTrigger.transform.localPosition = Easing.Ease(Vector2.zero, destination, currentAttackTime, Easing.EasingFunction.Linear);



        int frame = (int)(m_Sprites.Length * Easing.Ease(0.0f, 1.0f, currentAttackTime, Easing.EasingFunction.Linear));

        if(frame < 0) frame = 0;
        else if(frame >= m_Sprites.Length) frame = m_Sprites.Length - 1;

        spriteRenderer.sprite = m_Sprites[frame];

        /*

        for(int i = 0; i < spriteRenderers.Count; i++)
        {
            spriteRenderers[i].sprite = m_Sprites[frame];
        }
        */
    }

    protected override void OnReset()
    {

        spriteRenderer.gameObject.SetActive(false);

        /*
        for(int i = 0; i < triggers.Count; i++)
        {
            CircleCollider2D trigger = triggers[i];
            trigger.enabled = false;
            m_TriggerPool.Add(trigger);

        }

        for(int i = 0; i < spriteRenderers.Count; i++)
        {
            SpriteRenderer sRenderer = spriteRenderers[i];
            sRenderer.gameObject.SetActive(false);
            m_SpriteRendererPool.Add(sRenderer);
        }

        triggers.Clear();
        spriteRenderers.Clear();

        */
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

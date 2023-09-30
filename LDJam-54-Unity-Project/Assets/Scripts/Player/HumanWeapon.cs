using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Elanetic.Tools;

public class HumanWeapon : MonoBehaviour
{
    public float hoverSpeed = 0.4f;
    public float hoverDistance = 0.1f;
    public float followSpeed = 50.0f;
    public float attackFollowSpeed = 100.0f;
    public Vector2 positionInFront = new Vector2(0.35f, 0.2f);
    public float postSwingWaitTime = 0.2f;


    public SpriteRenderer spriteRenderer;

    private Human m_Human;
    private Vector2 m_HomePosition; //Actual position hovers around home position
    private Vector2 m_TargetPosition; //Home position tries to move to target position
    private Vector2 m_PositionInFront;

    private float m_HoverTimer = 0.0f;

    private float m_PostSwingTimer = 0.0f;
    private bool m_Attacking = false;

    void Awake()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/Character/Weapon/Sword");

        m_Human = transform.parent.GetComponent<Human>();
        m_Human.onFacingDirectionChanged += OnFacingDirectionChanged;
        m_Human.onAttack += OnAttack;

        transform.SetParent(null);

        m_PositionInFront = positionInFront;

        m_TargetPosition = (Vector2)m_Human.transform.position + m_PositionInFront;
        m_HomePosition = m_TargetPosition;
        transform.position = m_HomePosition;
    }


    void Update()
    {
        m_TargetPosition = GetTargetPosition();

        //Move sword towards target position
        Vector2 direction = (m_TargetPosition - m_HomePosition);
        if(direction.magnitude > 1.0f) direction.Normalize();

        if(m_Attacking)
        {
            m_HomePosition += direction * attackFollowSpeed * Time.deltaTime;
            transform.position = m_HomePosition;
        }
        else
        {
            m_HomePosition += direction * followSpeed * Time.deltaTime;
            AnimateHover();
        }

        spriteRenderer.flipX = transform.position.x < m_Human.transform.position.x;

    }

    private void AnimateHover()
    {
        m_HoverTimer += hoverSpeed * Time.deltaTime;

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

        float height = Easing.Sinusoidal.InOut(hoverDistance * -0.25f, hoverDistance * 0.75f, percent);

        transform.position = new Vector3(m_HomePosition.x, m_HomePosition.y + height, transform.position.z);
    }

    private Vector2 m_LastTriggerDirection = Vector2.zero;

    private Vector2 GetTargetPosition()
    {

        if(m_Attacking)
        {
            Vector2 direction;
            float distance = 0.5f;
            if(m_Human.attackTriggers.Count > 0)
            {
                DamageTrigger targetTrigger = m_Human.attackTriggers[m_Human.attackTriggers.Count - 1];
                direction = DegreesToVector2(targetTrigger.transform.eulerAngles.z + 90.0f);
                //m_TargetPosition = (Vector2)m_Human.transform.position + (direction * distance);
                

                m_LastTriggerDirection = direction;
            }
            else
            {
                direction = m_LastTriggerDirection;
                //m_TargetPosition = ;


                m_PostSwingTimer += Time.deltaTime;
                if(m_PostSwingTimer >= postSwingWaitTime)
                {
                    m_Attacking = false;
                }

            }

            if(spriteRenderer.flipX)
            {
                transform.eulerAngles = new Vector3(0, 0, 45);
                transform.eulerAngles = new Vector3(0, 0, Vector2ToDegrees(direction) + 180 + 45);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, 0, -45);
                transform.eulerAngles = new Vector3(0, 0, Vector2ToDegrees(direction) - 45);
            }

            return (Vector2)m_Human.transform.position + (direction * distance);
        }
        else
        {
            transform.eulerAngles = Vector3.zero;
            return (Vector2)m_Human.transform.position + m_PositionInFront;
        }
    }

    private void OnAttack()
    {
        m_Attacking = true;
        m_PostSwingTimer = 0.0f;
    }

    private void OnFacingDirectionChanged()
    {
        if(m_Human.facingDirection == FacingDirection.Left)
        {
            m_PositionInFront = new Vector2(-positionInFront.x, positionInFront.y);
            //spriteRenderer.flipX = true;
        }
        else
        {
            m_PositionInFront = new Vector2(positionInFront.x, positionInFront.y);
            //spriteRenderer.flipX = false;
        }
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

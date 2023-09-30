using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCameraController : MonoBehaviour
{
    public Human target;
    public float lookaheadDistance = 1.0f;
    public float dampening = 0.15f;
    public float noDirectionDampening = 0.5f;

    private Vector2 m_TargetPosition;

    void Awake()
    {
        if(target == null)
            target = FindObjectOfType<Human>();

    }

    void Start()
    {

    }

    private Vector2 m_Velocity = Vector3.zero;

    void LateUpdate()
    {
        if(target == null) return;


        float damp;

        if(target.moveDirection != Vector2.zero)
        {
            damp = dampening;
            Vector2 direction = target.moveDirection.normalized;
            m_TargetPosition = target.transform.position + new Vector3(lookaheadDistance * direction.x, lookaheadDistance * direction.y, 0);
        }
        else
        {
            damp = noDirectionDampening;
            m_TargetPosition = target.transform.position;
        }

        Vector2 newPosition = Vector2.SmoothDamp(transform.position, m_TargetPosition, ref m_Velocity, damp);
        transform.position = new Vector3(newPosition.x, newPosition.y, -10.0f);
    }
}
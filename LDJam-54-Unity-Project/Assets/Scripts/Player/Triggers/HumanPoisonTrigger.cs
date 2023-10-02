using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPoisontTrigger : MonoBehaviour
{
    public CircleCollider2D trigger;

    void Awake()
    {
        trigger = gameObject.AddComponent<CircleCollider2D>();
        trigger.radius = 5.0f;
        trigger.callbackLayers = 1 << 8;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {

    }
    private void OnTriggerStay2D(Collider2D collision)
    {

    }

    private void Trigger()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HumanEarthTrigger : MonoBehaviour
{
    public BoxCollider2D leftTrigger;
    public BoxCollider2D rightTrigger;

    void Awake()
    {
        leftTrigger = gameObject.AddComponent<BoxCollider2D>();
        rightTrigger = gameObject.AddComponent<BoxCollider2D>();
        leftTrigger.callbackLayers = 1 << 8;
        rightTrigger.callbackLayers = 1 << 8;
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

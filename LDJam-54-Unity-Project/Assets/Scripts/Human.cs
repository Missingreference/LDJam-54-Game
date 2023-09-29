using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Human : MonoBehaviour
{
    public float moveSpeed { get; set; } = 5000.0f;
    public Vector2 moveDirection { get; private set; } = Vector2.zero;

    public SpriteRenderer spriteRenderer { get; set; }
    public new Rigidbody2D rigidbody { get; set; }
    public BoxCollider2D bodyTrigger { get; set; }



    void Awake()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        rigidbody = gameObject.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0.0f;
        rigidbody.drag = 10.0f;
        rigidbody.freezeRotation = true;

        bodyTrigger = gameObject.AddComponent<BoxCollider2D>();
        bodyTrigger.isTrigger = true;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(moveDirection.magnitude > 0.0f)
        {
            //Do movement
            rigidbody.AddForce(moveSpeed * Time.fixedDeltaTime * moveDirection, ForceMode2D.Force);

            if(moveDirection.x < 0.0f)
            {
                spriteRenderer.flipX = true;
            }
            else if(moveDirection.x > 0.0f)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    public void Move(Vector2 moveDirection)
    {
        if(moveDirection.magnitude > 1.0f)
        {
            this.moveDirection = moveDirection.normalized;
        }
        else
        {
            this.moveDirection = moveDirection;
        }
    }
}

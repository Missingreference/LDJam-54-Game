using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    static protected AudioClip m_HurtSound;
    static protected AudioSource m_AudioPlayer;


    public int health { get; private set; } = 100;
    public float moveSpeed { get; set; } = 50;
    public Vector2 moveDirection { get; private set; } = Vector2.zero;
    public bool isAlive { get; private set; } = true;

    public SpriteRenderer spriteRenderer { get; private set; }
    public new Rigidbody2D rigidbody { get; private set; }

    public Action onDeath;

    protected virtual void Awake()
    {
        if(m_HurtSound == null)
        {
            m_HurtSound = Resources.Load<AudioClip>("Sounds/Hit_hurt 73");
        }

        GameObject spriteObject = new GameObject("Sprite");
        spriteObject.transform.parent = transform;
        spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        spriteObject.transform.localPosition = Vector3.zero;
        spriteObject.transform.localEulerAngles = Vector3.zero;
        spriteObject.transform.localScale = Vector3.one;

        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        if(sRenderer != null)
        {
            Destroy(sRenderer);
        }

        rigidbody = gameObject.AddComponent<Rigidbody2D>();
        rigidbody.gravityScale = 0.0f;
        rigidbody.drag = 10.0f;
        rigidbody.freezeRotation = true;
        rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    public void SetHealth(int value)
    {
        if(value <= 0)
        {
            health = 0;
            Die();
        }
        else
        {
            health = value;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        if(this is Enemy)
        {
            m_AudioPlayer.PlayOneShot(m_HurtSound);
        }

        if(health - damage <= 0)
        {
            health = 0;
            Die();
        }

        health -= damage;
    }

    public void Die()
    {
        isAlive = false;
        OnDeath();
        onDeath?.Invoke();
    }

    protected virtual void OnDeath()
    {

    }

    protected virtual void Update()
    { 
    
    }

    protected virtual void FixedUpdate()
    {

        if(isAlive && moveDirection.magnitude > 0)
        {
            OnMove();
        }
    }

    public void Move(Vector2 moveDirection)
    {
        if(!isAlive) return;

        if(moveDirection.magnitude > 1.0f)
        {
            this.moveDirection = moveDirection.normalized;
        }
        else
        {
            this.moveDirection = moveDirection;
        }
    }

    protected virtual void OnMove()
    {

    }
}

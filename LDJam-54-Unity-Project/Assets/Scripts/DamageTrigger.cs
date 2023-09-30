using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public Collider2D trigger { get; private set; }
    public bool persistentDamage { get; set; }
    public float persistentDamageTime { get; set; } = 0.5f;

    public Action<Actor> onDealDamage;

    public List<Actor> damagedActors = new List<Actor>();
    private List<float> damageTimers = new List<float>();

    public T CreateTrigger<T>() where T : Collider2D
    {
        trigger = gameObject.AddComponent<T>();
        trigger.isTrigger = true;

        return (T)trigger;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Actor actor = collision.GetComponent<Actor>();
        if (actor == null)
        {
            if(collision.transform.parent == null)
            {
                return;
            }

            actor = collision.transform.parent.GetComponent<Actor>();

            if(actor == null)
            {
                return;
            }
        }

        DamageActor(actor);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!persistentDamage) return;

        Actor actor = collision.GetComponent<Actor>();
        if(actor == null)
        {
            if(collision.transform.parent == null)
            {
                return;
            }

            actor = collision.transform.parent.GetComponent<Actor>();

            if(actor == null)
            {
                return;
            }
        }

        DamageActor(actor);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        
    }

    private void DamageActor(Actor actor)
    {
        if(!actor.isAlive || damagedActors.Contains(actor))
        {
            return;
        }
        else
        {
            damagedActors.Add(actor);
            if(persistentDamage)
                damageTimers.Add(persistentDamageTime);
            onDealDamage?.Invoke(actor);
        }
    }

    private void Update()
    {
        if(!persistentDamage) return;

        for (int i = 0; i < damageTimers.Count; i++)
        {
            float timer = damageTimers[i];
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                damageTimers.RemoveAt(i);
                damagedActors.RemoveAt(i);
                i--;
            }
            else
            {
                damageTimers[i] = timer;
            }
        }
    }
}

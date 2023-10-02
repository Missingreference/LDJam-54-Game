using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    public Collider2D trigger { get; private set; }
    public bool persistentDamage { get; set; } = false;
    public float persistentDamageTime { get; set; } = 0.5f;

    public Action<Actor> onDealDamage;

    public ReadOnlyCollection<Actor> damagedActors => m_DamagedActors.AsReadOnly();

    private List<Actor> m_DamagedActors = new List<Actor>();
    private List<float> m_DamageTimers = new List<float>();

    private List<Actor> m_ActorsWithinTrigger = new List<Actor>();

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

        m_ActorsWithinTrigger.Add(actor);

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

        m_ActorsWithinTrigger.Remove(actor);
    }

    private void DamageActor(Actor actor)
    {
        if(!actor.isAlive || m_DamagedActors.Contains(actor))
        {
            return;
        }
        else
        {
            m_DamagedActors.Add(actor);
            if(persistentDamage)
                m_DamageTimers.Add(persistentDamageTime);
            onDealDamage?.Invoke(actor);
        }
    }

    public void ClearDamagedActors()
    {
        m_DamagedActors.Clear();
        m_DamageTimers.Clear();

        for(int i = 0; i < m_ActorsWithinTrigger.Count; i++)
        {
            DamageActor(m_ActorsWithinTrigger[i]);
        }
    }

    private void Update()
    {
        if(!persistentDamage) return;

        for (int i = 0; i < m_DamageTimers.Count; i++)
        {
            float timer = m_DamageTimers[i];
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                m_DamageTimers.RemoveAt(i);
                m_DamagedActors.RemoveAt(i);
                i--;
            }
            else
            {
                m_DamageTimers[i] = timer;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSlashTrigger : MonoBehaviour
{
    public CircleCollider2D trigger;

    public float cooldownTime = 0.5f;

    private float m_CooldownTimer;

    private List<SlashAbility> m_AbilityPool = new List<SlashAbility>();

    void Awake()
    {
        trigger = gameObject.AddComponent<CircleCollider2D>();
        trigger.radius = 1.0f;
        trigger.callbackLayers = 1 << 8;
        trigger.isTrigger = true;
    }

    private void Update()
    {
        if(m_CooldownTimer > 0.0f)
        {
            m_CooldownTimer -= Time.deltaTime;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(m_CooldownTimer > 0.0f) return;

        Enemy enemy = GetEnemy(collision);
        if(enemy != null && enemy.isAlive)
        {
            Trigger(enemy.transform.position);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(m_CooldownTimer > 0.0f) return;

        Enemy enemy = GetEnemy(collision);
        if(enemy != null && enemy.isAlive)
        {
            Trigger(enemy.transform.position);
        }
    }

    Enemy GetEnemy(Collider2D collider)
    {
        Enemy enemy = collider.GetComponent<Enemy>();
        if(enemy == null)
        {
            if(collider.transform.parent == null)
            {
                return null;
            }

            enemy = collider.transform.parent.GetComponent<Enemy>();

            if(enemy == null)
            {
                return null;
            }
        }

        return enemy;
    }


    private void Trigger(Vector2 targetPosition)
    {
        m_CooldownTimer = cooldownTime;

        if(m_AbilityPool.Count == 0)
        {
            //Create new
            GameObject slashObject = new GameObject("Slash Ability");
            slashObject.transform.localPosition = Vector3.zero;
            slashObject.transform.localEulerAngles = Vector3.zero;
            slashObject.transform.localScale = Vector3.one;
            slashObject.transform.parent = transform;

            SlashAbility slashAbility = slashObject.AddComponent<SlashAbility>();
            slashAbility.onAttackComplete += () =>
            {
                PoolAbility(slashAbility);
            };

            slashAbility.Attack(targetPosition);
        }
        else
        {
            //Pop from pool
            SlashAbility ability = m_AbilityPool[m_AbilityPool.Count - 1];
            m_AbilityPool.RemoveAt(m_AbilityPool.Count - 1);
            ability.gameObject.SetActive(true);
            ability.Attack(targetPosition);
        }
    }

    private void PoolAbility(SlashAbility ability)
    {
        m_AbilityPool.Add(ability);
        ability.gameObject.SetActive(false);
    }
}

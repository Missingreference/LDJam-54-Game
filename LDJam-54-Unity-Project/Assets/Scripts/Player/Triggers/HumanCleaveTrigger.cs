using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCleaveTrigger : MonoBehaviour
{
    public CircleCollider2D trigger;

    public float cooldownTime = 0.5f;

    public Action onTrigger;

    private float m_CooldownTimer;

    private List<CleaveAbility> m_AbilityPool = new List<CleaveAbility>();
    private List<Enemy> m_EnemiesInRange = new List<Enemy>();
    private List<Action> m_RemoveFunctions = new List<Action>();

    void Awake()
    {
        gameObject.layer = 11;
        trigger = gameObject.AddComponent<CircleCollider2D>();
        trigger.radius = 10.0f;
        trigger.callbackLayers = 1 << 8;
        trigger.isTrigger = true;
    }

    private void Update()
    {
        if(m_CooldownTimer > 0.0f)
        {
            m_CooldownTimer -= Time.deltaTime;
        }
        else if(m_EnemiesInRange.Count > 0)
        {
            Enemy targetEnemy = PickNearestEnemy();
            Trigger(targetEnemy.transform.position);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Enemy enemy = GetEnemy(other);
        if(enemy != null && enemy.isAlive)
        {
            Action a = () =>
            {
                OnEnemyDeath(enemy);
            };
            enemy.onDeath += a;

            m_EnemiesInRange.Add(enemy);
            m_RemoveFunctions.Add(a);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Enemy enemy = GetEnemy(other);
        if(enemy != null)
        {
            for(int i = 0; i < m_EnemiesInRange.Count; i++)
            {
                if(m_EnemiesInRange[i] == enemy)
                {
                    m_EnemiesInRange.RemoveAt(i);
                    Action a = m_RemoveFunctions[i];
                    enemy.onDeath -= a;
                    m_RemoveFunctions.RemoveAt(i);

                }
            }
        }
    }

    private Enemy PickNearestEnemy()
    {
        float smallestDistance = Vector2.Distance(m_EnemiesInRange[0].transform.position, transform.position);
        int index = 0;
        for(int i = 1; i < m_EnemiesInRange.Count; i++)
        {
            float distance = Vector2.Distance(m_EnemiesInRange[i].transform.position, transform.position);
            if(distance < smallestDistance)
            {
                smallestDistance = distance;
                index = i;
            }
        }

        return m_EnemiesInRange[index];
    }

    private void OnEnemyDeath(Enemy enemy)
    {
        for(int i = 0; i < m_EnemiesInRange.Count; i++)
        {
            if(ReferenceEquals(m_EnemiesInRange[i], enemy))
            {
                m_EnemiesInRange.RemoveAt(i);
                Action a = m_RemoveFunctions[i];
                enemy.onDeath -= a;
                m_RemoveFunctions.RemoveAt(i);

            }
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
            slashObject.transform.parent = transform;
            slashObject.transform.localPosition = Vector3.zero;
            slashObject.transform.localEulerAngles = Vector3.zero;
            slashObject.transform.localScale = Vector3.one;

            CleaveAbility slashAbility = slashObject.AddComponent<CleaveAbility>();
            slashAbility.onAttackComplete += () =>
            {
                PoolAbility(slashAbility);
            };

            slashAbility.Attack(targetPosition);
        }
        else
        {
            //Pop from pool
            CleaveAbility ability = m_AbilityPool[m_AbilityPool.Count - 1];
            m_AbilityPool.RemoveAt(m_AbilityPool.Count - 1);
            ability.gameObject.SetActive(true);
            ability.Attack(targetPosition);
        }

        onTrigger?.Invoke();
    }

    private void PoolAbility(CleaveAbility ability)
    {
        m_AbilityPool.Add(ability);
        ability.gameObject.SetActive(false);
    }
}

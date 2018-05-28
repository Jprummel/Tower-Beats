﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public struct TowerData
{ 
    public string Type;
    public int Level;
    public int MaxLevel;
    public float UpgradeCost;
    public float BuyCost;
    public float SellValue;
    public float AttackDamage;
    public float AttackRange;
    public float AttackInterval;

    public TowerData(string type, int level, int maxLevel, float upgradeCost, float buyCost, float sellValue, float attackDamage, float attackRange, float attackInterval)
    {
        Type = type;
        Level = level;
        MaxLevel = maxLevel;
        UpgradeCost = upgradeCost;
        BuyCost = buyCost;
        SellValue = sellValue;
        AttackDamage = attackDamage;
        AttackRange = attackRange;
        AttackInterval = attackInterval;
    }
}

public class Tower : MonoBehaviour
{
    public static Action s_OnTargetsEmpty;
    protected TowerData m_TowerData;
    public TowerData TowerData { get; set; }
    [SerializeField]protected List<Enemy> m_EnemiesInRange = new List<Enemy>();
    protected Enemy m_Target;
    protected bool m_ReadyToAttack = true;
    protected bool m_StartedCooldown;
    private bool m_Paused;
    [SerializeField]
    protected GameObject attackProjectile;

    public Tower Self { get; set; } //Reference to itself

    //private LineRenderer m_LineRenderer;


    public virtual void Awake()
    {
        PauseCheck.Pause += TogglePause;
        Enemy.s_OnDestroyEnemy += RemoveEnemyFromList;
        //m_LineRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        GetTarget();
    }

    /// <summary>
    /// Sets the enemy that enters range first as target until it dies or leaves.
    /// After that the first enemy in the list (enemy that entered after the first) will become the new target
    /// </summary>
    /// <returns></returns>
    public Enemy GetTarget()
    {
        GetEnemiesInRange();
        if (m_EnemiesInRange.Count > 0)
        {
            m_Target = m_EnemiesInRange[0];
            return m_Target;
        }
        else
        {
            m_Target = null;
            if (s_OnTargetsEmpty != null)
                s_OnTargetsEmpty();
            return null;
        }
    }

    /// <summary>
    /// Gets all the enemies that are within the towers range
    /// </summary>
    /// <returns></returns>
    public List<Enemy> GetEnemiesInRange()
    {
        for (int i = 0; i < EnemySpawner.s_Instance.SpawnedEnemies.Count; i++)
        {
            Enemy enemy = EnemySpawner.s_Instance.SpawnedEnemies[i];

            if(enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);

                if (distance <= TowerData.AttackRange && !m_EnemiesInRange.Contains(enemy))
                {
                    m_EnemiesInRange.Add(enemy);
                }
                else if (distance >= TowerData.AttackRange && m_EnemiesInRange.Contains(enemy))
                {
                    RemoveEnemyFromList(enemy);
                }
            }
        }
        
        return m_EnemiesInRange;
    }

    void RemoveEnemyFromList(Enemy enemy)
    {
        m_EnemiesInRange.Remove(enemy);
    }

    public virtual void Attack()
    {
        //Do attack (done in child classes)
        //After tower has attacked, start cooldown
        if (!m_ReadyToAttack && !m_StartedCooldown)
        {
            StartCoroutine(AttackCooldown());
        }
    }

    private void TogglePause(bool Pause)
    {
        m_Paused = Pause;
    }

    IEnumerator AttackCooldown()
    {
        float timer = 0;
        m_StartedCooldown = true;
        while(timer <= TowerData.AttackInterval)
        {
            if(!m_Paused)
            {
                timer += Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }
        m_ReadyToAttack = true;
    }

    private void OnDestroy()
    {
        Enemy.s_OnDestroyEnemy -= RemoveEnemyFromList;
    }
}
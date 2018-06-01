﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

public class Enemy : PoolObj
{
    [SerializeField] private string m_EnemyString;
    public string EnemyString
    {
        get { return m_EnemyString; }
    }

    [SerializeField]private ParticleSystem m_DeathParticle;
    public delegate void DestroyEvent(Enemy enemy);
    public static DestroyEvent s_OnDestroyEnemy;
    [SerializeField]
    private float m_MaxHealth = 20;
    public float CurrentHealth { get; set; }
    public bool IsAlive { get; set; }

    public SkeletonAnimation SkeletonAnims { get; set; }
    //private Tween m_dopath;
    private AnimationState m_Anim;
    private EnemyHealthbar m_EnemyHealthbar;
    private MeshRenderer m_Renderer;

    [SerializeField] private float m_MoveSpeed;
    [SerializeField] private float m_CoinsToGive;

    private int m_CurrentNodeIndex;

    private void Awake()
    {
        CurrentHealth = m_MaxHealth;
        SkeletonAnims = GetComponent<SkeletonAnimation>();
        m_Renderer = GetComponent<MeshRenderer>();

        PauseCheck.Pause += TogglePause;

        m_EnemyHealthbar = GetComponent<EnemyHealthbar>();
    }

    private void OnEnable()
    {
        GameManager.s_OnGameStop += Death;
    }

    public void RestoreHealth()
    {
        CurrentHealth = m_MaxHealth;
        m_EnemyHealthbar.ChangeEnemyHealthUI(CurrentHealth / m_MaxHealth);
    }

    public void TakeDamage(float damage, string towerType)
    {
        if (IsAlive)
        {
            CurrentHealth -= damage;
            m_EnemyHealthbar.ChangeEnemyHealthUI(CurrentHealth / m_MaxHealth);

            if (CurrentHealth <= 0)
            {
                StartCoroutine(Death(true));
            }
            else if (CurrentHealth > 0)
            {
                switch (towerType)
                {
                    case "Bass":
                        SkeletonAnims.AnimationState.SetAnimation(0, m_EnemyString + "HIT_Electricity", false);
                        SkeletonAnims.AnimationState.AddAnimation(0, m_EnemyString + "MOVE", true, 0);
                        break;
                    case "Drum":
                        EffectsManager.s_Instance.SpawnEffect(EffectType.ENEMY_HIT, false, new Vector2(transform.position.x, transform.position.y + 0.5f));
                        break;
                    case "Lead":
                        EffectsManager.s_Instance.SpawnEffect(EffectType.ENEMY_HIT, false, new Vector2(transform.position.x, transform.position.y + 0.5f));
                        break;
                }
            }
        }
    }

    /// <summary>
    /// This gets added to the s_OnPlayListComplete delegate and won't give the player any coins for enemies that died this way.
    /// </summary>
    public void Death()
    {
        StartCoroutine(Death(false));
    }

    public IEnumerator Death(bool killedByPlayer)
    {
        DOTween.Kill(this);

        if (s_OnDestroyEnemy != null)
        {
            s_OnDestroyEnemy(this);
        }
        IsAlive = false;

        //If player kills the enemy
        if (killedByPlayer)
        {
            //Give coins
            PlayerData.s_Instance.ChangeCoinAmount(m_CoinsToGive);
        }

        m_DeathParticle.gameObject.SetActive(true);
        m_DeathParticle.Play();

        SkeletonAnims.AnimationState.SetAnimation(0, m_EnemyString + "DEATH", false).OnComplete();

        float animTime = SkeletonAnims.skeleton.data.FindAnimation(m_EnemyString + "DEATH").duration;

        yield return new WaitForSeconds(animTime);

        DeathCallback();
    }

    private void DeathCallback()
    {
        m_DeathParticle.gameObject.SetActive(false);
        ReturnToPool();
    }

    public void DamageObjective()
    {
        Effects.s_Screenshake(0.2f, 20);

        if (PlayerData.s_Instance.Lives > 0)
        {
            PlayerData.s_Instance.ChangeLivesAmount(-1);

            DOTween.Kill(this);
            ReturnToPool();
        }
    }

    public void Move(Vector3 startPos)
    {
        if (IsAlive)
        {
            DOTween.Kill(this);
            transform.position = startPos;
            Vector3[] pathArray = MapLoader.s_Instance.GetWaypointsFromPath();
            transform.DOPath(pathArray, pathArray.Length / m_MoveSpeed, PathType.CatmullRom).SetEase(Ease.Linear).SetId(this).OnComplete(() => DamageObjective()).OnWaypointChange(UpdateEnemyLayering);
        }
    }

    private void UpdateEnemyLayering(int waypointIndex)
    {
        m_Renderer.sortingOrder = HexGrid.s_Instance.GridSize.y - MapLoader.s_Instance.Path[waypointIndex].PositionInGrid.y;
    }


    public void TogglePause(bool pause)
    {
        if (pause)
        {
            DOTween.Pause(this);
        }
        else
        {
            DOTween.Play(this);
        }
    }

    private void OnDisable()
    {
        GameManager.s_OnGameStop -= Death;
    }

    void DeathRoutine()
    {

    }
}
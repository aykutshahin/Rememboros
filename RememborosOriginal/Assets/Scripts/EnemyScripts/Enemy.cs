using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rememboros;

public class Enemy : EnemyRenderer2D
{
    public void Init()
    {
        //Timer
        m_Timer = GetComponent<Timer>();
        m_Timer.addTimer("Attack", 1f, m_Attack.enemyAttackCooldown);

        m_General.currentHealth = m_General.Health;

        m_General.enemyAnimator = GetComponent<Animator>();

        enemyState = EnemyState.Patroling;

        enemyIsFacingRight = true;

    }

    private void Awake()
    {
        m_InputDriver = GetComponent<BaseInputDriver>();
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<BoxCollider2D>();
        m_Motor = GetComponent<CharacterMotor2D>();
    }
    void Start()
    {
        Init();
    }


    // Update is called once per frame
    void Update()
    {
        UpdateStates();
        SetTargetType();
    }
    private void FixedUpdate()
    {
        m_Motor.Move(new Vector2(Move().x, -m_General.gravity) * Time.fixedDeltaTime,false);
        UpdateTimers();
        SetEnemyState();
        ChangeTargetLocations();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

}

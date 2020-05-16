using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GY_SkeletonAnimationController : EnemyAnimationController
{
    private Enemy m_Controller;

    // Start is called before the first frame update
    void Awake()
    {
        m_Controller = GetComponent<Enemy>();

        m_Animator = GetComponent<Animator>();

        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        m_Controller.OnFacingFlip += onFacingFlip;

        m_Controller.OnAttack += onAttack;
        m_Controller.OnAttackEnd += onAttackEnd;

        m_Controller.OnChase += onChase;
        m_Controller.OnChaseEnd += onChaseEnd;

        m_Controller.OnPatrol += onPatrol;
        m_Controller.OnPatrolEnd += onPatrolEnd;

        m_Controller.OnIdle += onIdle;
        m_Controller.OnIdleEnd += onIdleEnd;
    }

    // Update is called once per frame
    void Update()
    {
        m_Animator.SetBool("isChasing", m_Controller.IsChasing());

        m_Animator.SetBool("isAttacking", m_Controller.IsAttacking());

        m_Animator.SetBool("isPatrolling", m_Controller.IsPatrolling());

        m_Animator.SetBool("isIdling", m_Controller.IsIdling());
    }
}

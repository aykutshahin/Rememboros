using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{ 
    protected SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    protected Animator m_Animator;

    protected virtual void onFacingFlip(int facing)
    {
        transform.localScale = new Vector3(facing, 1, 0);
    }

    protected virtual void onAttack()
    {
    }

    protected virtual void onAttackEnd()
    {
    }

    protected virtual void onPatrol()
    {
    }

    protected virtual void onPatrolEnd()
    {
    }

    protected virtual void onChase()
    {
    }

    protected virtual void onChaseEnd()
    {
    }

    protected virtual void onIdle()
    {
    }

    protected virtual void onIdleEnd()
    {
    }
}

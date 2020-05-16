using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rememboros;

public class PlayerAnimationController : MonoBehaviour
{
    private Player m_Controller;

    private SpriteRenderer m_SpriteRenderer;

    [SerializeField]
    private Animator m_Animator;

    private void Awake()
    {
        m_Controller = GetComponent<Player>();

        m_Animator = GetComponent<Animator>();

        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        m_Controller.OnFacingFlip += onFacingFlip;
        m_Controller.OnDash += onDash;
        m_Controller.OnDashStay += onDashStay;
        m_Controller.OnDashEnd += onDashEnd;
        m_Controller.OnWallJump += onWallJump;

        m_Controller.OnWallSliding += onWallSliding;
        m_Controller.OnWallSlidingEnd += onWallSlidingEnd;

        m_Controller.OnAttack += onAttack;
        m_Controller.OnAttackEnd += onAttackEnd;
    }

    private void Update()
    {
        if (Mathf.Abs(m_Controller.InputVelocity.x) > 0.0f)
            m_Animator.SetFloat("SpeedX", 1.0f);
        else
            m_Animator.SetFloat("SpeedX", 0.0f);

        m_Animator.SetFloat("SpeedY", m_Controller.InputVelocity.y);

        m_Animator.SetBool("IsGrounded", m_Controller.IsState(MotorState.OnGround));

        m_Animator.SetBool("TouchingGround", m_Controller.IsOnGround());

        m_Animator.SetBool("IsOnWall", m_Controller.IsState(MotorState.WallSliding));

        m_Animator.SetBool("IsAttacking", m_Controller.IsState(MotorState.Attack));

        m_Animator.SetBool("IsAirAttacking", m_Controller.IsState(MotorState.AirAttack));

        m_Animator.SetBool("IsFalling", m_Controller.IsState(MotorState.Falling));

        m_Animator.SetBool("IsJumping", m_Controller.IsState(MotorState.Jumping));

        m_Animator.SetInteger("clicksOfAttack", m_Controller.m_Attack.noOfClicks);

        m_Animator.SetInteger("clicksOfAirAttack", m_Controller.m_Attack.noOfClicksAir);

        m_Animator.SetBool("IsGrabbingLedge", m_Controller.IsState(MotorState.OnLedge));
    }

    private void onFacingFlip(int facing)
    {
        m_Controller.transform.localScale = new Vector3(facing, 1, 0);
    }

    private void onWallJump(Vector2 vec)
    {
        // TODO
    }

    private void onWallSliding(int wallDir)
    {
        //m_SpriteRenderer.flipX = true;
    }

    private void onWallSlidingEnd()
    {
        //m_SpriteRenderer.flipX = false;
    }

    private void onDash(int dashDir)
    {
        m_Animator.SetBool("IsDashing", true);
        m_Animator.Play("StartDash");
    }

    private void onDashStay(float progress)
    {
        m_Animator.SetFloat("DashProgress", progress);
    }

    private void onDashEnd()
    {
        m_Animator.SetBool("IsDashing", false);
    }

    private void onAttack()
    {
    }

    private void onAttackEnd()
    {
    }
}

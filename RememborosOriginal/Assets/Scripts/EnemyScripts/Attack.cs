using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : StateMachineBehaviour
{
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Enemy>().canFlip = false;
        if(animator.GetComponent<Enemy>().noOfClicks == 1)
        animator.GetComponent<Enemy>().noOfClicks += 1;
    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<Enemy>().EnemyAttack();
    }

    //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<Enemy>().noOfClicks >= 2)
            animator.GetComponent<Enemy>().noOfClicks = 1;
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Attack2");
        animator.GetComponent<Enemy>().canFlip = true;
    }
}
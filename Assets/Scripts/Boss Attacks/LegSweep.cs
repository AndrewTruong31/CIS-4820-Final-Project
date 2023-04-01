using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegSweep : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    //}

    public GameObject hitbox;
    private GameObject temp;

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
    )
    {
        if (stateInfo.normalizedTime > 0.40 && stateInfo.normalizedTime < 0.6)
        {
            GameObject enemy = GameObject.Find("Boss");

            if (temp == null)
            {
                temp = Instantiate(
                    hitbox,
                    enemy.transform.position + (enemy.transform.forward * 6),
                    Quaternion.Euler(
                        new Vector3(
                            (enemy.transform.eulerAngles.x + -90),
                            enemy.transform.eulerAngles.y,
                            enemy.transform.eulerAngles.z
                        )
                    )
                );
            }
        }
        else
        {
            if (temp != null)
            {
                Destroy(temp);
            }
        }
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class AoESlam : StateMachineBehaviour
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
            if (stateInfo.normalizedTime > 0.5 && stateInfo.normalizedTime < 0.7)
            {
                GameObject player = GameObject.Find("PlayerArmature");
                GameObject enemy = GameObject.Find("Boss");
                Vector3 toPlayer = player.transform.position - enemy.transform.position;
                float dist = toPlayer.magnitude;

                if (temp == null)
                {
                    temp = Instantiate(hitbox, enemy.transform.position, Quaternion.identity);
                }
            }
            else
            {
                if (temp)
                {
                    Destroy(temp);
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        // override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        // {
        //    Debug.Log("Finished Attack");
        // }

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
}

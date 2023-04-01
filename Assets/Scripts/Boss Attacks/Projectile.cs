using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class Projectile : StateMachineBehaviour
    {
        public GameObject proj;
        private GameObject temp1;
        private GameObject temp2;
        private GameObject temp3;
        int _animIDNumProj;

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateEnter(
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex
        )
        {
            _animIDNumProj = Animator.StringToHash("NumProj");
            animator.SetFloat(_animIDNumProj, 0.0f);
        }

        override public void OnStateUpdate(
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex
        )
        {
            if (
                stateInfo.normalizedTime >= (0.40f) & stateInfo.normalizedTime <= (0.55f)
                && temp1 == null
            )
            {
                GameObject player = GameObject.Find("Boss");
                temp1 = Instantiate(
                    proj,
                    player.transform.position
                        + (player.transform.forward * 1.5f)
                        + (player.transform.up * 1.5f),
                    Quaternion.identity
                );
                animator.SetFloat(_animIDNumProj, animator.GetFloat(_animIDNumProj) + 1);
            }

            if (
                stateInfo.normalizedTime >= (1.40f) & stateInfo.normalizedTime <= (1.55f)
                && temp2 == null
            )
            {
                GameObject player = GameObject.Find("Boss");
                temp2 = Instantiate(
                    proj,
                    player.transform.position
                        + (player.transform.forward * 1.5f)
                        + (player.transform.up * 1.5f),
                    Quaternion.identity
                );
                animator.SetFloat(_animIDNumProj, animator.GetFloat(_animIDNumProj) + 1);
            }

            if (
                stateInfo.normalizedTime >= (2.40f) & stateInfo.normalizedTime <= (2.55f)
                && temp3 == null
            )
            {
                GameObject player = GameObject.Find("Boss");
                temp3 = Instantiate(
                    proj,
                    player.transform.position
                        + (player.transform.forward * 1.5f)
                        + (player.transform.up * 1.5f),
                    Quaternion.identity
                );
                animator.SetFloat(_animIDNumProj, animator.GetFloat(_animIDNumProj) + 1);
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex
        )
        {
            temp1 = null;
            temp2 = null;
            temp3 = null;
        }

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

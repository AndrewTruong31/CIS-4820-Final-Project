using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class FrontBack : StateMachineBehaviour
    {
        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //
        //}
        public GameObject hitbox;
        private GameObject temp1;
        private GameObject temp2;

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

                Vector3 relative = enemy.transform.InverseTransformPoint(player.transform.position);
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

                if (
                    dist < 5
                    && (
                        (angle > -45 && angle < 45)
                        || (angle > 135 && angle < 225)
                        || (angle < -135 && angle > -225)
                    )
                )
                {
                    PlayerStatus playerStatus = player.GetComponent<PlayerStatus>();

                    if (playerStatus != null)
                    {
                        playerStatus.ApplyDamage(3);
                    }
                    else
                    {
                        Debug.Log("Couldn't find status");
                    }
                }
                if (temp1 == null && temp2 == null)
                {
                    temp1 = Instantiate(
                        hitbox,
                        enemy.transform.position
                            + (enemy.transform.forward * 4)
                            + (enemy.transform.up * 2),
                        Quaternion.Euler(
                            new Vector3(
                                enemy.transform.eulerAngles.x,
                                enemy.transform.eulerAngles.y,
                                enemy.transform.eulerAngles.z
                            )
                        )
                    );
                    temp2 = Instantiate(
                        hitbox,
                        enemy.transform.position
                            + (enemy.transform.forward * -4)
                            + (enemy.transform.up * 2),
                        Quaternion.Euler(
                            new Vector3(
                                enemy.transform.eulerAngles.x,
                                enemy.transform.eulerAngles.y,
                                enemy.transform.eulerAngles.z
                            )
                        )
                    );
                }
            }
            else
            {
                if (temp1)
                {
                    Destroy(temp1);
                }
                if (temp2)
                {
                    Destroy(temp2);
                }
            }
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex
        )
        {
            animator.SetBool(Animator.StringToHash("FacePlayer"), true);
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

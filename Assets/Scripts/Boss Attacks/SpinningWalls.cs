using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class SpinningWalls : StateMachineBehaviour
    {
        public GameObject hitbox;
        public GameObject hitbox1;
        private GameObject temp1;
        private GameObject temp2;
        private GameObject temp3;

        override public void OnStateUpdate(
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex
        )
        {
            if (stateInfo.normalizedTime > 0.5 && stateInfo.normalizedTime < 0.7)
            {
                GameObject enemy = GameObject.Find("Boss");

                if (temp1 == null && temp2 == null && temp3 == null)
                {
                    temp1 = Instantiate(
                        hitbox,
                        enemy.transform.position
                            + (enemy.transform.right * -11)
                            + (enemy.transform.up * 5),
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
                            + (enemy.transform.right * 11)
                            + (enemy.transform.up * 5),
                        Quaternion.Euler(
                            new Vector3(
                                enemy.transform.eulerAngles.x,
                                enemy.transform.eulerAngles.y,
                                enemy.transform.eulerAngles.z
                            )
                        )
                    );
                    temp3 = Instantiate(
                        hitbox1,
                        enemy.transform.position + (enemy.transform.up * 1.5f),
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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class Laser : StateMachineBehaviour
    {
        public GameObject proj;
        private GameObject temp;

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(
            Animator animator,
            AnimatorStateInfo stateInfo,
            int layerIndex
        )
        {
            if (stateInfo.normalizedTime >= 0.50f & stateInfo.normalizedTime <= 0.7f)
            {
                GameObject boss = GameObject.Find("Boss");
                GameObject player = GameObject.Find("PlayerArmature");
                RaycastHit hit;

                if (
                    Physics.Raycast(
                        boss.transform.position + (boss.transform.forward * 2) + boss.transform.up,
                        boss.transform.TransformDirection(Vector3.forward),
                        out hit,
                        Mathf.Infinity
                    )
                )
                {
                    Debug.Log("Did Hit: " + hit.collider);
                    Debug.DrawRay(
                        boss.transform.position + boss.transform.up + (boss.transform.forward),
                        boss.transform.TransformDirection(Vector3.forward) * hit.distance,
                        Color.yellow
                    );

                    if (temp == null)
                        temp = Instantiate(
                            proj,
                            boss.transform.position
                                + (boss.transform.forward * hit.distance)
                                + (boss.transform.forward)
                                + (boss.transform.up * 1.5f),
                            (
                                Quaternion.Euler(
                                    new Vector3(
                                        90,
                                        boss.transform.eulerAngles.y,
                                        boss.transform.eulerAngles.z
                                    )
                                )
                            )
                        );
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
            Destroy(temp);
        }
    }
}

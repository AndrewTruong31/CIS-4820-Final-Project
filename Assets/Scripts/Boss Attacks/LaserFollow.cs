using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class LaserFollow : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start() { }

        // Update is called once per frame
        void Update()
        {
            GameObject boss = GameObject.Find("Boss");

            GameObject player = GameObject.Find("PlayerArmature");
            RaycastHit hit;
            float dist = 20;

            if (
                Physics.Raycast(
                    boss.transform.position + (boss.transform.forward * 2) + boss.transform.up,
                    boss.transform.TransformDirection(Vector3.forward),
                    out hit,
                    Mathf.Infinity
                )
            )
            {
                if (hit.collider.name != "PlayerArmature")
                    dist = hit.distance;
                else
                {
                    dist = 20;
                }
                transform.localScale = new Vector3(2.0f, (dist / 2), 2.0f);
            }

            transform.position =
                boss.transform.position
                + (boss.transform.forward * (dist / 2))
                + (boss.transform.forward * 2)
                + (boss.transform.up * 1.5f);

            transform.rotation = Quaternion.Euler(
                new Vector3(90, boss.transform.eulerAngles.y, boss.transform.eulerAngles.z)
            );
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.name == "PlayerArmature")
            {
                PlayerStatus playerStatus = other.gameObject.GetComponent<PlayerStatus>();
                playerStatus.ApplyDamage(3);
            }
        }
    }
}

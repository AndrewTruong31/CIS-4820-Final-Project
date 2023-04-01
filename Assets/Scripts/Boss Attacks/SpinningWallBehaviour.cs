using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class SpinningWallBehaviour : MonoBehaviour
    {
        // Start is called before the first frame update
        private float _spinDeltaTime;
        private GameObject boss;

        void Start()
        {
            _spinDeltaTime = 10.0f;
            boss = GameObject.Find("Boss");
            boss.GetComponent<AIController>().IsAttacking = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (_spinDeltaTime >= 0.0f)
            {
                transform.RotateAround(
                    boss.transform.position,
                    new Vector3(0, 1, 0),
                    45 * Time.deltaTime
                );
            }

            _spinDeltaTime -= Time.deltaTime;

            if (_spinDeltaTime <= 0.0f)
            {
                Destroy(gameObject);
                GameObject center = GameObject.Find("Center(Clone)");
                if (center != null)
                {
                    Destroy(center);
                }
                boss.GetComponent<AIController>().IsAttacking = true;
            }
        }
    }
}

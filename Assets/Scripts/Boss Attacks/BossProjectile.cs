using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class BossProjectile : MonoBehaviour
    {
        public float speed = 8;
        public float lifespan = 3.0f;

        private float _lifespanDelta;
        GameObject player;
        Collider proj;
        Vector3 dir;

        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.Find("Boss");
            proj = GetComponent<Collider>();
            _lifespanDelta = lifespan;
            dir = player.transform.forward;
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(dir * Time.deltaTime * speed);

            if (_lifespanDelta >= 0)
            {
                _lifespanDelta -= Time.deltaTime;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.name == "PlayerArmature")
            {
                PlayerStatus playerStatus = other.gameObject.GetComponent<PlayerStatus>();
                playerStatus.ApplyDamage(3);

                Destroy(gameObject);
            }
            else if (other.collider.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }
        }
    }
}

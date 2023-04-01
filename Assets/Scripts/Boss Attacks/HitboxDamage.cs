using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StarterAssets
{
    public class HitboxDamage : MonoBehaviour
    {
        public float hitboxDamage;

        // Start is called before the first frame update
        void Start() { }

        // Update is called once per frame
        void Update() { }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name == "PlayerArmature")
            {
                PlayerStatus playerStatus = other.gameObject.GetComponent<PlayerStatus>();
                playerStatus.ApplyDamage(3);
            }
        }
    }
}

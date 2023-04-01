using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace StarterAssets
{
    public class AIStatus : MonoBehaviour
    {
        [SerializeField]
        private float health = 30.0f;

        [SerializeField]
        private float maxHealth = 30.0f;

        //Uncomment if we need to apply a power-up to the AI
        //private float	maxHealth = 10.0f;

        private bool dead = false;
        private AIController aiController;
        private int phase = 0;

        [SerializeField]
        private bool isInvincible = false;

        [SerializeField]
        private float curImmunityTime = 0;
        public float immunityDur = 1f;

        [SerializeField]
        private Image healthBar;

        [SerializeField]
        private TextMeshProUGUI percentHP;

        void Start()
        {
            aiController = GetComponent<AIController>();
        }

        public bool isAlive()
        {
            return !dead;
        }

        public float ImmunityTime
        {
            get { return curImmunityTime; }
            set { curImmunityTime = value;
                    isInvincible = true; }
        }

        public void ApplyDamage(float damage)
        {
            if (!isInvincible)
            {
                isInvincible = true;
                curImmunityTime = immunityDur;
                health -= damage;
                healthBar.fillAmount = health/maxHealth;
                percentHP.text = Mathf.Round((health/maxHealth) * 100).ToString() + "%";
                if (health <= 0 && !aiController.IsDead)
                {
                    dead = true;
                    health = 0;
                    print("***********Dead!*************");
                    aiController.IsDead = true;
                    aiController.IsPhase = 3;
                }
            }
            Debug.Log(
                "Enemy NPC took " + damage + " damage " + health + "hp/" + maxHealth + "hp remains."
            );
        }

        void Update()
        {
            if (isInvincible)
            {
                curImmunityTime -= Time.deltaTime;

                if (curImmunityTime < immunityDur)
                {
                    isInvincible = false;
                }
            }

            if (health <= (maxHealth / 3))
            {
                if (phase != 2)
                {
                    aiController.IsPhase = 2;
                    phase = 2;
                }
            }
            else if (health <= ((maxHealth / 3) * 2))
            {
                if (phase != 1)
                {
                    aiController.IsPhase = 1;
                    phase = 1;
                }
            }else if (health <= 0){
                dead = true;
                aiController.IsPhase = 4;
            }
        }

        public float getHealth()
        {
            return health;
        }

        public float getMaxHealth()
        {
            return maxHealth;
        }
    }
}

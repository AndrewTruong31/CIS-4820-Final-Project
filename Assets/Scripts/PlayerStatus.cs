using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

namespace StarterAssets
{
    public class PlayerStatus : MonoBehaviour
    {
        [Header("Player Stats")]
        [Tooltip("Current health of player")]
        [SerializeField]
        private float health = 100.0f;

        [Tooltip("Max health of player")]
        [SerializeField]
        private float maxHealth = 100.0f;
        [SerializeField]
        private int numHealthPotions = 3;

        private bool dead = false;
        [SerializeField]
        private bool isInvinceible = false;
        [SerializeField]
        private float curImmunityTime = 0;
        public float immunityDur = 0.5f;

        [SerializeField]
        private Image healthBar;

        [SerializeField]
        private TextMeshProUGUI numPotionText;

        private ThirdPersonController playerController;

        public void AddHealth(float moreHealth)
        {
            if(numHealthPotions > 0){
                if (health + moreHealth > maxHealth){
                    health = maxHealth;
                }else{
                    health += moreHealth;
                }
                healthBar.fillAmount = health/maxHealth;
                numHealthPotions -= 1;
                numPotionText.text = ("x" + numHealthPotions.ToString());
            }
        }

        public float GetHealth()
        {
            return health;
        }

        public float GetMaxHealth()
        {
            return maxHealth;
        }

        public bool IsInvinceible
        {
            get { return isInvinceible; }
            set { isInvinceible = value; }
        }

        public float CurImmunityTime
        {
            get { return curImmunityTime; }
            set { curImmunityTime = value; }
        }

        void Start()
        {
            playerController = GetComponent<ThirdPersonController>();
        }

        void Update(){
            if(Input.GetKeyDown(KeyCode.Q)){
                AddHealth(10);
            }
            if (isInvinceible)
            {
                curImmunityTime -= Time.deltaTime;

                if (curImmunityTime < 0)
                {
                    isInvinceible = false;
                }
            }
        }

        public bool isAlive()
        {
            return !dead;
        }

        public void ApplyDamage(float damage)
        {
            if (!isInvinceible)
            {
                isInvinceible = true;
                curImmunityTime = immunityDur;

                health -= damage;
                healthBar.fillAmount = health/maxHealth;
                //Debug.Log("Ouch! " + health);
                if (health <= 0)
                {
                    health = 0;
                    playerController.IsDead = true;
                    Debug.Log("DEAD");
                }
            }
        }

        public void PowerUp(int amount)
        {
            health += amount;
            maxHealth += amount;
        }

        void HideCharacter()
        {
            playerController.IsControllable = false;
        }

        void ShowCharacter()
        {
            playerController.IsControllable = true;
        }
    }
}

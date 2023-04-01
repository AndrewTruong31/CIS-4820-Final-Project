using UnityEngine;
using System.Collections;

public enum StateType
{
    state_RunAway,
    state_Patrol,
    state_Attack
};

public enum EventType
{
    event_Weaker,
    event_Threatened_Stronger,
    event_Threatened_Weaker,
    Safe
};

namespace StarterAssets
{
    public class AIController : MonoBehaviour
    {
        // private enum StateType{state_RunAway, state_Patrol, state_Attack};
        // private enum EventType{event_Weaker, event_Threatened_Stronger, event_Threatened_Weaker, Safe};

        // State[,] stateMatrix = new State[,]{
        // 	{StateType.state_RunAway, StateType.state_Patrol, StateType.state_RunAway},
        // 	{StateType.state_RunAway, StateType.state_Attack, StateType.state_Attack},
        // 	{StateType.state_RunAway, StateType.state_RunAway, StateType.state_Attack},
        // 	{StateType.state_Patrol, StateType.state_Patrol, StateType.state_Attack}

        // };
        [Header("Enemy Stats")]
        [Tooltip("Attack Cooldown")]
        public float attackCooldown = 10.0f;

        public float attackDistance = 2.0f;
        public float attackDamage = 2f;

        private CharacterController controller;
        private PlayerStatus playerStatus;
        private Transform target;
        private Vector3 moveDirection = new Vector3(0, 0, 0);

        [SerializeField]
        private int phase = 0;
        private bool transition = false;

        // private Animation		anim;

        private StateType curState;

        private bool isControllable = true;
        private bool isDead = false;
        private bool isAttacking = true;

        [SerializeField]
        private float _attackCooldownDelta;

        // animation IDs
        private int _animIDRotation;
        private int _animIDAttack; //Attack number
        private int _animIDReady;
        private int _animIDFacePlayer;
        private int _animIDPhase;
        private int _animIDTransition;

        [SerializeField]
        private float _currentAttack;
        private Animator _animator;
        private bool _hasAnimator;

        private GameObject p2_floor;
        private GameObject p3_floor;
        private AIStatus aiStatus;

        public bool IsControllable
        {
            get { return isControllable; }
            set { isControllable = value; }
        }

        public bool IsDead
        {
            get { return isDead; }
            set { isDead = value; }
        }

        public bool IsAttacking
        {
            get { return isAttacking; }
            set { isAttacking = value; }
        }

        // Use this for initialization
        void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            controller = GetComponent<CharacterController>();
            aiStatus = GetComponent<AIStatus>();
            // anim = GetComponent<Animation>();
            GameObject tmp = GameObject.FindWithTag("Player");
            if (tmp != null)
            {
                target = tmp.transform;
                playerStatus = tmp.GetComponent<PlayerStatus>();
            }

            AssignAnimationIDs();

            _attackCooldownDelta = attackCooldown;

            p2_floor = GameObject.Find("P2");
            p2_floor.SetActive(false);
            p3_floor = GameObject.Find("P3");
            p3_floor.SetActive(false);
        }

        void Update()
        {
            // Debug.Log(curState);
            if (!isControllable)
                return;
            _hasAnimator = TryGetComponent(out _animator);

            calculateAttack();

            if (_animator.GetBool(_animIDFacePlayer))
            {
                FacePlayer();
            }

            PhaseChange(phase);

            // currentState.Execute(this);
            // moveDirection.y -= gravity*Time.deltaTime;
            // controller.Move(moveDirection * Time.deltaTime);


            // updateState();
        }

        public int IsPhase
        {
            get { return phase; }
            set
            {
                phase = value;
                _animator.SetFloat(_animIDPhase, phase);
                transition = true;
                _animator.SetFloat(_animIDTransition, phase);
                _animator.SetBool(_animIDFacePlayer, false);
                aiStatus.ImmunityTime = 999f;
            }
        }

        private void AssignAnimationIDs()
        {
            _animIDRotation = Animator.StringToHash("Rotation");
            _animIDAttack = Animator.StringToHash("Attack");
            _animIDReady = Animator.StringToHash("Ready");
            _animIDFacePlayer = Animator.StringToHash("FacePlayer");
            _animIDPhase = Animator.StringToHash("Phase");
            _animIDTransition = Animator.StringToHash("Transition");
        }

        private void PhaseChange(int phase)
        {
            string cur_anim = _animator?.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            if (phase == 1 && transition && cur_anim == "Kneel")
            {
                Debug.Log("Transitioning - P1");
                GameObject walls = GameObject.Find("P1 - Walls");
                GameObject floor = GameObject.Find("Ground");
                GameObject player = GameObject.Find("PlayerArmature");
                CharacterController controller = player.GetComponent<CharacterController>();
                GameObject boss = GameObject.Find("Boss");

                if (walls != null)
                {
                    walls.transform.Translate(walls.transform.up * -1.5f * Time.deltaTime);

                    if (walls.transform.position.y <= -7)
                    {
                        Destroy(walls);
                    }
                }
                else if (p2_floor.activeSelf == false)
                {
                    p2_floor.SetActive(true);
                }
                else if (p2_floor?.transform.position.y < 0)
                {
                    p2_floor.transform.Translate(p2_floor.transform.up * 3 * Time.deltaTime);
                }
                else if (
                    Mathf.Abs(player.transform.position.x) < 15
                    && Mathf.Abs(player.transform.position.z) < 15
                )
                {
                    if (controller.enabled == true)
                    {
                        controller.enabled = false;
                        ThirdPersonController temp = player.GetComponent<ThirdPersonController>();
                        temp.IsMoveable = false;
                    }

                    Vector3 dir = (player.transform.position - boss.transform.position).normalized;
                    dir = new Vector3(dir.x, 0, dir.z);

                    player.transform.Translate(dir * 5 * Time.deltaTime, Space.World);
                }
                else if (floor?.transform.position.y > -5)
                {
                    floor.transform.Translate(p2_floor.transform.up * -3 * Time.deltaTime);
                }
                else
                {
                    Destroy(floor);
                    // Transition Finished
                    if (p2_floor.transform.position.y >= 0 && walls == null && floor == null)
                    {
                        transition = false;
                        _animator.SetFloat(_animIDTransition, 0.0f);
                        _animator.SetBool(_animIDFacePlayer, true);
                        aiStatus.ImmunityTime = 0f;
                        if (controller.enabled == false)
                        {
                            controller.enabled = true;
                            ThirdPersonController temp =
                                player.GetComponent<ThirdPersonController>();
                            temp.IsMoveable = true;
                        }
                    }
                }
            }else if(phase == 2 && transition && cur_anim == "Dropping"){
                Debug.Log("Transitioning - P2");
                GameObject player = GameObject.Find("PlayerArmature");
                // CharacterController controller = player.GetComponent<CharacterController>();
                GameObject boss = GameObject.Find("Boss");
                GameObject floor = GameObject.Find("P3 - Ground");
                if (p3_floor.activeSelf == false)
                {
                    p3_floor.SetActive(true);
                }else if (floor?.transform.position.y < 0){
                    floor.transform.Translate(floor.transform.up * 3 * Time.deltaTime);
                }else{
                    Destroy(p2_floor);

                    if (p2_floor == null && p3_floor.transform.position.y >= 0)
                    {
                        transition = false;
                        _animator.SetFloat(_animIDTransition, 0.0f);
                        _animator.SetBool(_animIDFacePlayer, true);
                        aiStatus.ImmunityTime = 0f;
                    }
                    
                }
            }else if (phase == 3 && transition){

            }
        }

        public void BeIdle()
        {
            // anim.CrossFade("idle", 0.2f);
            moveDirection = new Vector3(0, 0, 0);
        }

        public void calculateAttack()
        {
            // Debug.Log(_attackCooldownDelta);
            if (isAttacking)
            {
                if (_attackCooldownDelta <= 0)
                {
                    _attackCooldownDelta = attackCooldown;

                    if (_currentAttack > 0)
                    {
                        Debug.Log(_currentAttack);
                        _animator.SetFloat(_animIDAttack, _currentAttack);
                    }
                }
                else if (_attackCooldownDelta >= 0.0f)
                {
                    _animator.SetFloat(_animIDAttack, 0.0f);

                    if (_attackCooldownDelta <= (attackCooldown * 0.3))
                    {
                        float dist = distToPlayer();
                        if (dist < 5 && _currentAttack == 0)
                        {
                            _currentAttack = (float)Random.Range(1, 3);
                            _animator.SetFloat(_animIDReady, _currentAttack);
                        }
                        else if (dist >= 5 && _currentAttack == 0)
                        {
                            _currentAttack = (float)Random.Range(3, 5);
                            _animator.SetFloat(_animIDReady, _currentAttack);
                        }

                        if (_currentAttack == 2.0f)
                        {
                            _animator.SetBool(_animIDFacePlayer, false);
                        }
                    }
                    else
                    {
                        _animator.SetFloat(_animIDReady, 0.0f);
                        _currentAttack = 0.0f;
                    }

                    _attackCooldownDelta -= Time.deltaTime;
                }
            }
        }

        void FacePlayer()
        {
            GameObject player = GameObject.Find("PlayerArmature");
            Vector3 relative = transform.InverseTransformPoint(player.transform.position);
            float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

            transform.Rotate(0, angle, 0);
            // transform.rotation = Quaternion.RotateTowards(transform.rotation, angle, Time.deltaTime * 10f);


            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDRotation, angle);
            }
        }

        float distToPlayer()
        {
            GameObject player = GameObject.Find("PlayerArmature");
            target = player.transform;

            Vector3 toPlayer = target.position - transform.position;
            float dist = toPlayer.magnitude;
            return dist;
        }

        Vector3 GetRotationVector(GameObject start, GameObject target)
        {
            // Vector3 heading = start.transform.InverseTransformPoint(target.transform.position);
            Vector3 heading = target.transform.position - start.transform.position;
            heading = heading.normalized;
            heading.y = 0;

            return heading;
        }

        void OnDisable()
        {
            /*
             * If you uncomment this, you need to somehow tell the PlayerController to update
             * the enemies array by calling GameObject.FindGameObjectsWithTag("Enemy").
             * Otherwise the reference to a desgtroyed GameObject will still be in the enemies
             * array and you will get null pointer exceptions if you try to access it
             */

            //Destroy(gameObject);
        }
    }
}

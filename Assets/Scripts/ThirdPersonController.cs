using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Header("Attack Stats")]
        public float attackDamage = 3f;
        public float attackDistance = 2.0f;
        public float attackRadius = 90f;

        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Tooltip("Controllable")]
        public bool isControllable = true;
        public bool isAttackable = true;
        public bool isRollable = true;
        public bool isMoveable = true;
        public bool isDead = false;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;

        [Range(0, 1)]
        public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip(
            "Time required to pass before being able to jump again. Set to 0f to instantly jump again"
        )]
        public float JumpTimeout = 0.50f;

        [Tooltip(
            "Time required to pass before entering the fall state. Useful for walking down stairs"
        )]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip(
            "If the character is grounded or not. Not part of the CharacterController built in grounded check"
        )]
        public bool Grounded = true;

        [Header("Player Double Jump")]
        [Tooltip("If the character can double jump or not.")]
        public bool _canDoubleJump = false;

        [Tooltip("The height the player can jump")]
        public float doubleJumpHeight = 3f;

        [Tooltip(
            "Time required to pass before being able to jump again to doublejump. Set to 0f to instantly jump again"
        )]
        public float DoubleJumpTimeout = 0.25f;

        [Space(10)]
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = 0.0f;

        [Tooltip(
            "The radius of the grounded check. Should match the radius of the CharacterController"
        )]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip(
            "The follow target set in the Cinemachine Virtual Camera that the camera will follow"
        )]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip(
            "Additional degress to override the camera. Useful for fine tuning camera position when locked"
        )]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _doubleTimeoutDelta;
        private float _attackTimeoutDelta;
        private float[] _currentAttackDeltas = { 1.7f, 2.0f, 1.2f };
        private float _rollTimeoutDelta;
        private float _rollCooldownDelta;
        Vector3 rollDirection;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDDoubleJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDAttack;
        private int _animIDRanged;
        private int _animIDRoll;
        private int _animIDDead;

        //Attacking stuff
        private GameObject[] enemies;

        [SerializeField]
        private Image rollCooldown;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError(
                "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it"
            );
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Debug.Log(enemies.Length);
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();

            if (isControllable == true && _controller.enabled)
            {
                Move();
            }else{
                _animator.SetFloat(_animIDSpeed, 0.0f);
                _animator.SetFloat(_animIDMotionSpeed, 0.0f);
            }
            Attack();
            Roll();
            CheckIsControllable();
            CheckOutOfBounds();
        }

        private void CheckOutOfBounds(){
            if (transform.position.y <= -5){
                transform.position = new Vector3(0,0,-15);
                PlayerStatus status = gameObject.GetComponent<PlayerStatus>();
                status.ApplyDamage(15);
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDDoubleJump = Animator.StringToHash("DoubleJump");
            _animIDAttack = Animator.StringToHash("AttackNumber");
            _animIDRoll = Animator.StringToHash("Roll");
            _animIDDead = Animator.StringToHash("IsDead");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(
                transform.position.x,
                transform.position.y - GroundedOffset,
                transform.position.z
            );
            Grounded = Physics.CheckSphere(
                spherePosition,
                GroundedRadius,
                GroundLayers,
                QueryTriggerInteraction.Ignore
            );

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
                _animator.SetBool(_animIDFreeFall, false);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(
                _cinemachineTargetYaw,
                float.MinValue,
                float.MaxValue
            );
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(
                _cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw,
                0.0f
            );
        }

        private void CheckIsControllable()
        {
            if (IsAttackable && IsRollable && IsMoveable)
            {
                isControllable = true;
            }
            else
            {
                isControllable = false;
            }
        }

        public bool IsControllable
        {
            get { return isControllable; }
            set { isControllable = value; }
        }

        public bool IsMoveable
        {
            get { return isMoveable; }
            set { isMoveable = value; }
        }

        public bool IsAttackable
        {
            get { return isAttackable; }
            set { isAttackable = value; }
        }

        public bool IsRollable
        {
            get { return isRollable; }
            set { isRollable = value; }
        }

        public bool IsDead
        {
            get { return isDead; }
            set { isDead = value; 
                _animator.SetBool(_animIDDead, true);
                _controller.enabled = false;;
                }
        }

        private void Attack()
        {
            string cur_anim = _animator?.GetCurrentAnimatorClipInfo(0)[0].clip.name;
 
                //Z is 1 if moving forward, -1 otherwise
            if (Input.GetMouseButton(0) && Grounded && isControllable && (cur_anim == "Idle" || cur_anim == "Run with sword" || cur_anim == "Walk_N"))
            {
                if (_attackTimeoutDelta <= 0 && isAttackable)
                {
                    _attackTimeoutDelta = _currentAttackDeltas[0];
                    _animator.SetFloat(_animIDAttack, 1.0f);
                    isAttackable = false;

                    int closestEnemyInd = FindClosest();

                    if (closestEnemyInd >= 0)
                    {
                        AIStatus status = enemies[closestEnemyInd].GetComponent<AIStatus>();
                        status.ApplyDamage(attackDamage);
                    }
                }

                if (_attackTimeoutDelta >= 0.0f)
                {
                    _attackTimeoutDelta -= Time.deltaTime;
                }
            }
            else if (Input.GetKeyDown(KeyCode.E) && Grounded && isControllable && (cur_anim == "Idle" || cur_anim == "Run with sword" || cur_anim == "Walk_N"))
            {
                if (_attackTimeoutDelta <= 0 && isAttackable)
                {
                    _attackTimeoutDelta = _currentAttackDeltas[1];
                    _animator.SetFloat(_animIDAttack, 2.0f);
                    isAttackable = false;
                }

                if (_attackTimeoutDelta >= 0.0f)
                {
                    _attackTimeoutDelta -= Time.deltaTime;
                }
            }
            else if (Input.GetMouseButton(1) && Grounded && isControllable && (cur_anim == "Idle" || cur_anim == "Run with sword" || cur_anim == "Walk_N"))
            {
                if (_attackTimeoutDelta <= 0 && isAttackable)
                {
                    _attackTimeoutDelta = _currentAttackDeltas[2];
                    _animator.SetFloat(_animIDAttack, 3.0f);
                    isAttackable = false;
                    PlayerStatus status = gameObject.GetComponent<PlayerStatus>();
                    status.CurImmunityTime = 0.5f;
                    status.IsInvinceible = true;
                }

                if (_attackTimeoutDelta >= 0.0f)
                {
                    _attackTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                if (_attackTimeoutDelta >= 0.0f)
                {
                    _attackTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    isAttackable = true;
                }
                _animator.SetFloat(_animIDAttack, 0.0f);
            }
            
            
        }

        int FindClosest()
        {
            Transform target;
            float minDistance = 20000;
            int closest = -1;
            for (int i = 0; i < enemies.Length; i++)
            {
                AIStatus enemyStatus = enemies[i].GetComponent(typeof(AIStatus)) as AIStatus;
                if (!enemyStatus.isAlive())
                    continue;
                target = enemies[i].transform;
                Vector3 toEnemy = target.position - transform.position;

                float dist = toEnemy.magnitude;

                toEnemy.y = 0;
                toEnemy = Vector3.Normalize(toEnemy);

                //Forward in world space
                Vector3 forward = transform.TransformDirection(new Vector3(0, 0, 1));
                forward.y = 0;
                forward = Vector3.Normalize(forward);

                GameObject enemy = enemies[i];
                Vector3 relative = transform.InverseTransformPoint(enemy.transform.position);
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                float viewFov = attackRadius / 2;

                if (dist <= attackDistance && Mathf.Abs(angle) < viewFov)
                {
                    if (dist < minDistance)
                    {
                        minDistance = dist;
                        closest = i;
                    }
                }
            }

            return closest;
        }

        private void Move()
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero)
                targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(
                _controller.velocity.x,
                0.0f,
                _controller.velocity.z
            ).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (
                currentHorizontalSpeed < targetSpeed - speedOffset
                || currentHorizontalSpeed > targetSpeed + speedOffset
            )
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(
                    currentHorizontalSpeed,
                    targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate
                );

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(
                _animationBlend,
                targetSpeed,
                Time.deltaTime * SpeedChangeRate
            );
            if (_animationBlend < 0.01f)
                _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero && isControllable)
            {
                _targetRotation =
                    Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg
                    + _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    _targetRotation,
                    ref _rotationVelocity,
                    RotationSmoothTime
                );

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection =
                Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            
                    _controller.Move(
                        targetDirection.normalized * (_speed * Time.deltaTime)
                            + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime
                    );

            // update animator if using character
            if (_hasAnimator && isControllable)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
            else
            {
                _animator.SetFloat(_animIDSpeed, 0.0f);
                _animator.SetFloat(_animIDMotionSpeed, 0.0f);
            }
        }

        private void Roll()
        {
            string cur_anim = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

            if (Input.GetKeyDown(KeyCode.LeftControl) && Grounded && isRollable && isControllable && (cur_anim == "Idle" || cur_anim == "Run with sword" || cur_anim == "Walk_N") && _rollCooldownDelta <= 0)
            {
                Transform player = GameObject.Find("PlayerArmature").transform;
                _rollTimeoutDelta = 0.8f;
                _rollCooldownDelta = 2.5f;
                _animator.SetBool(_animIDRoll, true);
                isRollable = false;
                rollDirection = player.forward;
                if (_rollTimeoutDelta >= 0.0f)
                {
                    _rollTimeoutDelta -= Time.deltaTime;
                    _rollCooldownDelta -= Time.deltaTime;
                    rollCooldown.fillAmount = 1-(_rollCooldownDelta/2.5f);
                }
            }
            else
            {
                _animator.SetBool(_animIDRoll, false);
                if (_rollTimeoutDelta >= 0.0f)
                {
                    _rollTimeoutDelta -= Time.deltaTime;
                    Vector3 targetDirection =
                Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            
                    _controller.Move(
                        targetDirection.normalized * (7 * Time.deltaTime)
                            + new Vector3(0.0f, _verticalVelocity, 0.0f)
                    );
                    
                    transform.rotation = Quaternion.LookRotation(targetDirection);
                }
                else
                {
                    isRollable = true;
                    _rollCooldownDelta -= Time.deltaTime;
                    rollCooldown.fillAmount = 1-(_rollCooldownDelta/2.5f);
                }
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded && isControllable)
            {
                // _canDoubleJump = false;
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDDoubleJump, false);
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // _canDoubleJump = true;
                    _doubleTimeoutDelta = DoubleJumpTimeout;
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
                _canDoubleJump = true;
            }
            else if (_canDoubleJump)
            {
                // Double Jump
                if (_input.jump && _doubleTimeoutDelta <= 0.0f)
                {
                    _canDoubleJump = false;
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(doubleJumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, false);
                        _animator.SetBool(_animIDDoubleJump, true);
                    }
                }

                // jump timeout
                if (_doubleTimeoutDelta >= 0.0f)
                {
                    _doubleTimeoutDelta -= Time.deltaTime;
                    _input.jump = false;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;
                _doubleTimeoutDelta = DoubleJumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                        _animator.SetBool(_animIDDoubleJump, false);
                    }
                }

                // if we are not grounded, do not jump

                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f)
                lfAngle += 360f;
            if (lfAngle > 360f)
                lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded)
                Gizmos.color = transparentGreen;
            else
                Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(
                    transform.position.x,
                    transform.position.y - GroundedOffset,
                    transform.position.z
                ),
                GroundedRadius
            );
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(
                        FootstepAudioClips[index],
                        transform.TransformPoint(_controller.center),
                        FootstepAudioVolume
                    );
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(
                    LandingAudioClip,
                    transform.TransformPoint(_controller.center),
                    FootstepAudioVolume
                );
            }
        }
    }
}

 using UnityEngine;
 using System.Collections;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class PlayerController : ControllerHandler
    {

        private StarterAssetsInputs _input;
#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private GameObject _mainCamera;
        public SpriteRenderer _sprite;
        // player

        public bool _canAttack = true;

        public float checkAnimation;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }
        public override void Start()
        {
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            _combat = GetComponent<CombatHandler>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        public override void Update()
        {
            checkAnimation = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                Move();
            }
            AttackMove();
            GroundedCheck();
            Gravity();

            if (_input.move.x < 0){
                _sprite.flipX = false;
            }else if (_input.move.x > 0){
                _sprite.flipX = true;
            }

        }

        public void AttackMove()
        {
            if (_input.attack && _canAttack)
            {
                _input.attack = false;
                Aimming();
                _combat.ChangeAnimation("BaseAttack");
            }

        }
        private void RotateRelative(float smoothRotate)
        {
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, smoothRotate);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
        private void Move()
        {
            float targetSpeed = _combat._speedMove;

            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * 100);

                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * 100);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            if (_input.move != Vector2.zero)
            {
                RotateRelative(0.025f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }
        private void Aimming()
        {
            float rayDistance;

            Vector3 mousePos = Input.mousePosition;
            Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);
            Plane groundPlane = new Plane(Vector3.up, transform.position);

            if (groundPlane.Raycast(mouseRay, out rayDistance))
            {
                Vector3 lookAtPoint = mouseRay.GetPoint(rayDistance);

                if (Vector3.Distance(transform.position, lookAtPoint) <= 10)
                {
                    Quaternion rotation = Quaternion.LookRotation(lookAtPoint - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 2);
                }
            }
        }
    }
}
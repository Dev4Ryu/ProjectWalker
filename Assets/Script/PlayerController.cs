using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.EventSystems;

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
        // player

        public float checkAnimation;

        //zooming
        [Header("ZoomCamera")]
        [Tooltip("camera")]
        public float zoomMax = 20f;
        public float zoomMin = 10f;
        public float zoomSpeed = 12f;
        public float zoomFactor = 0.5f;
        public float zoom = 20f;
        public float cameraDistance = 20f;

        public CinemachinePositionComposer cinemachineVirtualCamera;

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
            if (cinemachineVirtualCamera == null)
            {
                cinemachineVirtualCamera = GameObject.FindGameObjectWithTag("CinemachineTarget").GetComponent<CinemachinePositionComposer>();
            }
        }

        public override void Update()
        {
            checkAnimation = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                Move();
            }
            Aimming();
            CameraZooming();
            GroundedCheck();
            Gravity();
        }
        private void CameraZooming()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");

            zoom -= scrollInput * zoomFactor;
            zoom = Mathf.Clamp(zoom, zoomMin, zoomMax);
            cameraDistance = Mathf.Lerp(cameraDistance,this.zoom,Time.deltaTime * this.zoomSpeed);

            cinemachineVirtualCamera.CameraDistance = cameraDistance;
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
            if (Input.GetKeyDown(KeyCode.E))
            {
                print("ok");
                Attack(0);
            }
        }
        public void Aimming()
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                Vector3 mousePos = Input.mousePosition;
                Ray mouseRay = Camera.main.ScreenPointToRay(mousePos);

                RaycastHit hit;
                Plane groundPlane = new Plane(Vector3.up, transform.position);


                if (Physics.Raycast(mouseRay, out hit) && _input.attack)
                {
                    TurnBaseManager.turnBaseData.charSelect = hit.collider.GetComponent<AIController>();
                }
            }
        }
        public void Attack(int _move)
        {
            AIController charSelect = TurnBaseManager.turnBaseData.charSelect;
            if (charSelect != null)
            {
                print(_combat.AbilityMove[_move].moveName);
                if (Vector3.Distance(transform.position, charSelect.transform.position) <= 10)
                {
                    Quaternion rotation = Quaternion.LookRotation(charSelect.transform.position - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 2);
                    _combat.ChangeAnimation(_combat.AbilityMove[_move].moveName);
                }
            }
        }
    }
}
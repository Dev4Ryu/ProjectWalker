 using UnityEngine;
 using System.Collections;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    public class ControllerHandler : MonoBehaviour
    {
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 200f)]
        public float RotationSmoothTime = 0.12f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;
        
        public float SpeedChangeRate = 10.0f;
        public float SpeedChangeStop = 20.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        protected float _speed;
        protected float _animationBlend;
        protected float _targetRotation = 0.0f;
        protected float _rotationVelocity;
        protected float _verticalVelocity;

        // animation IDs
        protected int _animIDSpeed;
        protected int _animIDMotionSpeed;

        public Animator _animator;
        public CharacterController _controller;

        protected CombatHandler _combat;

        private const float _threshold = 0.01f;

        protected bool _hasAnimator;

        public virtual void Start()
        {
            _combat = GetComponent<CombatHandler>();

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            AssignAnimationIDs();
        }

        public virtual void Update()
        {
            GroundedCheck();
            Gravity();
        }

        protected void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }
        protected void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
        }
        protected void Gravity()
        {
            if (Grounded)
            {
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }
            }
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f) && !Grounded)
            {
                transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
                _verticalVelocity = 0;
                Grounded = true;
            }
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
        public virtual void OnDisable()
        {
            _animator.SetFloat(_animIDSpeed, 0);
        }
    }
}
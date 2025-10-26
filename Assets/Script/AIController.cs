 using UnityEngine;
 using System.Collections;
 using UnityEngine.AI;

namespace StarterAssets
{
    public class AIController : ControllerHandler
    {
        [Header("AI")]
        [Tooltip("Target destination for Nav Mesh Agent as Transform")]
        public Transform Target;
        [Tooltip("Distance of target and enemy.")]
        public float PlayerRange;
        [Tooltip("Move of enemey")]

        private NavMeshAgent _thisAgent;
        private bool _canAttack = true;

        public override void Start()
        {
            _thisAgent = GetComponent<NavMeshAgent>();
            _combat = GetComponent<CombatHandler>();
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();

            AssignAnimationIDs();
        }
        void OnDisable()
        {
            _thisAgent.SetDestination(transform.position);
        }
        public override void Update()
        {
            if(_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && Target != null){
                PlayerRange = Vector3.Distance(transform.position, Target.position);
                _thisAgent.SetDestination(Target.position);

                if (_thisAgent.remainingDistance > _thisAgent.stoppingDistance)
                    MoveAI(_thisAgent.desiredVelocity.normalized, _thisAgent.desiredVelocity.magnitude);
                else
                    MoveAI(_thisAgent.desiredVelocity.normalized, 0f);
                Attack();
            }else{
                _thisAgent.SetDestination(transform.position);
            }
            
            GroundedCheck();
            Gravity();
        }
        private void MoveAI(Vector3 AgentDestination, float AgentSpeed)
        {
            if (AgentSpeed > 0f)
            {
                float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

                float speedOffset = 0.1f;

                float SpeedChange = AgentSpeed == 0f ? SpeedChangeStop : SpeedChangeRate;

                if (currentHorizontalSpeed < AgentSpeed - speedOffset || currentHorizontalSpeed > AgentSpeed + speedOffset)
                {
                    _speed = Mathf.Lerp(currentHorizontalSpeed, AgentSpeed, Time.deltaTime * SpeedChangeRate);

                    _speed = Mathf.Round(_speed * 1000f) / 1000f;
                }
                else
                {
                    _speed = AgentSpeed;
                }
                _animationBlend = Mathf.Lerp(_animationBlend, AgentSpeed, Time.deltaTime * SpeedChangeRate);

                if (_speed != 0f)
                {
                    _targetRotation = Mathf.Atan2(AgentDestination.x, AgentDestination.z) * Mathf.Rad2Deg;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }

                Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

                _thisAgent.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

                float theMagnitude = 1f;
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, theMagnitude);

            }
            else
            {
                _animationBlend = Mathf.Lerp(_animationBlend, 0f, Time.deltaTime * SpeedChangeRate);
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, 1f);
            }

        }
        private IEnumerator EnemyAttack()
        {
             Quaternion rotation = Quaternion.LookRotation(Target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 2);
            _canAttack = false;
            _combat.ChangeAnimation(_combat.AbilityMove[0].moveName);
            yield return new WaitForSeconds(2);
            _canAttack = true;
        }
        private void Attack()
        {
            PlayerRange = Vector3.Distance(transform.position, Target.position);
            if (PlayerRange <= 2 && _canAttack)
            {
                StartCoroutine(EnemyAttack());
            }
        }
    }
}
using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Runtime.InteropServices;

namespace StarterAssets
{
    public class AIController : ControllerHandler
    {
        [Header("AI")]
        [Tooltip("Target destination for Nav Mesh Agent as Transform")]
        public Transform _target;
        private PlayerController _player;

        [Tooltip("Distance of target and enemy.")]
        public float PlayerRange;

        [Tooltip("How far the AI can roam randomly when idle.")]
        public float WanderRadius = 10f;

        [Tooltip("How long AI waits before picking a new random destination.")]
        public float WanderDelay = 3f;
        [Tooltip("Monster Perspective")]
        public float attackRange;
        public Vector2 visonRange;

        private NavMeshAgent _thisAgent;
        private bool _isWandering = false;

        public override void Start()
        {
            _thisAgent = GetComponent<NavMeshAgent>();
            _combat = GetComponent<CombatHandler>();
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();

            AssignAnimationIDs();

            this.transform.parent = null;
            _player = TurnBaseManager.turnBaseData.player;
        }

        public override void Update()
        {
            PlayerRange = Vector3.Distance(transform.position, _player.transform.position);
            if (PlayerRange < visonRange.x && _target == null)
            {
                _target = _player.transform;
                TurnBaseManager.turnBaseData.charQueue.Add(this);
                if (TurnBaseManager.turnBaseData.queue == 0)
                {
                    TurnBaseManager.turnBaseData.queue++;
                    TurnBaseManager.turnBaseData.savedOriginal = true;
                }
            }
            else if (PlayerRange > visonRange.y && _target != null)
            {
                _target = null;
                TurnBaseManager.turnBaseData.charQueue.Remove(this);
                TurnBaseManager.turnBaseData.queue++;
                TurnBaseManager.turnBaseData.savedOriginal = true;
            }
            if (_target != null)
            {
                HandleChaseAndAttack();
            }
            else if(!TurnBaseManager.turnBaseData._turnBaseMode)
            {
                HandleWander();
            }

            GroundedCheck();
            Gravity();
        }

        private void HandleChaseAndAttack()
        {
            PlayerRange = Vector3.Distance(transform.position, _target.position);
            _thisAgent.SetDestination(_target.position);

            if (_thisAgent.remainingDistance > _thisAgent.stoppingDistance && _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                MoveAI(_thisAgent.desiredVelocity.normalized, _thisAgent.desiredVelocity.magnitude);
            else
                MoveAI(_thisAgent.desiredVelocity.normalized, 0f);

            Attack();
        }

        private void HandleWander()
        {
            // Start wandering coroutine if not already wandering
            if (!_isWandering)
            {
                StartCoroutine(WanderRoutine());
            }

            if (_thisAgent.remainingDistance > _thisAgent.stoppingDistance)
                MoveAI(_thisAgent.desiredVelocity.normalized, _thisAgent.desiredVelocity.magnitude);
            else
                MoveAI(_thisAgent.desiredVelocity.normalized, 0f);
        }

        private IEnumerator WanderRoutine()
        {
            _isWandering = true;

            // Choose a random point on the NavMesh within WanderRadius
            Vector3 randomDirection = Random.insideUnitSphere * WanderRadius;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, WanderRadius, NavMesh.AllAreas))
            {
                _thisAgent.SetDestination(hit.position);
            }

            // Wait before wandering again
            yield return new WaitForSeconds(WanderDelay);

            _isWandering = false;
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
            Quaternion rotation = Quaternion.LookRotation(_target.transform.position - transform.position);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 2);
            _combat.ChangeAnimation(_combat.AbilityMove[0].moveName);
            yield return new WaitForSeconds(0.5f);
        }

        private void Attack()
        {
            if (PlayerRange < 2 && _animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                StartCoroutine(EnemyAttack());
            }
        }
        public override void OnDisable()
        {
            _thisAgent.enabled = false;
            _animator.SetFloat(_animIDSpeed, 0);
        }
        public void OnEnable()
        {
            if(_thisAgent != null)
            {
                _thisAgent.enabled = true;
            }
        }
    }
}

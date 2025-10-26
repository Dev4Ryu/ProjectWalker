using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public bool attack;
		public bool dash;

		[Header("Movement Settings")]
		public bool analogMovement;


#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}
		public void OnAttack(InputValue value)
		{
			AttackInput(value.isPressed);
		}
		public void OnDash(InputValue value)
		{
			DashInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}
		public void AttackInput(bool newAttackState)
		{
			attack = newAttackState;
		}
		public void DashInput(bool newDashState)
		{
			dash = newDashState;
		}
	}
	
}
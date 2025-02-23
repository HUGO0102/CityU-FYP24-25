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
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool weakAttack;
		//public bool Attack2;
		//public bool Attack3;
		//public bool Attack4;
		public bool isDodge;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnWeakAttack(InputValue value)
		{
			WeakAttackInput(value.isPressed);
		}

		/*public void OnAttack2(InputValue value)
		{
			Attack2Input(value.isPressed);
		}

		public void OnAttack3(InputValue value)
		{
			Attack3Input(value.isPressed);
		}

		public void OnAttack4(InputValue value)
		{
			Attack4Input(value.isPressed);
		}*/

		public void OnDodge(InputValue value)
		{
			DodgeInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		public void WeakAttackInput(bool newweakAttackState)
		{
			weakAttack = newweakAttackState;
		}

		/*public void Attack2Input(bool newAttack2State)
		{
			Attack2 = newAttack2State;
		}

		public void Attack3Input(bool newAttack3State)
		{
			Attack3 = newAttack3State;
		}

		public void Attack4Input(bool newAttack4State)
		{
			Attack4 = newAttack4State;
		}*/

		public void DodgeInput(bool newisDodgeState)
		{
			isDodge = newisDodgeState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Confined : CursorLockMode.None;
			//Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}
	
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
[RequireComponent(typeof(PlayerInput))]
#endif
public class InputManager : AbstractSingleton<InputManager>
{
    PlayerController _player => PlayerController.Instance;
    [Header("Character Input Values")]
    public Vector3 Move;
    public Vector2 Look;
	public UnityEvent Jump;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
	private PlayerInput _playerInput;
#endif

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

	private float DeltaMultiplier
    {
        get
        {
			return IsCurrentDeviceMouse ? 1 : Time.deltaTime / 4;
        }
    }

	protected override void Awake()
    {
		base.Awake();
		_playerInput = GetComponent<PlayerInput>();
    }

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    public void OnMove(InputValue value)
	{
		MoveInput(value.Get<Vector2>());
	}

	public void OnLook(InputValue value)
	{
		LookInput(value.Get<Vector2>());
	}
#endif

	public void MoveInput(Vector2 newMoveDirection)
	{
		Move = new Vector3(newMoveDirection.x,0,newMoveDirection.y);
	}

	public void LookInput(Vector2 newLookDirection)
	{
		Look = newLookDirection * DeltaMultiplier;
	}

    public void OnJump(InputValue value)
    {
		//Jump?.Invoke();
		if(_player == null) return;
		_player.OnPressJump();
    }
}

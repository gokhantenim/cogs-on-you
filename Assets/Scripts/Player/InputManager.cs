using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
[RequireComponent(typeof(PlayerInput))]
#endif
public class InputManager : MonoBehaviour
{
	public static InputManager Instance;
    [Header("Character Input Values")]
    public Vector3 move;
    public Vector2 look;

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

	void Awake()
    {
		Application.targetFrameRate = 60;
		Instance = this;
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
		move = new Vector3(newMoveDirection.x,0,newMoveDirection.y);
	}

	public void LookInput(Vector2 newLookDirection)
	{
		look = newLookDirection * DeltaMultiplier;
	}
}

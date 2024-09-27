using UnityEngine;
#if ENABLE_INPUT_SYSTEM
    using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(PlayerInput))]
public class InputController : MonoBehaviour
{
	[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool dash;
		public bool crouch;
		public bool shoot;
		public bool ads;
		
	[Header("Movement Settings")]
		public bool analogMovement;

	[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

	//[Header("Input and Actions")]
	//InputAction jumpAction;

	void Start()
	{
		//playerInput = GetComponent<PlayerInput>();
		//jumpAction = playerInput.actions.FindAction("Jump");
		//jumpAction.started += OnStartJump;
		//jumpAction.canceled += OnCancelJump;
	}

    #if ENABLE_INPUT_SYSTEM
	    public void OnMove(InputValue value)
	    {
	    	move = value.Get<Vector2>();
	    }
	    public void OnLook(InputValue value)
	    {
	    	if(cursorInputForLook)
	    	{
	    		look = value.Get<Vector2>();
	    	}
	    }
	    public void OnJump(InputValue value)
	    {
			jump = value.isPressed;
	    }
	    public void OnDash(InputValue value)
	    {
	    	dash = value.isPressed;
	    }
	    public void OnCrouch(InputValue value)
	    {
	    	crouch = value.isPressed;
	    }
	    public void OnShoot(InputValue value)
	    {
	    	shoot = value.isPressed;
	    }
	    public void OnADS(InputValue value)
	    {
	    	ads = value.isPressed;
	    }
    #endif

	private void OnApplicationFocus(bool hasFocus)
	{
		SetCursorState(cursorLocked);
	}
	private void SetCursorState(bool newState)
	{
		Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
	}
}
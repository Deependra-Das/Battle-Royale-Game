using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInputManager : MonoBehaviour
{
    public Vector2 moveInputVector = Vector2.zero;
    public Vector2 lookInputVector = Vector2.zero;
    public bool jumpInput;

    public bool analogMovement;
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    public void OnMove(InputValue value)
    {
        moveInputVector = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            lookInputVector = value.Get<Vector2>();
        }
    }

    public void OnJump(InputValue value)
    {
        jumpInput = value.isPressed;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();
        networkInputData.lookInput = lookInputVector;
        networkInputData.movementInput = moveInputVector;
        networkInputData.isJumpPressed = jumpInput;

        if(jumpInput)
        {
            jumpInput = false;
        }

        return networkInputData;
    }
}

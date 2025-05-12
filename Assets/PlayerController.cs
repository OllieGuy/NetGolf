using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    public Vector2 moveInput;
    public Vector2 lookInput;
    public bool sprintInput;
    public bool jumpInput;

#if ENABLE_INPUT_SYSTEM
    void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    void OnLook(InputValue value)
    {
        LookInput(value.Get<Vector2>());
    }

    void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }
#endif

    public void MoveInput(Vector2 moveDirection)
    {
        moveInput = moveDirection;
    }

    public void LookInput(Vector2 lookDirection)
    {
        lookInput = lookDirection;
    }

    public void SprintInput(bool sprintState)
    {
        sprintInput = sprintState;
    }

    public void JumpInput(bool jumpState)
    {
        jumpInput = jumpState;
    }

    private void OnApplicationFocus(bool focus)
    {
        Cursor.visible = focus;
        Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
    }

}

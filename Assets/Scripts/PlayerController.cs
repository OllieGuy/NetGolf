using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : MonoBehaviour
{
    public Vector2 moveInput;
    public Vector2 lookInput;
    public bool sprintInput;
    public bool jumpInput;
    public bool interactInput;
    public bool attackInput;

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
    
    void OnInteract(InputValue value)
    {
        InteractInput(value.isPressed);
    }
    
    void OnAttack(InputValue value)
    {
        AttackInput(value.isPressed);
    }

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
    
    public void InteractInput(bool interactState)
    {
        interactInput = interactState;
    }

    public void AttackInput(bool attackState)
    {
        attackInput = attackState;
    }

    private void OnApplicationFocus(bool focus)
    {
        Cursor.visible = focus;
        Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

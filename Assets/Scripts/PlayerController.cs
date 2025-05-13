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
        moveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        sprintInput = value.isPressed;
    }

    void OnJump(InputValue value)
    {
        jumpInput = value.isPressed;
    }
    
    void OnInteract(InputValue value)
    {
        interactInput = value.isPressed;
    }
    
    void OnAttack(InputValue value)
    {
        attackInput = value.isPressed;
    }

    private void OnApplicationFocus(bool focus)
    {
        Cursor.visible = focus;
        Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

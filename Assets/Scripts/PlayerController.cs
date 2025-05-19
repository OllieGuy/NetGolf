using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PlayerController : MonoBehaviour
{
    public Vector2 moveInput;
    public float rotateInput;
    public Vector2 lookInput;
    public bool sprintInput;
    public bool jumpInput;
    public bool crouchInput;
    public bool interactInput;
    public bool attackInput;
    public bool attack2Input;
    public bool scorecardInput;

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    void OnRotate(InputValue value)
    {
        rotateInput = value.Get<float>();
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
    
    void OnCrouch(InputValue value)
    {
        crouchInput = value.isPressed;
    }
    
    void OnInteract(InputValue value)
    {
        interactInput = value.isPressed;
    }
    
    void OnAttack(InputValue value)
    {
        attackInput = value.isPressed;
    }
    
    void OnAttack2(InputValue value) // BUTTON - REMEMBER TO RESET
    {
        attack2Input = value.isPressed;
    }
    
    void OnScorecard(InputValue value) // BUTTON - REMEMBER TO RESET
    {
        scorecardInput = value.isPressed;
    }

    private void OnApplicationFocus(bool focus)
    {
        Cursor.visible = focus;
        Cursor.lockState = focus ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

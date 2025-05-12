using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    PlayerMovement pm;

    void Start()
    {
        if (pm == null) pm = GetComponent<PlayerMovement>();
    }

    void OnMove(InputValue value)
    {
        pm.moveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        pm.lookInput = value.Get<Vector2>();
    }

    void OnSprint(InputValue value)
    {
        pm.sprintInput = value.isPressed;
    }

    void OnJump(InputValue value)
    {
        pm.TryJump();
    }
}

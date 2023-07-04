using RBZG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class InputManager : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] MouseLook mouseLook;
    [SerializeField] Weapon weapon;

    PlayerControls controls;
    PlayerControls.MovementActions movement;
    PlayerControls.CameraLookActions cameraLook;
    PlayerControls.InteractionActions interaction;

    Vector2 horizontalInput;
    Vector2 cameraInput;

    bool jump;
    bool sprint;

    private void Awake()
    {
        controls = new PlayerControls();
        movement = controls.Movement;
        cameraLook = controls.CameraLook;
        interaction = controls.Interaction;
    }

    private void Update()
    {
        if (movement.Jump.triggered)
        {
            if (jump)
            {
                jump = false;
            }
            else
            {
                jump = true;
            }
        }

        if (movement.Run.triggered)
        {
            if (sprint)
            {
                sprint = false;
            }
            else
            {
                sprint = true;
            }
        }

        player.ReceicveMoveInput(horizontalInput);
        mouseLook.RecieveInput(cameraInput);

        player.ReceicveSprintInput(sprint);
        player.ReceicveJumpInput(jump);
    }

    private void OnEnable()
    {
        controls.Enable();

        movement.HorizontalMovement.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
        cameraLook.Look.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();

        movement.Crouch.performed += ctx => player.ReceicveCrouchInput(ctx);

        interaction.Aim.performed += ctx => weapon.Aim(ctx);
    }

    private void OnDisable()
    {
        controls.Disable();

        movement.HorizontalMovement.performed -= ctx => horizontalInput = ctx.ReadValue<Vector2>();
        cameraLook.Look.performed -= ctx => cameraInput = ctx.ReadValue<Vector2>();

        movement.Crouch.performed -= ctx => player.ReceicveCrouchInput(ctx);

        interaction.Aim.performed -= ctx => weapon.Aim(ctx);
    }
}

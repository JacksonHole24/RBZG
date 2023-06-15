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

    PlayerControls controls;
    PlayerControls.MovementActions movement;
    PlayerControls.CameraLookActions cameraLook;

    Vector2 horizontalInput;
    Vector2 cameraInput;

    float Fjump;
    float Fsprint;
    float Fslide;
    float Fcrouch;

    bool jump;
    bool sprint;
    bool slide;
    bool crouch;

    private void Awake()
    {
        controls = new PlayerControls();
        movement = controls.Movement;
        cameraLook = controls.CameraLook;

        movement.HorizontalMovement.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
        cameraLook.Look.performed += ctx => cameraInput = ctx.ReadValue<Vector2>();

        movement.Jump.performed += ctx => Fjump = ctx.ReadValue<float>();
        movement.Run.performed += ctx => Fsprint = ctx.ReadValue<float>();
        movement.Slide.performed += ctx => Fslide = ctx.ReadValue<float>();
        movement.Crouch.performed += ctx => Fcrouch = ctx.ReadValue<float>();
    }

    private void Update()
    {
        if(Fjump == 1)
        {
            jump = true;
        }
        else
        {
            jump = false;
        }

        if (Fslide == 1)
        {
            slide = true;
        }
        else
        {
            slide = false;
        }

        if (Fcrouch == 1)
        {
            crouch = true;
        }
        else
        {
            crouch = false;
        }

        if (Fsprint == 1)
        {
            sprint = true;
        }
        else
        {
            sprint = false;
        }

        player.ReceicveMoveInput(horizontalInput);
        mouseLook.RecieveInput(cameraInput);

        player.ReceicveCrouchInput(crouch);
        player.ReceicveSprintInput(sprint);
        player.ReceicveJumpInput(jump);
        player.ReceicveSlideInput(slide);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}

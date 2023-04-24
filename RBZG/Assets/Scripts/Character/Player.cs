using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RBZG
{
    public class Player : MonoBehaviour
    {
        [SerializeField] Transform weaponParent;

        private Vector3 weaponParentOrigin;
        private Vector3 targetWeaponBobPosition;
        private Vector3 weaponParentCurrentPosition;

        private float movementCounter;
        private float idleCounter;

        [HideInInspector] public bool isAiming;
        private Weapon weaponScript;

        [SerializeField] float speed;
        [SerializeField] float sprintModifier;
        [SerializeField] float crouchModifier;
        [SerializeField] float lengthOfSlide;
        [SerializeField] float slideModidier;
        [SerializeField] Camera normalCam;

        [SerializeField] Transform groundCheck;
        [SerializeField] float groundDistance;
        [SerializeField] LayerMask groundMask;

        private CharacterController controller;

        private float baseFOV;
        private float sprintFOVModifier = 1.1f;
        private Vector3 origin;

        private bool isGrounded;
        private bool isSprinting;
        private bool crouched;

        [SerializeField] float jumpHeight;

        [SerializeField] float gravity;
        Vector3 velocity;

        [SerializeField] float slideAmount;
        [SerializeField] float crouchAmount;
        [SerializeField] GameObject standingCollider;
        [SerializeField] GameObject crouchingCollider;

        private bool sliding;
        private float slide_time;
        private Vector3 slide_dir;

        private float t_vmove;
        private float t_hmove;
        private Vector3 t_direction;

        [SerializeField] Transform deathCamPos;
        [SerializeField] float deathCamTransitionTime;

        private HUDManager hudManager;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            weaponScript = GetComponent<Weapon>();
            hudManager = FindObjectOfType<HUDManager>();
        }

        private void Start()
        {
            baseFOV = normalCam.fieldOfView;
            origin = normalCam.transform.localPosition;

            weaponParentOrigin = weaponParent.localPosition;
            weaponParentCurrentPosition = weaponParentOrigin;
        }
        private void Update()
        {
            //Axis
            t_hmove = Input.GetAxis("Horizontal");
            t_vmove = Input.GetAxis("Vertical");


            //controls
            bool slide = Input.GetKeyDown(KeyCode.C);
            bool jump = Input.GetKeyDown(KeyCode.Space);
            bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            bool crouch = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);

            //states
            bool isJumping = jump;
            isSprinting = sprint && t_vmove > 0 && !isJumping && !isAiming;
            bool isSliding = isSprinting && slide && !sliding && isGrounded;
            bool isCrouching = crouch && !isSprinting && !isJumping && isGrounded;

            //Crouching
            if (isCrouching)
            {
                SetCrouch(!crouched);
            }

            //Jumping
            if (isJumping && isGrounded)
            {
                if (crouched)
                {
                    SetCrouch(false);
                }
                else if (sliding)
                {
                    sliding = false;
                    weaponParentCurrentPosition += Vector3.up * slideAmount;
                }
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            }


            //Sliding
            if (isSliding)
            {
                sliding = true;
                slide_dir = t_direction;
                slide_time = lengthOfSlide;
                //adjust the camera
                weaponParentCurrentPosition += Vector3.down * slideAmount;
                if (crouched) SetCrouch(true);
            }


            //UI Refreshes
            hudManager.UpdateAmmoCount(weaponScript.GetCurrentWeapon().GetClip(), weaponScript.GetCurrentWeapon().GetStash());
            RefreshCrosshair();
        }

        void FixedUpdate()
        {
            //Movement
            t_direction = Vector3.zero;
            float t_adjustedSpeed = speed;

            if (!sliding)
            {
                t_direction = new Vector3(t_hmove, 0, t_vmove);
                t_direction.Normalize();
                t_direction = transform.TransformDirection(t_direction);

                if (isSprinting)
                {
                    if (crouched) SetCrouch(false);
                    t_adjustedSpeed *= sprintModifier;
                }
                else if (crouched)
                {
                    t_adjustedSpeed *= crouchModifier;
                }
            }
            else
            {
                t_direction = slide_dir;
                t_adjustedSpeed *= slideModidier;
                slide_time -= Time.deltaTime;
                if (slide_time <= 0)
                {
                    sliding = false;
                    weaponParentCurrentPosition += Vector3.up * slideAmount;
                }
            }

            //Physics
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            controller.Move(t_direction * t_adjustedSpeed * Time.deltaTime);


            //Head Bob
            if (isAiming)
            {
                //Aiming
                HeadBob(idleCounter, 0.002f, 0.002f);
                idleCounter += Time.deltaTime;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f);
            }
            else if (t_hmove == 0 && t_vmove == 0)
            {
                //Idle
                HeadBob(idleCounter, 0.025f, 0.025f);
                idleCounter += Time.deltaTime;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 2f);
            }
            else if (!isSprinting && !crouched)
            {
                //Walking
                HeadBob(movementCounter, 0.035f, 0.035f);
                movementCounter += Time.deltaTime * 3f;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 6f);
            }
            else if (crouched)
            {
                //Crouching
                HeadBob(movementCounter, 0.02f, 0.02f);
                movementCounter += Time.deltaTime * 1.75f;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 6f);
            }
            else if (isSprinting)
            {
                //Sprinting
                HeadBob(movementCounter, 0.05f, 0.05f);
                movementCounter += Time.deltaTime * 5f;
                weaponParent.localPosition = Vector3.Lerp(weaponParent.localPosition, targetWeaponBobPosition, Time.deltaTime * 10f);
            }


            //Camera Stuff
            if (sliding)
            {
                normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * sprintFOVModifier * 1.25f, Time.deltaTime * 8f);
                normalCam.transform.localPosition = Vector3.Lerp(normalCam.transform.localPosition, origin + Vector3.down * slideAmount, Time.deltaTime * 6f);
            }
            else
            {
                if (isSprinting) { normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV * sprintFOVModifier, Time.deltaTime * 8f); }
                else { normalCam.fieldOfView = Mathf.Lerp(normalCam.fieldOfView, baseFOV, Time.deltaTime * 8f); }

                if (crouched) normalCam.transform.localPosition = Vector3.Lerp(normalCam.transform.localPosition, origin + Vector3.down * crouchAmount, Time.deltaTime * 6f);
                else normalCam.transform.localPosition = Vector3.Lerp(normalCam.transform.localPosition, origin, Time.deltaTime * 6f);
            }
        }

        void SetCrouch(bool p_state)
        {
            if (crouched == p_state) return;

            crouched = p_state;

            if (crouched)
            {
                standingCollider.SetActive(false);
                crouchingCollider.SetActive(true);
                weaponParentCurrentPosition += Vector3.down * crouchAmount;
            }

            else
            {
                standingCollider.SetActive(true);
                crouchingCollider.SetActive(false);
                weaponParentCurrentPosition -= Vector3.down * crouchAmount;
            }
        }

        void HeadBob(float p_z, float p_x_intensity, float p_y_intensity)
        {
            targetWeaponBobPosition = weaponParentCurrentPosition + new Vector3(Mathf.Cos(p_z) * p_x_intensity, Mathf.Sin(p_z * 2) * p_y_intensity, 0);
        }

        void RefreshCrosshair()
        {
            if (isAiming)
            {
                hudManager.UpdateCrosshair(false);
            }
            else
            {
                hudManager.UpdateCrosshair(true);
            }
        }
    }
}

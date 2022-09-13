using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBZG
{
    public class MouseLook : MonoBehaviour
    {
        public static bool cursorLocked;

        public Transform player;
        public Transform cams;
        public Transform weapon;

        public float ySensitivity;
        public float xSensitivity;
        public float maxAngle;

        private Quaternion camCenter;
        void Start()
        {
            cursorLocked = true;

            camCenter = cams.localRotation; //set rotation origin for cameras to camCenter
        }

        private void Update()
        {
            UpdateCursorLock();
        }

        private void FixedUpdate()
        {
            SetY();
            SetX();
        }

        void SetY()
        {
            float t_input = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
            Quaternion t_adj = Quaternion.AngleAxis(t_input, -Vector3.right);
            Quaternion t_delta = cams.localRotation * t_adj;

            if (Quaternion.Angle(camCenter, t_delta) < maxAngle)
            {
                cams.localRotation = t_delta;
                weapon.localRotation = t_delta;
            }
        }

        void SetX()
        {
            float t_input = -Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
            Quaternion t_adj = Quaternion.AngleAxis(t_input, -Vector3.up);
            Quaternion t_delta = player.localRotation * t_adj;
            player.localRotation = t_delta;
        }

        void UpdateCursorLock()
        {
            if (cursorLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBZG
{
    public class Sway : MonoBehaviour
    {
        public float intensity;
        public float smooth;

        private Quaternion originRotation;

        void Start()
        {
            originRotation = transform.localRotation;
        }

        void Update()
        {
            UpdateSway();
        }

        private void UpdateSway()
        {
            //controls
            float t_x_mouse = Input.GetAxis("Mouse X");
            float t_y_mouse = Input.GetAxis("Mouse Y");

            //calculate target rotation
            Quaternion t_x_adj = Quaternion.AngleAxis(-intensity * t_x_mouse, Vector3.up);
            Quaternion t_y_adj = Quaternion.AngleAxis(intensity * t_y_mouse, Vector3.right);
            Quaternion targetRotation = originRotation * t_x_adj * t_y_adj;

            //rotate towards target rotation
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * smooth);
        }
    }
}

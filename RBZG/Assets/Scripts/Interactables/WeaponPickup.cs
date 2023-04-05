using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBZG
{
    public class WeaponPickup : MonoBehaviour
    {
        public Gun weapon;
        public float cooldown;
        public GameObject gunDisplay;
        public List<GameObject> targets;
        public GameObject pickupText;

        private bool isDisabled;
        private float wait;
        private bool isColliding;
        private GameObject player;

        private void Start()
        {
            foreach (Transform t in gunDisplay.transform) Destroy(t.gameObject);

            GameObject newDisplay = Instantiate(weapon.prefab, gunDisplay.transform.position, gunDisplay.transform.rotation);
            newDisplay.transform.SetParent(gunDisplay.transform);
        }

        private void Update()
        {
            if (isColliding)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    Weapon weaponController = player.GetComponent<Weapon>();
                    weaponController.PickupWeapon(weapon.name);
                    Disable();
                }
            }

            if (isDisabled)
            {
                if (wait >= 0)
                {
                    wait -= Time.deltaTime;
                }
                else
                {
                    Enable();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                pickupText.SetActive(true);
                isColliding = true;
                player = other.gameObject;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Player")
            {
                isColliding = false;
                pickupText.SetActive(false);
            }
        }

        public void Disable()
        {
            wait = cooldown;
            isDisabled = true;
            pickupText.SetActive(false);

            foreach (GameObject a in targets)
            {
                a.SetActive(false);
            }
        }

        private void Enable()
        {
            wait = 0;
            isDisabled = false;

            foreach (GameObject a in targets)
            {
                a.SetActive(true);
            }
        }
    }
}

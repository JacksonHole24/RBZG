using System.Collections;
using System.Collections.Generic;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RBZG
{
    public class Weapon : MonoBehaviour
    {
        public List<Gun> loadout;
        public Transform weaponParent;
        public LayerMask canBeShot;

        private float currentCooldown;
        private int currentIndex;
        private GameObject currentWeapon;

        private bool isReloading;
        private bool isAiming;

        private Player playerScript;

        void Start()
        {
            playerScript = FindObjectOfType<Player>();
            foreach (Gun a in loadout) a.Initialize();
            Equip(0);
        }


        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) Equip(0);
            if (loadout.Count == 2)
            {
                if (Input.GetKeyDown(KeyCode.Alpha2)) Equip(1);
            }

            if (currentWeapon != null)
            {
                if (loadout[currentIndex].fireMode == 0)
                {
                    //Fire
                    if (Input.GetMouseButtonDown(0) && currentCooldown <= 0)
                    {
                        if (loadout[currentIndex].FireBullet())
                        {
                            Shoot();
                        }
                        else StartCoroutine(Reload(loadout[currentIndex].reloadTime));
                    }
                }
                else if (loadout[currentIndex].fireMode == 1)
                {
                    if (Input.GetMouseButton(0) && currentCooldown <= 0)
                    {
                        if (loadout[currentIndex].FireBullet())
                        {

                            Shoot();
                        }
                        else StartCoroutine(Reload(loadout[currentIndex].reloadTime));
                    }
                }
                else
                {
                    if (Input.GetMouseButton(0) && currentCooldown <= 0)
                    {
                        if (loadout[currentIndex].FireBullet())
                        {
                            Shoot();
                        }
                        else StartCoroutine(Reload(loadout[currentIndex].reloadTime));
                    }
                }

                //Reload
                if (Input.GetKeyDown(KeyCode.R))
                {
                    StartCoroutine(Reload(loadout[currentIndex].reloadTime));
                }

                //weapon position elasticity
                currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

                //cooldown
                if (currentCooldown > 0) currentCooldown -= Time.deltaTime;

            }
        }

        private void Equip(int p_ind)
        {
            if (currentWeapon != null)
            {
                if (isReloading) { StopCoroutine("Reload"); }
                Destroy(currentWeapon);
            }

            currentIndex = p_ind;

            GameObject t_newWeapon = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
            t_newWeapon.transform.localPosition = Vector3.zero;
            t_newWeapon.transform.localEulerAngles = Vector3.zero;

            currentWeapon = t_newWeapon;
        }


        public void PickupWeapon(string name)
        {
            Gun newWeapon = GunLibrary.FindGun(name);

            if (loadout.Count >= 2)
            {
                loadout[currentIndex] = newWeapon;
                Equip(currentIndex);
                loadout[currentIndex].Initialize();
            }
            else
            {
                loadout.Add(newWeapon);
                Equip(loadout.Count - 1);
                loadout[currentIndex].Initialize();
            }
        }

        public void Aim(InputAction.CallbackContext ctx)
        {
            if (ctx.performed)
            {
                if(currentWeapon != null)
                {
                    Debug.Log("here");
                    if (!isAiming)
                    {
                        Debug.Log("Aim");
                        StartCoroutine(MoveGunUp());
                        playerScript.isAiming = true;
                        isAiming = true;
                    }
                    else
                    {
                        Debug.Log("Not Aim");
                        StartCoroutine(MoveGunDown());
                        playerScript.isAiming = false;
                        isAiming = false;
                    }
                }
            }
        }

        private IEnumerator MoveGunUp()
        {
            Transform t_anchor = currentWeapon.transform.Find("Anchor");
            Transform t_state_ads = currentWeapon.transform.Find("States/ADS");

            while (t_anchor.position != t_state_ads.position)
            {
                t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_ads.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
                yield return null;
            }

            t_anchor.position = t_state_ads.position;
            yield return null;
        }

        private IEnumerator MoveGunDown()
        {
            Transform t_anchor = currentWeapon.transform.Find("Anchor");
            Transform t_state_hip = currentWeapon.transform.Find("States/Hip");

            while (t_anchor.position != t_state_hip.position)
            {
                t_anchor.position = Vector3.Lerp(t_anchor.position, t_state_hip.position, Time.deltaTime * loadout[currentIndex].aimSpeed);
                yield return null;
            }

            t_anchor.position = t_state_hip.position;
            yield return null;
        }

        private void Shoot()
        {
            Transform t_spawn = transform.Find("Cameras/Normal Camera");

            //cooldown
            currentCooldown = loadout[currentIndex].firerate;

            for (int i = 0; i < Mathf.Max(1, loadout[currentIndex].pellets); i++)
            {
                //bloom
                Vector3 t_bloom;

                if (playerScript.isAiming && loadout[currentIndex].pellets <= 1)
                {
                    t_bloom = t_spawn.position + t_spawn.forward * 1000f;
                    t_bloom -= t_spawn.position;
                }
                else if (playerScript.isAiming && loadout[currentIndex].pellets > 1)
                {
                    t_bloom = t_spawn.position + t_spawn.forward * 1000f;
                    t_bloom += Random.Range(-loadout[currentIndex].bloom * 0.5f, loadout[currentIndex].bloom * 0.5f) * t_spawn.up;
                    t_bloom += Random.Range(-loadout[currentIndex].bloom * 0.5f, loadout[currentIndex].bloom * 0.5f) * t_spawn.right;
                    t_bloom -= t_spawn.position;
                    t_bloom.Normalize();
                }
                else
                {
                    t_bloom = t_spawn.position + t_spawn.forward * 1000f;
                    t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.up;
                    t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.right;
                    t_bloom -= t_spawn.position;
                    t_bloom.Normalize();
                }

                //Raycast
                RaycastHit t_hit = new RaycastHit();
                if (Physics.Raycast(t_spawn.position, t_bloom, out t_hit, 1000f, canBeShot))
                {
                    if (t_hit.transform.gameObject.tag == "Enemy")
                    {
                        GameObject enemy = t_hit.transform.gameObject;
                        ZombieHealth enemyHealth = enemy.GetComponent<ZombieHealth>();

                        if (!enemyHealth.isDead)
                        {
                            enemyHealth.TakeDamage(loadout[currentIndex].damage);
                        }
                    }
                    else
                    {
                        GameObject t_newHole = Instantiate(loadout[currentIndex].bulletholePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
                        t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
                        Destroy(t_newHole, 5f);
                    }
                }
            }

            //gun fx
            if (playerScript.isAiming)
            {
                currentWeapon.transform.Rotate(-loadout[currentIndex].recoil * 0.1f, 0, 0);
                currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback * 0.1f;
            }
            else
            {
                currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
                currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;
            }

            if (loadout[currentIndex].recovery)
            {
                currentWeapon.GetComponent<Animator>().Play("Recovery", 0, 0);
            }

        }

        IEnumerator Reload(float p_wait)
        {
            isReloading = true;
            currentWeapon.SetActive(false);

            yield return new WaitForSeconds(p_wait);

            currentWeapon.SetActive(true);
            loadout[currentIndex].Reload();
            isReloading = false;
        }

        public Gun GetCurrentWeapon()
        {
            return loadout[currentIndex];
        }
    }
}


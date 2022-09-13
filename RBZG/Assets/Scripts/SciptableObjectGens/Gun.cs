using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RBZG
{
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class Gun : ScriptableObject
    {
        public string name;
        public int damage;
        public int reloadTime;
        public int ammo;
        public int fireMode; // 0 semi | 1 auto | 2+ burst
        public int pellets;
        public int clipSize;
        public float firerate; //How fast an automatic weapon can fire, how fast a semi auto can be pressed, how fast a burst can be pressed
        public float bloom;
        public float recoil;
        public float kickback;
        public float aimSpeed;
        public GameObject prefab;
        public bool recovery;

        public int stash; //current ammo
        public int clip; //current clip

        public void Initialize()
        {
            stash = ammo;
            clip = clipSize;
        }

        public bool FireBullet()
        {
            if (clip > 0)
            {
                clip -= 1;
                return true;
            }
            else return false;
        }

        public void Reload()
        {
            stash += clip;
            clip = Mathf.Min(clipSize, stash);
            stash -= clip;
        }

        public int GetStash() { return stash; }
        public int GetClip() { return clip; }
    }
}

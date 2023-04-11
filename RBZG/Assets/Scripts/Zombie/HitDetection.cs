using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    private ZombieAI zombieAI = null;

    void Start()
    {
        zombieAI = transform.parent.root.GetComponent<ZombieAI>();
    }


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            zombieAI.Hit(collision.gameObject);
        }
    }
}

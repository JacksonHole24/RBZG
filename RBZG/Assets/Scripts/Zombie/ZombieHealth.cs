using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] int health = 25;
    private Animator animator;
    public bool isDead = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            KillEnemy();
        }
        else
        {
            animator.SetTrigger("damaged");
        }
    }

    public void KillEnemy()
    {
        isDead = true;
        animator.SetTrigger("death");
        ZombieAI zom = GetComponent<ZombieAI>();
        zom.dead = true;
        zom.player.GetComponent<ZombieSpawner>().zombiesKilledThisRound++;
    }
}

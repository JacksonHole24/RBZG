using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RBZG
{
    public class EnemyAI : MonoBehaviour
    {
        private NavMeshAgent agent;

        private Transform player;

        public LayerMask whatIsPlayer;

        //stats
        public int health;
        public int damage;

        //attacking
        public float timeBetweenAttacks;
        public float attackRange;
        private bool alreadyAttacked;
        private bool playerInAttackRange;


        void Awake()
        {
            player = GameObject.Find("First Person Player").transform;
            agent = GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            if (playerInAttackRange)
            {
                AttackPlayer();
            }
            else
            {
                ChasePlayer();
            }
        }

        private void ChasePlayer()
        {
            agent.SetDestination(player.position);
        }

        private void AttackPlayer()
        {
            agent.SetDestination(transform.position);

            transform.LookAt(player);

            if (!alreadyAttacked)
            {
                Player playerScript = player.gameObject.GetComponent<Player>();
                playerScript.TakeDamage(damage);
                alreadyAttacked = true;
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }

        private void ResetAttack()
        {
            alreadyAttacked = false;
        }

        public void TakeDamage(int damage)
        {
            health -= damage;

            if (health <= 0)
            {
                Invoke(nameof(KillEnemy), 0.5f);
            }
        }

        public void KillEnemy()
        {
            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}

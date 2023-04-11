using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAI : MonoBehaviour
{
    [Tooltip("The layers that zombies should avoid.")]
    public LayerMask obstacleMask;
  
    [SerializeField] public Zombie zombieStats;

    [HideInInspector] public Transform player;

    private NavMeshAgent agent;                  // zombie's NavMeshAgent component
    private float nextUpdateTime;                // time until next update of player's position
    private float moveSpeed;                     // speed at which the zombie moves
    private bool isChasingPlayer = false;        // flag to indicate if zombie is chasing player
    private Coroutine LookCoroutine = null;
    private Animator animator;
    [HideInInspector] public bool dead = false;


    private void Awake()
    {
        // get player's transform
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // get zombie's NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        // set random move speed
        moveSpeed = Random.Range(zombieStats.minMoveSpeed, zombieStats.maxMoveSpeed);
        agent.speed = moveSpeed * Mathf.Clamp01(1f);

        // set random destination within spawn radius around player
        SetRandomDestination();

        // set initial update time
        nextUpdateTime = Time.time + Random.Range(zombieStats.minReactionTime, zombieStats.maxReactionTime);

        animator = GetComponentInChildren<Animator>();
        int ran = Random.Range(0, zombieStats.animators.Count);
        animator.runtimeAnimatorController = zombieStats.animators[ran];
    }

    private void Update()
    {
        if (dead) return;
        UpdateAnimator();
        startRotation();
    }

    void FixedUpdate()
    {
        if(dead) return;

        // check if it's time to update player's position
        if (Time.time >= nextUpdateTime)
        {
            // set new random destination within spawn radius around player
            SetRandomDestination();

            // set new update time
            nextUpdateTime = Time.time + Random.Range(zombieStats.minReactionTime, zombieStats.maxReactionTime);
        }

        // check if zombie is within stop chase distance of player
        if (Vector3.Distance(transform.position, player.position) <= zombieStats.stopChaseDistance)
        {
            // stop chasing random points and run directly to player
            agent.SetDestination(player.position);
            isChasingPlayer = true;

            // check if zombie is within attack range of player
            if (Vector3.Distance(transform.position, player.position) <= zombieStats.attackRange)
            {
                // call attack function
                Attack();
            }
        }
        else
        {
            isChasingPlayer = false;
        }
    }

    private void startRotation()
    {
        if (LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }
        LookCoroutine = StartCoroutine(LookAt());
    }

    private IEnumerator LookAt()
    {
        Quaternion lookRotation = Quaternion.LookRotation(player.transform.position - transform.position);
        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
            time += Time.deltaTime * LookSpeed();
            yield return null;
        }
    }

    private float LookSpeed()
    {
        return Random.Range(0.5f, 3f);
    }

    void OnCollisionEnter(Collision collision)
    {
        // avoid obstacles
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Vector3 avoidanceDirection = (transform.position - collision.transform.position).normalized;
            agent.Move(avoidanceDirection * (moveSpeed * Time.deltaTime));
        }
    }

    // set random destination within spawn radius around player
    void SetRandomDestination()
    {
        if (!isChasingPlayer)
        {
            Vector3 randomDirection = Random.insideUnitSphere * zombieStats.chasePlayerRadius;
            randomDirection += player.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, zombieStats.chasePlayerRadius, 1);
            agent.SetDestination(hit.position);
        }
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = agent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float forwardSpeed = localVelocity.z;
        float horizontalSpeed = localVelocity.x;
        animator.SetFloat("forwardMovement", forwardSpeed);
        animator.SetFloat("horizontalMovement", horizontalSpeed);
    }

    // empty attack function
    private void Attack()
    {
        animator.SetTrigger("attack");
    }

    public void Hit(GameObject playerHit)
    {
        PlayerHealth playerHealth = playerHit.GetComponent<PlayerHealth>();
        playerHealth.TakeDamage(zombieStats.damage);
    }
}

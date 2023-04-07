using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieAi1 : MonoBehaviour
{
    [Tooltip("The layers that zombies should avoid.")]
    public LayerMask obstacleMask;
    private Transform player;
    [Tooltip("The radius of the sphere around the player where zombies should spawn.")]
    public float chasePlayerRadius = 10.0f;
    [Tooltip("The minimum time in seconds before the zombie updates the player's position.")]
    public float minReactionTime = 1.0f;
    [Tooltip("The maximum time in seconds before the zombie updates the player's position.")]
    public float maxReactionTime = 3.0f;
    [Tooltip("The minimum speed at which the zombie moves.")]
    public float minMoveSpeed = 1.0f;
    [Tooltip("The maximum speed at which the zombie moves.")]
    public float maxMoveSpeed = 5.0f;
    [Tooltip("The distance at which the zombie will stop running to random points and run directly to the player.")]
    public float stopChaseDistance = 5.0f;

    private NavMeshAgent agent;                  // zombie's NavMeshAgent component
    private float nextUpdateTime;                // time until next update of player's position
    private float moveSpeed;                     // speed at which the zombie moves
    private bool isChasingPlayer = false;        // flag to indicate if zombie is chasing player
    private Coroutine LookCoroutine = null;

    void Start()
    {
        // get player's transform
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // get zombie's NavMeshAgent component
        agent = GetComponent<NavMeshAgent>();

        // set random move speed
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        agent.speed = moveSpeed * Mathf.Clamp01(1f);

        // set random destination within spawn radius around player
        SetRandomDestination();

        // set initial update time
        nextUpdateTime = Time.time + Random.Range(minReactionTime, maxReactionTime);
    }

    private void Update()
    {
        UpdateAnimator();
        startRotation();
    }

    void FixedUpdate()
    {
        // check if it's time to update player's position
        if (Time.time >= nextUpdateTime)
        {
            // set new random destination within spawn radius around player
            SetRandomDestination();

            // set new update time
            nextUpdateTime = Time.time + Random.Range(minReactionTime, maxReactionTime);
        }

        // check if zombie is within stop chase distance of player
        if (Vector3.Distance(transform.position, player.position) <= stopChaseDistance)
        {
            // stop chasing random points and run directly to player
            agent.SetDestination(player.position);
            isChasingPlayer = true;
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
            Vector3 randomDirection = Random.insideUnitSphere * chasePlayerRadius;
            randomDirection += player.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, chasePlayerRadius, 1);
            agent.SetDestination(hit.position);
        }
    }

    private void UpdateAnimator()
    {
        Vector3 velocity = agent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float forwardSpeed = localVelocity.z;
        float horizontalSpeed = localVelocity.x;
        Animator animator = GetComponentInChildren<Animator>();
        animator.SetFloat("forwardMovement", forwardSpeed);
        animator.SetFloat("horizontalMovement", horizontalSpeed);
        Debug.Log(forwardSpeed + "F");
        Debug.Log(horizontalSpeed + "H");
    }
}

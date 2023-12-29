using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RobotMG : MonoBehaviour
{
    public float health = 1.0f;
    public float detectionRadius = 5f;
    public float roamingRadius = 10f;
    public float shootingCooldown = 2f;
    public string playerTag = "Player";

    public ParticleSystem particleSystem;

    private enum RobotState
    {
        Roaming,
        Attacking
    }

    private RobotState currentState = RobotState.Roaming;
    private Transform playerTransform;
    private float nextShootTime;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (health <= 0.0f)
        {
            Debug.Log("Transition");
            enabled = false;
            return;
        }

        switch (currentState)
        {
            case RobotState.Roaming:
                Roam();
                DetectPlayer();
                break;

            case RobotState.Attacking:
                Attack();
                break;
        }
    }

    private void Roam()
    {
        if (!navMeshAgent.hasPath || navMeshAgent.remainingDistance < 0.1f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamingRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, roamingRadius, 1);
            Vector3 finalPosition = hit.position;
            navMeshAgent.SetDestination(finalPosition);
        }

        // Transition to Attack state if player is detected
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) <= detectionRadius)
        {
            currentState = RobotState.Attacking;
        }
    }

    private void Attack()
    {
        if (playerTransform != null)
        {
            transform.LookAt(playerTransform);
        }

        if (Time.time >= nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + shootingCooldown;
        }

        if (playerTransform == null || Vector3.Distance(transform.position, playerTransform.position) > detectionRadius)
        {
            currentState = RobotState.Roaming;
        }
    }

    private void DetectPlayer()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag(playerTag))
            {
                // Update the playerTransform when the player is detected
                playerTransform = collider.transform;
                currentState = RobotState.Attacking;
            }
        }
    }

    private void Shoot()
    {
        // Shooting logic here
        particleSystem.Play();
        // Example: Instantiate a bullet or raycast to hit the player
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, roamingRadius);
    }
}
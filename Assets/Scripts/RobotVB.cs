using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RobotVB : MonoBehaviour
{
    public float sensingRadius = 10f;
    public float explosionRadius = 2f;
    public float roamRadius = 5f;
    public float roamSpeed = 3f;
    public float chaseSpeed = 5f;
    public float jumpHeight = 1f;
    public float explosionDelay = 2f;
    public float damageAmount = 5f;
    public float health = 2.5f;
    public GameObject explosionParticle;
    public PlayerHealth playerHealth;

    private enum EnemyState
    {
        Roaming,
        Chasing,
        Exploding
    }

    private EnemyState currentState = EnemyState.Roaming;
    private Transform player;
    private NavMeshAgent navMeshAgent;
    private bool isJumping = false;
    private bool isExploded = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
        StartCoroutine(UpdateState());
        StartCoroutine(Jump());

        if (playerHealth == null)
        {
            playerHealth = GameObject.Find("PlayerCapsule").GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (health <= 0.0f)
        {
            Explode();
        }
    }

    IEnumerator UpdateState()
    {
        while (true)
        {
            switch (currentState)
            {
                case EnemyState.Roaming:
                    Roam();
                    break;
                case EnemyState.Chasing:
                    Chase();
                    break;
                case EnemyState.Exploding:
                    Explode();
                    break;
            }

            yield return null;
        }
    }

    void Roam()
    {
        if (Vector3.Distance(transform.position, player.position) < sensingRadius)
        {
            currentState = EnemyState.Chasing;
            return;
        }

        // Roaming behavior
        if (navMeshAgent.remainingDistance < 0.5f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1);
            Vector3 finalPosition = hit.position;

            navMeshAgent.SetDestination(finalPosition);
            navMeshAgent.speed = roamSpeed;
        }
    }

    void Chase()
    {
        // Chasing behavior
        navMeshAgent.SetDestination(player.position);
        navMeshAgent.speed = chaseSpeed;

        if (Vector3.Distance(transform.position, player.position) < explosionRadius)
        {
            currentState = EnemyState.Exploding;
        }
        else if (Vector3.Distance(transform.position, player.position) > sensingRadius)
        {
            currentState = EnemyState.Roaming;
        }
    }

    void Explode()
    {
        if (isExploded) return;
        GameObject particle = Instantiate(explosionParticle, transform.position,Quaternion.identity);
        Destroy(particle, 4f);
        Destroy(gameObject, .5f);
        playerHealth.DamagePlayer(damageAmount);
        isExploded = true;

        StartCoroutine(ResetStateAfterDelay());
        currentState = EnemyState.Roaming;
    }

    IEnumerator ResetStateAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        navMeshAgent.ResetPath();
    }

    IEnumerator Jump()
    {
        while (true)
        {
            if (!isJumping)
            {
                isJumping = true;
                float originalY = transform.position.y;
                float newY = originalY + jumpHeight;

                float t = 0f;
                while (t < 1f)
                {
                    t += Time.deltaTime * 2f;
                    transform.position = new Vector3(transform.position.x, Mathf.Lerp(originalY, newY, t), transform.position.z);
                    yield return null;
                }

                isJumping = false;
            }

            yield return null;
        }
    }

    void OnDrawGizmos()
    {
        // Draw sensing radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sensingRadius);
    }
}

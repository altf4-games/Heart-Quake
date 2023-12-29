using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RobotMG : MonoBehaviour
{
    public float health = 1.0f;
    public float detectionRadius = 5f;
    public float damageAmount = .5f;
    public float roamingRadius = 10f;
    public float shootingCooldown = 2f;
    private Transform playerTransform;
    public string playerTag = "Player";
    public float maxInaccuracyAngle = 5f;

    public ParticleSystem particleSys;
    public GameObject explosionParticle;
    public GameObject charmParticle;
    public Transform bulletSpawnPoint;
    public LineRenderer lineRenderer;
    public GameObject glass;
    public GameObject laser;


    private enum RobotState
    {
        Roaming,
        Attacking,
        Charmed,
    }

    private RobotState currentState = RobotState.Roaming;
    private float nextShootTime;
    private NavMeshAgent navMeshAgent;
    private bool isExploded = false;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        transform.GetChild(0).name = transform.name;
    }

    void Update()
    {
        if (isExploded) return;
        if (health <= 0.0f)
        {
            health = 1.0f;
            if (currentState == RobotState.Charmed)
            {
                Killed();
                return;
            }

            glass.SetActive(false);
            laser.SetActive(true);
            charmParticle.SetActive(true);
            playerTransform = null;
            EnemyManager.instance.activeRobots.Remove(this);
            currentState = RobotState.Charmed;
            return;
        }
        lineRenderer.SetPosition(0, bulletSpawnPoint.position);

        switch (currentState)
        {
            case RobotState.Roaming:
                Roam();
                DetectPlayer();
                break;

            case RobotState.Attacking:
                Attack();
                break;

            case RobotState.Charmed:
                Charmed();
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

    private void Killed()
    {
        GameObject particle = Instantiate(explosionParticle, transform.position, Quaternion.identity);
        Destroy(particle, 4f);
        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        playerTag = "";
        EnemyManager.instance.activeRobots.Remove(this);
        navMeshAgent.ResetPath();
        navMeshAgent.speed = 0f;
        isExploded = true;
        enabled = false;
    }

    private void Charmed()
    {
        if (EnemyManager.instance.activeRobots.Count == 0) return;

        string target = EnemyManager.instance.activeRobots[0].gameObject.name;
        playerTag = target;
        Roam();
        DetectPlayer();
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
            if (collider.name == playerTag)
            {
                playerTransform = collider.transform;
                currentState = RobotState.Attacking;
            }
        }
    }

    private void Shoot()
    {
        // Shooting logic here
        particleSys.Play();

        Quaternion bulletInaccuracy = Quaternion.Euler(Random.Range(-maxInaccuracyAngle, maxInaccuracyAngle),
                                                      Random.Range(-maxInaccuracyAngle, maxInaccuracyAngle),
                                                      0f);
        Vector3 bulletDirection = bulletInaccuracy * bulletSpawnPoint.forward;
        Ray ray = new Ray(bulletSpawnPoint.position, bulletDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(1, hit.point);

            //Debug.Log(hit.transform.name);

            if (hit.transform.name == playerTag)
            {
                if (hit.transform.parent.GetComponent<PlayerHealth>())
                {
                    hit.transform.parent.GetComponent<PlayerHealth>().DamagePlayer(damageAmount);
                }
                if (hit.transform.parent.GetComponent<RobotMG>())
                {
                    hit.transform.parent.GetComponent<RobotMG>().health -= damageAmount;
                }
            }
        }
        else
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(1, bulletSpawnPoint.position + bulletSpawnPoint.forward * 100f); // Change 100f to the desired line length
        }
        Invoke("DisableLineRenderer", 0.1f);
    }

    private void DisableLineRenderer()
    {
        particleSys.Stop();
        lineRenderer.enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, roamingRadius);
    }
}
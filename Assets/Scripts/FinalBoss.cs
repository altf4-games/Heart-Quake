using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalBoss : MonoBehaviour
{
    public Transform playerTransform;
    public float shootingCooldown = 2f;
    public float damageAmount = .5f;
    public float health = 100.0f;
    public GameObject explosionParticle;
    public ParticleSystem particleSys;
    public LineRenderer lineRenderer;
    public Transform bulletSpawnPoint;
    public AudioClip explodeClip;
    public AudioClip shootClip;
    public GameObject canvas;
    public GameObject eventSystem;

    public float maxInaccuracyAngle = 5f;
    private bool killed = false;

    private float nextShootTime;
    private string playerTag = "Player";

    void Update()
    {
        if (killed) return;
        if (health <= 0.0f)
        {
            Killed();
            return;
        }
        lineRenderer.SetPosition(0, bulletSpawnPoint.position);
        Attack();
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
    }

    private void Killed()
    {
        killed = true;
        GameObject particle = Instantiate(explosionParticle, transform.position, Quaternion.identity);
        Destroy(particle, 4f);
        if (explodeClip != null)
        {
            AudioManager.instance.PlayAudio(explodeClip, 1.0f, true, 100, transform.position);
        }
        for (int i = 0; i < 4; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        StartCoroutine(CompleteGame());
        //enabled = false;
    }

    private void Shoot()
    {
        // Shooting logic here
        particleSys.Play();
        if (shootClip != null)
        {
            AudioManager.instance.PlayAudio(shootClip, .2f, true, 100, transform.position);
        }

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

    private IEnumerator CompleteGame()
    {
        yield return new WaitForSeconds(1.5f);
        Instantiate(canvas);
        Instantiate(eventSystem);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(0);
    }
}
using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform cameraT;
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private int maxBullets = 7;
    [SerializeField] private float reloadTime = 1.0f;
    [SerializeField] private float fireRate = 0.3f;
    [SerializeField] private float damage = 0.5f;
    [SerializeField] private float maxRange = 300f;
    private int bullets;
    private bool isReloading = false;
    private bool canShoot = true;

    void Start()
    {
        bullets = maxBullets;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        lineRenderer.SetPosition(0, firePoint.position);
        if (isReloading || !canShoot || bullets <= 0) return;

        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Shoot());
        }

        if (Input.GetKeyDown(KeyCode.R) && bullets < maxBullets)
        {
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Shoot()
    {
        canShoot = false;
        weaponAnimator.SetTrigger("Fire");

        Ray ray = new Ray(cameraT.position, cameraT.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRange))
        {
            // TODO: Handle damage to the hit object (hit.transform)
            // Debug.Log("Hit: " + hit.transform.name);
        }

        bullets--;

        if (bullets <= 0)
        {
            StartCoroutine(Reload());
        }

        // Visualize the ray with a Line Renderer

        muzzleFlash.SetActive(true);
        Vector3 hitLoc = (hit.point == Vector3.zero) ? cameraT.forward * maxRange : hit.point;
        lineRenderer.enabled = true;

        lineRenderer.SetPosition(1, hitLoc);

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        StartCoroutine(DisableLineRenderer());

        yield return new WaitForSeconds(fireRate); // Recovery Time

        canShoot = true;
    }

    IEnumerator DisableLineRenderer()
    {
        yield return new WaitForSeconds(fireRate);
        lineRenderer.enabled = false;
        muzzleFlash.SetActive(false);
    }

    private IEnumerator Reload()
    {
        if (isReloading)
            yield break; // Exit if already reloading

        isReloading = true;
        weaponAnimator.SetTrigger("Reload");
        yield return new WaitForSeconds(reloadTime);
        bullets = maxBullets;
        isReloading = false;
    }
}
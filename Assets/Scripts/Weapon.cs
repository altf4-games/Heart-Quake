using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform cameraT;
    [SerializeField] private int bullets = 7;
    [SerializeField] private float reloadTime = 1.0f;
    [SerializeField] private float damage = 0.5f;
    [SerializeField] private float maxRange = 300f;
    private bool isReloading = false;

    void Start()
    {
        
    }

    void Update()
    {
        if (isReloading) return;
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = new Ray(cameraT.position, cameraT.forward);
            RaycastHit hit;
            bool rayCast = Physics.Raycast(ray,out hit, maxRange);
            Debug.DrawRay(cameraT.position, cameraT.forward * maxRange,Color.red,4f);
            if (rayCast)
            {
                Debug.Log(hit.transform.name);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [SerializeField] private float swaySpeed = 3f;
    private GameObject weapon;
    private Vector3 initialPos;

    void Start()
    {
        weapon = gameObject;
        initialPos = transform.localPosition;
    }

    void Update()
    {
        float movementX = -Input.GetAxis("Mouse X") * 0.1f;
        float movementY = -Input.GetAxis("Mouse Y") * 0.1f;

        Vector3 finalPos = new Vector3(movementX, movementY, 0);
        weapon.transform.localPosition = Vector3.Slerp(weapon.transform.localPosition, finalPos + initialPos, swaySpeed * Time.deltaTime);
    }
}

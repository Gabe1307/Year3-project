using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    // Weapon Sway
    [SerializeField] private float smooth;
    [SerializeField] private float multiplier;

    private void Update()
    {
        // Mouse Input 
        float mouseX = Input.GetAxisRaw("Mouse ") * multiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * multiplier;

        // Traget rotataion
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);


        Quaternion targetRotation = rotationX * rotationY;

        // rotation
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockTransform : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0, 30, 0); // Set the rotation speed (degrees per second) for each axis
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;
    }

    void LateUpdate()
    {
        transform.position = initialPosition;
        transform.rotation = initialRotation;
        
        // Apply a constant rotation over time
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}

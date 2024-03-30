using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class PlanetInteraction : MonoBehaviour
{
    public float rotationSpeed = 0.1f; // You can adjust this value to get the desired rotation speed
    public float tiltAngle = 23.5f; // You can adjust this value to get the desired axial tilt

    private void Start()
    {

    }

    private void Update()
    {
        Rotate();
    }

    private void Rotate()
    {
        // Axial tilt
        Quaternion tilt = Quaternion.Euler(tiltAngle, 0, 0);

        // Calculate rotation around the X-axis (planet's local right direction)
        Quaternion rotation = Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0);

        // Apply both axial tilt and rotation
        transform.rotation *= tilt * rotation;
    }
}
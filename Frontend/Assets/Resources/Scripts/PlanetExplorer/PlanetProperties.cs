using UnityEngine;

[CreateAssetMenu(fileName = "PlanetProperties", menuName = "ScriptableObjects/PlanetProperties", order = 1)]
public class PlanetProperties : ScriptableObject
{
    public float rotationSpeed = 10f;
    public Vector3 rotationAxis = Vector3.up;
    public float autoRotationSpeed = 5f;
}

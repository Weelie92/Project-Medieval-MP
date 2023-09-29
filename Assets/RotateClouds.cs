using UnityEngine;

public class RotateClouds : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1f; // Adjust the rotation speed as desired

    // Update is called once per frame
    void Update()
    {
        // Rotate the game object around the y-axis
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}

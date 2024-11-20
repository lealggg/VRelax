using UnityEngine;

public class UpnDown : MonoBehaviour
{
    [SerializeField] private float floatHeight = 1f; // How high the object floats
    [SerializeField] private float floatSpeed = 1f;  // Speed of the floating motion
    private Vector3 startPosition;

    void Start()
    {
        // Store the starting position of the GameObject
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new Y position using a sine wave for smooth motion
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        // Apply the calculated position back to the GameObject
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}

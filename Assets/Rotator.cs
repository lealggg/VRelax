using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 33f;
    [SerializeField] private float scaleSpeed = 0.66f; // Slower speed for smooth scaling

    private Vector3 minScale;
    private Vector3 maxScale;
    private bool scalingUp = true;

    void Start()
    {
        // Initialize scales based on the object's initial scale
        minScale = transform.localScale;
        maxScale = minScale * 1.8f;
    }

    void Update()
    {
        // Rotate the object around its local Y-axis
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Oscillate scale smoothly using Mathf.PingPong
        float pingPongValue = Mathf.PingPong(Time.time * scaleSpeed, 1f); // Time-based oscillation
        Vector3 currentScale = Vector3.Lerp(minScale, maxScale, pingPongValue); // Smooth transition between min and max scale

        transform.localScale = currentScale;
    }
}

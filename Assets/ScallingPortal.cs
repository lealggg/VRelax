using UnityEngine;

public class ScalingPortal : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private Transform pointA;  // Start point
    [SerializeField] private Transform pointB;  // End point
    [SerializeField] private float moveSpeed = 1f;  // Speed of movement

    [Header("Scaling Settings")]
    [SerializeField] private float scaleMultiplier = 2.4f;  // Factor to scale by when reaching point B

    private Vector3 startScale;  // Object's initial scale
    private float journeyLength;  // Distance between point A and B
    private float startTime;  // Time when movement starts
    private bool movingToB = true;  // Boolean to track direction of movement

    void Start()
    {
        // Record the object's initial scale
        startScale = transform.localScale;

        // Calculate the total journey distance
        journeyLength = Vector3.Distance(pointA.position, pointB.position);
        startTime = Time.time;
    }

    void Update()
    {
        // Calculate how far along the journey the object is
        float distanceCovered = (Time.time - startTime) * moveSpeed;
        float fractionOfJourney = Mathf.Clamp01(distanceCovered / journeyLength);  // Ensure it's between 0 and 1

        // Move the object smoothly between point A and point B
        if (movingToB)
        {
            transform.position = Vector3.Lerp(pointA.position, pointB.position, fractionOfJourney);
            transform.localScale = Vector3.Lerp(startScale, startScale * scaleMultiplier, fractionOfJourney);
        }
        else
        {
            transform.position = Vector3.Lerp(pointB.position, pointA.position, fractionOfJourney);
            transform.localScale = Vector3.Lerp(startScale * scaleMultiplier, startScale, fractionOfJourney);
        }

        // Once the object reaches point B or point A, reverse direction
        if (fractionOfJourney >= 1f)
        {
            movingToB = !movingToB;  // Reverse the direction
            startTime = Time.time;  // Reset the time to continue moving in the new direction
        }
    }
}

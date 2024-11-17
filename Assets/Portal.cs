using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Transform pointA;  // Start point
    [SerializeField] private Transform pointB;  // End point
    [SerializeField] private float moveSpeed = 2f;  // Speed of movement

    private float journeyLength;  // Distance between point A and B
    private float startTime;  // Time when movement starts

    void Start()
    {
        // Calculate the total journey distance
        journeyLength = Vector3.Distance(pointA.position, pointB.position);
        startTime = Time.time;  // Record the start time
    }

    void Update()
    {
        // Calculate how far along the journey the object is
        float distanceCovered = (Time.time - startTime) * moveSpeed;
        float fractionOfJourney = distanceCovered / journeyLength;

        // Smoothly move the object between point A and point B
        transform.position = Vector3.Lerp(pointA.position, pointB.position, fractionOfJourney);

        // Once the object reaches point B, reset to point A and start again
        if (fractionOfJourney >= 1)
        {
            startTime = Time.time;  // Reset the start time
            // Swap the points (making it go from B to A in the next iteration)
            Transform temp = pointA;
            pointA = pointB;
            pointB = temp;
        }
    }
}

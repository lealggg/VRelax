using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 33f;
    

   

    void Start()
    {
       
    }

    void Update()
    {
        // Rotate the object around its local Y-axis
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}

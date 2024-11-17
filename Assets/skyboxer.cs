using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    [SerializeField] private Material newSkybox; // Assign the new skybox material in the Inspector
    private float changeTime = 10f; // Time in seconds to wait before changing the skybox

    void Start()
    {
        // Start a coroutine to wait for the specified time and then change the skybox
        StartCoroutine(ChangeSkyboxAfterTime());
    }

    private System.Collections.IEnumerator ChangeSkyboxAfterTime()
    {
        // Wait for the specified time
        yield return new WaitForSeconds(changeTime);

        // Change the skybox
        if (newSkybox != null)
        {
            RenderSettings.skybox = newSkybox;
            DynamicGI.UpdateEnvironment(); // Update lighting to reflect the new skybox
        }
        else
        {
            Debug.LogWarning("No new skybox assigned in the Inspector!");
        }
    }
}

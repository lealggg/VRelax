using UnityEngine;

public class SkyboxChangerWithHeartRate : MonoBehaviour
{
    [SerializeField] private Material[] skyboxes; // Assign skybox materials in the Inspector (index 0 = calm, 1 = normal, 2 = stressed)
    [SerializeField] private float transitionSpeed = 2f; // Speed for smooth transition
    private Material currentSkybox; // The active skybox being displayed
    private float simulatedHeartRate; // Variable to simulate heart rate input
    private string currentState; // Track the current condition (calm, normal, stressed)
   // public TextMeshProUGUI moodLabel;

    void Start()
    {   
        

        if (skyboxes.Length < 3)
        {
            Debug.LogError("Please assign at least 3 skyboxes for calm, normal, and stressed states.");
            return;
        }

        // Initialize to the calm skybox and starting heart rate
       currentSkybox = skyboxes[HandleOption(GameData.userMood)];
        RenderSettings.skybox = currentSkybox;
        DynamicGI.UpdateEnvironment();

        simulatedHeartRate = Random.Range(60f, 80f); // Start with a realistic normal heart rate
        currentState = "normal";

        // Start simulating heart rate and evaluating conditions
        InvokeRepeating(nameof(SimulateHeartRate), 1f, 0.5f); // Gradually adjust heart rate every 0.5 seconds
        InvokeRepeating(nameof(CheckHeartRateAndChangeSkybox), 1f, 2f); // Evaluate heart rate condition every 2 seconds
    }

    void SimulateHeartRate()
    {
        // Gradually adjust the heart rate
        float heartRateChange = Random.Range(-10f, 10f); // Small changes per frame
        simulatedHeartRate = Mathf.Clamp(simulatedHeartRate + heartRateChange, 50f, 130f); // Keep heart rate within realistic bounds

        Debug.Log($"Simulated Heart Rate: {simulatedHeartRate}");
    }

    void CheckHeartRateAndChangeSkybox()
    {
        string newState = DetermineState(simulatedHeartRate);

        if (newState != currentState) // Change skybox only if the state changes
        {
            currentState = newState;
            int skyboxIndex = GetSkyboxIndexForState(newState);
            if (skyboxIndex >= 0 && skyboxIndex < skyboxes.Length)
            {
                StartCoroutine(SmoothTransitionToSkybox(skyboxes[skyboxIndex]));
            }
        }
    }

    string DetermineState(float heartRate)
    {
        if (heartRate > 90) return "stressed"; // Stressed condition
        if (heartRate >= 60 && heartRate <= 90) return "normal"; // Normal condition
        return "calm"; // Calm condition
    }

    int GetSkyboxIndexForState(string state)
    {
        return state switch
        {
            "calm" => 0,
            "normal" => 1,
            "stressed" => 2,
            _ => -1
        };
    }

    System.Collections.IEnumerator SmoothTransitionToSkybox(Material targetSkybox)
    {
        Material startingSkybox = RenderSettings.skybox;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;

            // Smoothly interpolate between the starting and target skybox materials
            RenderSettings.skybox.Lerp(startingSkybox, targetSkybox, t);
            DynamicGI.UpdateEnvironment();
            yield return null;
        }

        RenderSettings.skybox = targetSkybox; // Ensure the target skybox is fully applied
        DynamicGI.UpdateEnvironment();
    }

     int HandleOption(int selectedOption)
    {
        switch (selectedOption)
        {
            case 0:
                Debug.Log("Has seleccionado la opción 1: Iniciar juego.");
                Debug.Log($"Skybox set to Calm: {skyboxes[0].name}");
                return 0;
                

            case 1:
                Debug.Log("Has seleccionado la opción 2: Cargar partida.");
                return 1;
               

            case 2:
                Debug.Log("Has seleccionado la opción 3: Salir del juego.");
                return 2;
                

            default:
                Debug.Log("Opción no válida. Por favor, selecciona entre 1, 2 o 3.");
                return 0;
                
        }
    }
}


using UnityEngine;

public class skc3 : MonoBehaviour
{
    [SerializeField] private Material[] skyboxes; // Assign skybox materials in the Inspector (index 0 = calm, 1 = normal, 2 = stressed)
    [SerializeField] private float transitionSpeed = 2f; // Speed for smooth transition
    private Material currentSkybox; // The active skybox being displayed
    private float simulatedHeartRate; // Variable to simulate heart rate input
    private string currentState; // Track the current condition (calm, normal, stressed)

    private AudioSource audioSource; // Audio source for microphone input
    private float[] audioData; // Array to hold microphone data
    private int sampleRate = 44100; // Standard audio sample rate
    private float breathFrequency; // Estimated breathing frequency (in Hz)
    private float heartRate; // Calculated heart rate based on breathing frequency

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

        currentState = "normal";

        // Start microphone recording
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;

        // Ensure the microphone is available and start recording
        if (Microphone.devices.Length > 0)
        {
            audioSource.clip = Microphone.Start(Microphone.devices[0], true, 1, sampleRate);
            while (!(Microphone.GetPosition(Microphone.devices[0]) > 0)) { } // Wait for the microphone to start
            audioSource.Play();
        }
        else
        {
            Debug.LogError("No microphone found.");
        }

        // Initialize audio data array
        audioData = new float[sampleRate];

        // Start evaluating the heart rate based on microphone data
        InvokeRepeating(nameof(ProcessBreathingData), 1f, 1f); // Process every second
        InvokeRepeating(nameof(CheckHeartRateAndChangeSkybox), 1f, 2f); // Evaluate heart rate condition every 2 seconds
    }

    void ProcessBreathingData()
    {
        // Get microphone data into the audioData array
        audioSource.GetOutputData(audioData, 0);

        // Calculate the frequency of the breathing pattern using a Fourier Transform
        float frequency = CalculateFrequency(audioData);

        // Estimate the breathing rate in Hz (breaths per second)
        breathFrequency = frequency;

        // Convert breathing frequency to heart rate (assuming 1 breath = 1/3 heartbeats)
        heartRate = breathFrequency * 60f * 3; // Simple estimation of heart rate (in bpm)

        // Log the heart rate estimate for debugging
        Debug.Log($"Estimated Heart Rate: {heartRate} bpm");
    }

    float CalculateFrequency(float[] data)
    {
        // Perform a simple frequency analysis (Fourier Transform or other methods)
        // Here we just calculate the dominant frequency by looking at the peak values in the audio data
        float maxAmplitude = 0;
        int maxIndex = 0;

        for (int i = 0; i < data.Length / 2; i++) // Only look at the first half of the spectrum (real frequencies)
        {
            if (Mathf.Abs(data[i]) > maxAmplitude)
            {
                maxAmplitude = Mathf.Abs(data[i]);
                maxIndex = i;
            }
        }

        // Convert index to frequency (this is an approximation, you can use a more accurate FFT if needed)
        float frequency = maxIndex * sampleRate / data.Length;

        return frequency;
    }

    void CheckHeartRateAndChangeSkybox()
    {
        string newState = DetermineState(heartRate);

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

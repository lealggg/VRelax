using UnityEngine;

public class SKC2 : MonoBehaviour
{
    [SerializeField] private Material[] skyboxes; // Assign skybox materials (index 0 = calm, 1 = normal, 2 = stressed)
    [SerializeField] private float transitionSpeed = 2f; // Speed for smooth skybox transitions
    private Material currentSkybox; // The current active skybox
    private float simulatedHeartRate; // Variable to store the calculated heart rate
    private string currentState; // Tracks the current state (calm, normal, stressed)
    public GameObject[] objetosCalma;
    public GameObject[] objetosNormal;
    public GameObject[] objetosEstres;

    private AudioClip microphoneInput; // AudioClip for microphone input
    private const int SampleRate = 44100; // Sample rate for microphone input
    private const int SampleSize = 2048; // Size of the audio buffer for processing frequencies

    void Start()
    {
        // Validate that all required skyboxes are assigned
        if (skyboxes.Length < 3)
        {
            Debug.LogError("Please assign at least 3 skyboxes for calm, normal, and stressed states.");
            return;
        }

        // Initialize to the starting state based on user mood
        int moodIndex = HandleOption(GameData.userMood);
        currentSkybox = skyboxes[moodIndex];
        RenderSettings.skybox = currentSkybox;
        DynamicGI.UpdateEnvironment();

        // Set the initial state
        currentState = DetermineStateBasedOnMood(moodIndex);
        UpdateObjectsState(currentState);

        // Start capturing microphone input
        InitializeMicrophone();

        // Periodically check heart rate and change state
        InvokeRepeating(nameof(CheckHeartRateAndChangeSkybox), 1f, 2f);
    }

    void InitializeMicrophone()
    {
        // Start recording from the default microphone
        if (Microphone.devices.Length > 0)
        {
            microphoneInput = Microphone.Start(null, true, 1, SampleRate);
            Debug.Log("Microphone initialized.");
        }
        else
        {
            Debug.LogError("No microphone detected. Ensure your device has an active microphone.");
        }
    }

    float CalculateHeartRateFromMicrophone()
{
    // Retrieve microphone data
    float[] audioData = new float[SampleSize];
    microphoneInput.GetData(audioData, 0);

    // Variables to store frequency analysis results
    float maxFrequency = 0f;
    float maxAmplitude = 0f;

    // Analyze the microphone data for the strongest frequency
    for (int i = 0; i < audioData.Length; i++)
    {
        float amplitude = Mathf.Abs(audioData[i]); // Use absolute value to handle negative samples

        if (amplitude > maxAmplitude)
        {
            maxAmplitude = amplitude;
            maxFrequency = i * (SampleRate / 2f) / SampleSize; // Convert index to frequency
        }
    }

    // Debugging: Log the calculated max frequency and amplitude
    Debug.Log($"Max Frequency: {maxFrequency} Hz, Max Amplitude: {maxAmplitude}");

    // Adjusted expected range based on realistic microphone input
    if (maxFrequency < 0.5f || maxFrequency > 10f) // Expanded range
    {
        Debug.LogWarning("Frequency out of realistic range for heart rate calculation.");
        return 60f; // Default to resting heart rate
    }

    // Convert frequency to heart rate and clamp to realistic range
    float heartRate = Mathf.Clamp(maxFrequency * 60f / 2f, 20f, 110f);

    // Debugging: Log the heart rate
    Debug.Log($"Calculated Heart Rate: {heartRate}");

    return heartRate;
}


    void CheckHeartRateAndChangeSkybox()
    {
        // Calculate heart rate using microphone input
        simulatedHeartRate = CalculateHeartRateFromMicrophone();
        Debug.Log($"Calculated Heart Rate: {simulatedHeartRate}");

        // Determine the new state
        string newState = DetermineState(simulatedHeartRate);

        // Change state and skybox if the state has changed
        if (newState != currentState)
        {
            Debug.Log($"State changed from {currentState} to {newState}");
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
        // Determine the state based on heart rate ranges
        if (heartRate > 90)
        {
            UpdateObjectsState("stressed");
            return "stressed"; // Stressed condition
        }
        else if (heartRate > 60)
        {
            UpdateObjectsState("normal");
            return "normal"; // Normal condition
        }
        else
        {
            UpdateObjectsState("calm");
            return "calm"; // Calm condition
        }
    }

    string DetermineStateBasedOnMood(int moodIndex)
    {
        // Map mood index to state
        return moodIndex switch
        {
            0 => "calm",
            1 => "normal",
            2 => "stressed",
            _ => "calm"
        };
    }

    int GetSkyboxIndexForState(string state)
    {
        // Map state to skybox index
        return state switch
        {
            "calm" => 0,
            "normal" => 1,
            "stressed" => 2,
            _ => 1
        };
    }

    System.Collections.IEnumerator SmoothTransitionToSkybox(Material targetSkybox)
    {
        // Smoothly transition to the target skybox
        Material startingSkybox = RenderSettings.skybox;
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * transitionSpeed;
            RenderSettings.skybox.Lerp(startingSkybox, targetSkybox, t);
            DynamicGI.UpdateEnvironment();
            yield return null;
        }

        RenderSettings.skybox = targetSkybox;
        DynamicGI.UpdateEnvironment();
    }

    void UpdateObjectsState(string state)
    {
        // Activate objects based on the current state
        bool isCalm = state == "calm";
        bool isNormal = state == "normal";
        bool isStressed = state == "stressed";

        foreach (var obj in objetosCalma) obj.SetActive(isCalm);
        foreach (var obj in objetosNormal) obj.SetActive(isNormal);
        foreach (var obj in objetosEstres) obj.SetActive(isStressed);
    }

    int HandleOption(int selectedOption)
    {
        // Handle initial user mood and return corresponding index
        switch (selectedOption)
        {
            case 0: UpdateObjectsState("calm"); return 0;
            case 1: UpdateObjectsState("normal"); return 1;
            case 2: UpdateObjectsState("stressed"); return 2;
            default: Debug.LogError("Invalid mood selected. Defaulting to Calm."); UpdateObjectsState("calm"); return 0;
        }
    }
}

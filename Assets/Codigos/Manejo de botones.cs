using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Manejodebotones : MonoBehaviour
{
    // Reference to the AudioSource component
    [SerializeField] private AudioSource buttonSound;
    public TextMeshProUGUI moodLabel; 

    void Start()
    { 
        
    }

    // Method to load a scene by name
    public void EmpezarNivel(string NombreNivel)
    {
        PlayButtonSound();
        SceneManager.LoadScene(NombreNivel);
    }

    // Method to load the next scene by index
    public void CargarSiguienteNivel()
    {
        PlayButtonSound();

        // Get the current scene's index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Increment the index by 1
        int nextSceneIndex = currentSceneIndex + 1;

        // Check if the next scene index is valid
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Load the next scene
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No more scenes to load!");
        }
    }

    public void CargarNivelAnterior()
    {
        PlayButtonSound();

        // Get the current scene's index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Increment the index by 1
        int nextSceneIndex = currentSceneIndex - 1;

        // Check if the next scene index is valid
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Load the next scene
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No more scenes to load!");
        }
    }

    // Helper method to play the button sound
    private void PlayButtonSound()
    {
        if (buttonSound != null)
        {
            buttonSound.Play();
        }
        else
        {
            Debug.LogWarning("AudioSource not assigned!");
        }
    }

    // Helper method to play the button sound
    public void MoodSetter(int m)
    {
        buttonSound.Play();
        GameData.userMood=m;
         switch (m)
        {
            case 0:
                Debug.Log("calma");
                moodLabel.text="calma";
                GameData.userMood = 0;
                break;

            case 1:
                Debug.Log("normal");
                moodLabel.text="normal";
                GameData.userMood = 1;
                break;

            case 2:
                Debug.Log("estres");
                moodLabel.text="estres";
                GameData.userMood = 2;
                break;

            default:
                Debug.Log("Opción no válida. Por favor, selecciona entre 1, 2 o 3.");
                break;
        }
        
    }
}

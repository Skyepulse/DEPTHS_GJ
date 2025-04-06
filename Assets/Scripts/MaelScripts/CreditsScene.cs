using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditsScene : MonoBehaviour
{
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private Button backButton = null;

    //================================//
    private void Awake()
    {
        if (backButton == null)
        {
            Debug.LogError("Back button is not assigned!");
            return;
        }

        backButton.onClick.AddListener(OnBackButtonClicked);
    }

    //================================//
    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(OnBackButtonClicked);
    }

    //================================//
    private void OnBackButtonClicked()
    {
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainMenuSceneName);
    }
}

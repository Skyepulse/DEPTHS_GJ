using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu: MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    [SerializeField] private string creditSceneName;

    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button creditButton;

    //================================//
    public void Awake()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("Game scene is not assigned!");
            return;
        }

        if (string.IsNullOrEmpty(creditSceneName))
        {
            Debug.LogError("Credit scene is not assigned!");
            return;
        }

        if (playButton == null || exitButton == null || creditButton == null)
        {
            Debug.LogError("Buttons are not assigned!");
            return;
        }

        playButton.onClick.AddListener(OnPlayButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
        creditButton.onClick.AddListener(OnCreditButtonClicked);
    }

    //================================//
    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        exitButton.onClick.RemoveListener(OnExitButtonClicked);
        creditButton.onClick.RemoveListener(OnCreditButtonClicked);
    }

    //================================//
    public void OnPlayButtonClicked()
    {
        // Load the game scene
        SceneManager.LoadScene(gameSceneName);
    }

    //================================//
    public void OnExitButtonClicked()
    {
        // Exit the game
        Application.Quit();
    }

    //================================//
    public void OnCreditButtonClicked()
    {
        // Load the credit scene
        SceneManager.LoadScene(creditSceneName);
    }
}
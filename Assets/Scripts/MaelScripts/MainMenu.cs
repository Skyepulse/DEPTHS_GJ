using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName;
    [SerializeField] private string creditSceneName;

    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button creditButton;

    [SerializeField] private GameObject followGameObject;
    [SerializeField] private float followSpeed = 10f;

    [SerializeField] private Camera uiCamera;
    [SerializeField] private Material fullShaderMaterial;

    //================================//
    private void Awake()
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

        fullShaderMaterial.SetFloat("_Active", 0f);
    }

    //================================//
    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClicked);
        exitButton.onClick.RemoveListener(OnExitButtonClicked);
        creditButton.onClick.RemoveListener(OnCreditButtonClicked);
    }

    //================================//
    private void Update()
    {
        if (followGameObject == null || uiCamera == null)
            return;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = uiCamera.nearClipPlane + 1f; // Push slightly in front of the camera

        Vector3 worldPos = uiCamera.ScreenToWorldPoint(mousePos);
        followGameObject.transform.position = Vector3.Lerp(followGameObject.transform.position, worldPos, Time.deltaTime * followSpeed);

        // rotate on z axis to face the mouse
        Vector3 lookAt = worldPos - followGameObject.transform.position;
        lookAt.z = 0f; // Keep z axis zero to avoid tilting

        Quaternion rotation = Quaternion.LookRotation(Vector3.forward, lookAt);
        followGameObject.transform.rotation = Quaternion.Slerp(followGameObject.transform.rotation, rotation, Time.deltaTime * 10f);
    }


    //================================//
    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    //================================//
    private void OnExitButtonClicked()
    {
        Application.Quit();
    }

    //================================//
    private void OnCreditButtonClicked()
    {
        SceneManager.LoadScene(creditSceneName);
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class SkipButton : MonoBehaviour
{

    public Button yourButton;

    void Start()
    {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }

    void TaskOnClick()
    {
        Debug.Log("You have clicked the button!");
        Debug.Log("Skip button clicked");
        NextScene();
    }

    void NextScene()
    {
        // Load the main menu scene
        SceneManager.LoadScene("GameSceneTest");

        Destroy(this.gameObject);
    }
}


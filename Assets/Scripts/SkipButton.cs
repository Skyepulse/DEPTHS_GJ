using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipButton : MonoBehaviour
{
    public void NextScene()
    {
        SceneManager.LoadScene("GameSceneTest");
    }
}


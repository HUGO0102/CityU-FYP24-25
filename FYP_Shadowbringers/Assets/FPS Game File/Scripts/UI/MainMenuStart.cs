using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuStart : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("Scene1-Laboratory");
    }


    public void ExitGame()
    {
        Debug.Log("¹CÀ¸°h¥X");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

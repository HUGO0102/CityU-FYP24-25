using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;

    public GameObject startMenuUI;

    // Update is called once per frame
    void Update()
    {

        
    }



    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("FinalScene");
        Debug.Log("Starting Game...");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }

}

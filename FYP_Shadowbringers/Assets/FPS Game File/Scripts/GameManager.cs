using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject settingsUI;

    [Header("Settings Manager")]
    [SerializeField] private SettingsManager settingsManager;

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Pause UI is not assigned in GameManager!");
        }

        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Settings UI is not assigned in GameManager!");
        }

        if (settingsManager == null)
        {
            Debug.LogError("Settings Manager is not assigned in GameManager!");
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
        }
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
        Debug.Log("Game Resumed");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Game Restarted");
    }

    public void ExitGame()
    {
        Debug.Log("Game Exited");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenSettings()
    {
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
        if (settingsUI != null)
        {
            settingsUI.SetActive(true);
        }
        Debug.Log("Settings Opened");
    }

    public void SaveSettings()
    {
        if (settingsManager != null)
        {
            settingsManager.SaveSettings();
        }
        CloseSettings();
    }

    public void CancelSettings()
    {
        if (settingsManager != null)
        {
            settingsManager.CancelSettings();
        }
        CloseSettings();
    }

    private void CloseSettings()
    {
        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
        }
        Debug.Log("Settings Closed");
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
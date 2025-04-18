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
    [SerializeField] private GameObject deathUI;
    [SerializeField] private GameObject winGameUI; // �s�W Win Game UI �ޥ�

    [Header("Settings Manager")]
    [SerializeField] private SettingsManager settingsManager;

    [Header("Player Reference")]
    [SerializeField] private GameObject player; // ���a��H�]�Ҧp FPS_controller�^
    [SerializeField] private Vector3 playerStartPosition = new Vector3(0, 1, 0); // ���a��l��m

    [Header("Boss Reference")]
    [SerializeField] private Boss_Ai bossAI; // Boss_Ai �ޥ�

    private bool isPaused = false;
    private PlayerHealth playerHealth;
    private bool isGameWon = false; // �l�ܳӧQ���A

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);


            // �q�\�������J�ƥ�
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (playerHealth != null)
        {
            playerHealth.OnDeath -= HandlePlayerDeath;
        }
    }

    private void Start()
    {
        // ��l�� UI ���A
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Pause UI �����t�b GameManager ���I");
        }

        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Settings UI �����t�b GameManager ���I");
        }

        if (deathUI != null)
        {
            deathUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Death UI �����t�b GameManager ���I");
        }

        if (winGameUI != null)
        {
            winGameUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Win Game UI �����t�b GameManager ���I");
        }

        if (settingsManager == null)
        {
            Debug.LogError("Settings Manager �����t�b GameManager ���I");
        }

        // �q�\���a���`�ƥ�
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.OnDeath += HandlePlayerDeath;
            }
            else
            {
                Debug.LogError("PlayerHealth �ե�b Player �W�����I");
            }
        }

        // �d���l Boss_Ai �ޥ�
        if (bossAI == null)
        {
            GameObject bossObject = GameObject.Find("Boss_Enemy");
            if (bossObject != null)
            {
                bossAI = bossObject.GetComponent<Boss_Ai>();
                if (bossAI == null)
                {
                    Debug.LogWarning("BOSS_ENEMY �W����� Boss_Ai �ե�I");
                }
            }
            else
            {
                Debug.LogWarning("����������� BOSS_ENEMY�I");
            }
        }
    }

    private void Update()
    {
        // �����ˬd Boss ��q
        if (!isGameWon && bossAI != null && bossAI.isDead)
        {
            HandleBossDeath();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // ���m���a��m�M��q
        if (player != null)
        {
            player.transform.position = playerStartPosition;
            if (playerHealth != null)
            {
                playerHealth.ResetHealth();
            }
            Debug.Log($"���a��m���m���G{playerStartPosition}");
        }

        // ���� Death_UI �M WinGameUI
        if (deathUI != null)
        {
            deathUI.SetActive(false);
        }
        if (winGameUI != null)
        {
            winGameUI.SetActive(false);
        }

        // ���m�C�����A
        isPaused = false;
        isGameWon = false;
        Time.timeScale = 1f;

        // ���s���t Boss_Ai �ޥ�
        GameObject bossObject = GameObject.Find("BOSS_ENEMY");
        if (bossObject != null)
        {
            bossAI = bossObject.GetComponent<Boss_Ai>();
            if (bossAI == null)
            {
                Debug.LogWarning("BOSS_ENEMY �W����� Boss_Ai �ե�I");
            }
        }
        else
        {
            Debug.LogWarning("����������� BOSS_ENEMY�I");
            bossAI = null; // �p�G����� Boss�A�M���ޥ�
        }
    }

    private void HandlePlayerDeath()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // ��� Death_UI
        if (deathUI != null)
        {
            deathUI.SetActive(true);
        }
        else
        {
            Debug.LogError("Death UI �����t�b GameManager ���I");
        }

        // ������L UI
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
        if (winGameUI != null)
        {
            winGameUI.SetActive(false);
        }

        Debug.Log("���a���`�A��� Death UI�A�C���Ȱ�");
    }

    private void HandleBossDeath()
    {
        isPaused = true;
        isGameWon = true;
        Time.timeScale = 0f;

        // ��� Win Game UI
        if (winGameUI != null)
        {
            winGameUI.SetActive(true);
        }
        else
        {
            //Debug.LogError("Win Game UI �����t�b GameManager ���I");
        }

        // ������L UI
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
        if (deathUI != null)
        {
            deathUI.SetActive(false);
        }

        Debug.Log("Boss ���`�A��� Win Game UI�A�C���Ȱ�");
    }

    public void PauseGame()
    {
        if ((playerHealth != null && playerHealth.IsDead()) || isGameWon) return; // �p�G���`�γӧQ�A����Ȱ�

        isPaused = true;
        Time.timeScale = 0f;
        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
        }
        Debug.Log("�C���Ȱ�");
    }

    public void ResumeGame()
    {
        if ((playerHealth != null && playerHealth.IsDead()) || isGameWon) return; // �p�G���`�γӧQ�A�����~��

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
        Debug.Log("�C���~��");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("�C�����s�}�l");
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scene0-Menu"); // �д������A���D�������W��
        Debug.Log("��^�D���");
    }

    public void ExitGame()
    {
        Debug.Log("�C���h�X");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenSettings()
    {
        if ((playerHealth != null && playerHealth.IsDead()) || isGameWon) return; // �p�G���`�γӧQ�A����}�]�m

        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
        if (settingsUI != null)
        {
            settingsUI.SetActive(true);
        }
        Debug.Log("���}�]�m");
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
        Debug.Log("�����]�m");
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
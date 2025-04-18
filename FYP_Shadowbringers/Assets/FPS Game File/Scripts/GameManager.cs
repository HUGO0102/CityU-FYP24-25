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
    [SerializeField] private GameObject winGameUI; // 新增 Win Game UI 引用

    [Header("Settings Manager")]
    [SerializeField] private SettingsManager settingsManager;

    [Header("Player Reference")]
    [SerializeField] private GameObject player; // 玩家對象（例如 FPS_controller）
    [SerializeField] private Vector3 playerStartPosition = new Vector3(0, 1, 0); // 玩家初始位置

    [Header("Boss Reference")]
    [SerializeField] private Boss_Ai bossAI; // Boss_Ai 引用

    private bool isPaused = false;
    private PlayerHealth playerHealth;
    private bool isGameWon = false; // 追蹤勝利狀態

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);


            // 訂閱場景載入事件
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
        // 初始化 UI 狀態
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Pause UI 未分配在 GameManager 中！");
        }

        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Settings UI 未分配在 GameManager 中！");
        }

        if (deathUI != null)
        {
            deathUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Death UI 未分配在 GameManager 中！");
        }

        if (winGameUI != null)
        {
            winGameUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Win Game UI 未分配在 GameManager 中！");
        }

        if (settingsManager == null)
        {
            Debug.LogError("Settings Manager 未分配在 GameManager 中！");
        }

        // 訂閱玩家死亡事件
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.OnDeath += HandlePlayerDeath;
            }
            else
            {
                Debug.LogError("PlayerHealth 組件在 Player 上未找到！");
            }
        }

        // 查找初始 Boss_Ai 引用
        if (bossAI == null)
        {
            GameObject bossObject = GameObject.Find("Boss_Enemy");
            if (bossObject != null)
            {
                bossAI = bossObject.GetComponent<Boss_Ai>();
                if (bossAI == null)
                {
                    Debug.LogWarning("BOSS_ENEMY 上未找到 Boss_Ai 組件！");
                }
            }
            else
            {
                Debug.LogWarning("場景中未找到 BOSS_ENEMY！");
            }
        }
    }

    private void Update()
    {
        // 直接檢查 Boss 血量
        if (!isGameWon && bossAI != null && bossAI.isDead)
        {
            HandleBossDeath();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 重置玩家位置和血量
        if (player != null)
        {
            player.transform.position = playerStartPosition;
            if (playerHealth != null)
            {
                playerHealth.ResetHealth();
            }
            Debug.Log($"玩家位置重置為：{playerStartPosition}");
        }

        // 關閉 Death_UI 和 WinGameUI
        if (deathUI != null)
        {
            deathUI.SetActive(false);
        }
        if (winGameUI != null)
        {
            winGameUI.SetActive(false);
        }

        // 重置遊戲狀態
        isPaused = false;
        isGameWon = false;
        Time.timeScale = 1f;

        // 重新分配 Boss_Ai 引用
        GameObject bossObject = GameObject.Find("BOSS_ENEMY");
        if (bossObject != null)
        {
            bossAI = bossObject.GetComponent<Boss_Ai>();
            if (bossAI == null)
            {
                Debug.LogWarning("BOSS_ENEMY 上未找到 Boss_Ai 組件！");
            }
        }
        else
        {
            Debug.LogWarning("場景中未找到 BOSS_ENEMY！");
            bossAI = null; // 如果未找到 Boss，清除引用
        }
    }

    private void HandlePlayerDeath()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // 顯示 Death_UI
        if (deathUI != null)
        {
            deathUI.SetActive(true);
        }
        else
        {
            Debug.LogError("Death UI 未分配在 GameManager 中！");
        }

        // 關閉其他 UI
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

        Debug.Log("玩家死亡，顯示 Death UI，遊戲暫停");
    }

    private void HandleBossDeath()
    {
        isPaused = true;
        isGameWon = true;
        Time.timeScale = 0f;

        // 顯示 Win Game UI
        if (winGameUI != null)
        {
            winGameUI.SetActive(true);
        }
        else
        {
            //Debug.LogError("Win Game UI 未分配在 GameManager 中！");
        }

        // 關閉其他 UI
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

        Debug.Log("Boss 死亡，顯示 Win Game UI，遊戲暫停");
    }

    public void PauseGame()
    {
        if ((playerHealth != null && playerHealth.IsDead()) || isGameWon) return; // 如果死亡或勝利，阻止暫停

        isPaused = true;
        Time.timeScale = 0f;
        if (pauseUI != null)
        {
            pauseUI.SetActive(true);
        }
        Debug.Log("遊戲暫停");
    }

    public void ResumeGame()
    {
        if ((playerHealth != null && playerHealth.IsDead()) || isGameWon) return; // 如果死亡或勝利，阻止繼續

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
        Debug.Log("遊戲繼續");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("遊戲重新開始");
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Scene0-Menu"); // 請替換為你的主菜單場景名稱
        Debug.Log("返回主菜單");
    }

    public void ExitGame()
    {
        Debug.Log("遊戲退出");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OpenSettings()
    {
        if ((playerHealth != null && playerHealth.IsDead()) || isGameWon) return; // 如果死亡或勝利，阻止打開設置

        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
        if (settingsUI != null)
        {
            settingsUI.SetActive(true);
        }
        Debug.Log("打開設置");
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
        Debug.Log("關閉設置");
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
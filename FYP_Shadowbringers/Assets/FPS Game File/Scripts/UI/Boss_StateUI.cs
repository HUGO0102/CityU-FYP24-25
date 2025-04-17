using UnityEngine;
using UnityEngine.UI;

public class Boss_StateUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider bossHealthBar; // Boss_HealthBar Slider
    [SerializeField] private Slider bossShieldBar; // Boss_ShieldBar Slider

    [Header("Boss Reference")]
    [SerializeField] private Boss_Ai bossAi; // 引用 Boss_Ai 腳本

    private void Start()
    {
        // 檢查引用是否正確分配
        if (bossAi == null)
        {
            Debug.LogError("Boss_Ai reference is not assigned in Boss_StateUI!");
            return;
        }

        if (bossHealthBar == null)
        {
            Debug.LogError("Boss_HealthBar Slider is not assigned in Boss_StateUI!");
            return;
        }

        if (bossShieldBar == null)
        {
            Debug.LogError("Boss_ShieldBar Slider is not assigned in Boss_StateUI!");
            return;
        }

        // 設置 Slider 範圍
        bossHealthBar.minValue = 0;
        bossHealthBar.maxValue = bossAi.maxHealth; // 使用 Boss_Ai 的 maxHealth
        bossShieldBar.minValue = 0;
        bossShieldBar.maxValue = bossAi.GetMaxShieldHealth(); // 使用 Boss_Ai 的 maxShieldHealth

        // 初始更新 Slider
        UpdateHealthBar();
        UpdateShieldBar();
    }

    private void Update()
    {
        // 每幀更新 Slider
        UpdateHealthBar();
        UpdateShieldBar();
    }

    private void UpdateHealthBar()
    {
        if (bossHealthBar != null)
        {
            bossHealthBar.value = bossAi.health; // 直接使用 health 值
            Debug.Log($"Boss Health: {bossAi.health}/{bossAi.maxHealth}, Slider Value: {bossHealthBar.value}");
        }
    }

    private void UpdateShieldBar()
    {
        if (bossShieldBar != null)
        {
            // 如果護盾未激活，將 Slider 值設為 0
            if (!bossAi.IsShieldActive())
            {
                bossShieldBar.value = 0;
            }
            else
            {
                bossShieldBar.value = bossAi.GetShieldHealth(); // 直接使用 shieldHealth 值
            }

            // 動態更新 Slider 的最大值（因為 maxShieldHealth 可能在遊戲中變化）
            bossShieldBar.maxValue = bossAi.GetMaxShieldHealth();
            Debug.Log($"Boss Shield Health: {bossAi.GetShieldHealth()}/{bossAi.GetMaxShieldHealth()}, Slider Value: {bossShieldBar.value}");
        }
    }
}
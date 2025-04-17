using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    private float stamina;
    private float lerpTimer;

    [Header("Stamina Bar")]
    public float maxStamina = 100f; // 最大體力值（與 PlayerMotor 同步）
    public float chipSpeed = 2f; // 背景條追隨前景條的速度（延遲效果）
    public Slider frontStaminaBar;
    public Slider backStaminaBar;
    public Image backStaminaFill; // 僅保留 BackSlider -> Fill 的 Image 用於改變顏色

    private PlayerMotor playerMotor; // 引用 PlayerMotor 來獲取體力值
    private float previousStamina; // 記錄上一次的體力值，用於判斷體力是增加還是減少

    void Start()
    {
        // 獲取 PlayerMotor 組件
        playerMotor = FindObjectOfType<PlayerMotor>();
        if (playerMotor == null)
        {
            Debug.LogError("PlayerMotor not found in the scene!");
        }

        // 初始化體力值
        stamina = maxStamina;
        previousStamina = stamina;

        // 初始更新 Slider
        UpdateStaminaUI();
    }

    void Update()
    {
        // 從 PlayerMotor 獲取當前體力值
        if (playerMotor != null)
        {
            stamina = playerMotor.currentStamina;
            maxStamina = playerMotor.maxStamina;
        }

        // 確保體力值在合理範圍內
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        UpdateStaminaUI();

        // 更新 previousStamina，以便下一次 Update 時判斷體力變化方向
        previousStamina = stamina;
    }

    public void UpdateStaminaUI()
    {
        // 前景條即時更新為當前 stamina 值
        if (frontStaminaBar != null)
            frontStaminaBar.value = stamina;

        // 背景條根據體力值變化顯示延遲效果
        if (backStaminaBar != null)
        {
            float fillB = backStaminaBar.value;

            // 判斷體力是減少還是恢復（使用 previousStamina 比較）
            bool isStaminaDecreasing = stamina < previousStamina;

            // 只要 fillB 不等於 stamina，就觸發延遲效果
            if (Mathf.Abs(fillB - stamina) > 0.01f) // 添加小容差，避免微小差異觸發
            {
                if (isStaminaDecreasing)
                {
                    if (backStaminaFill != null)
                        backStaminaFill.color = Color.red; // 背景條變紅
                }
                else
                {
                    if (backStaminaFill != null)
                        backStaminaFill.color = Color.green; // 背景條變綠
                }

                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete; // 平方效果，使過渡更平滑
                backStaminaBar.value = Mathf.Lerp(fillB, stamina, percentComplete);
            }
            else
            {
                // 如果 fillB 已經接近 stamina，重置 lerpTimer
                lerpTimer = 0f;
            }
        }
    }

    public void OnStaminaChanged()
    {
        lerpTimer = 0f;
    }
}
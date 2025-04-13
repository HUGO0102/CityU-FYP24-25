using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    private float stamina;
    private float lerpTimer;

    [Header("Stamina Bar")]
    public float maxStamina = 100f; // 最大體力值（與 PlayerMotor 同步）
    public float chipSpeed = 2f; // 背景條追隨前景條的速度（延遲效果）
    public Image frontStaminaBar; // 前景體力條（黃色部分）
    public Image backStaminaBar; // 背景體力條（白色部分）

    private PlayerMotor playerMotor; // 引用 PlayerMotor 來獲取體力值
    private float previousStamina; // 記錄上一次的體力值，用於判斷體力是增加還是減少
    private bool refilling;

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
        float fillF = frontStaminaBar.fillAmount; // 前景條的填充比例
        float fillB = backStaminaBar.fillAmount; // 背景條的填充比例
        float sFraction = stamina / maxStamina; // 體力值的比例

        // 判斷體力是減少還是恢復
        bool isStaminaDecreasing = stamina < previousStamina;

        if (isStaminaDecreasing) // 體力減少時：BackStaminaBar 帶有延遲效果
        {
            frontStaminaBar.fillAmount = sFraction; // 前景條即時更新
            backStaminaBar.color = Color.red; // 背景條為白色（正常情況）

            // 背景條延遲跟隨
            if (fillB > sFraction)
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete; // 平方效果，使過渡更平滑
                backStaminaBar.fillAmount = Mathf.Lerp(fillB, sFraction, percentComplete);
            }
        }
        else // 體力恢復時：BackStaminaBar 領先並顯示為綠色
        {
            backStaminaBar.color = Color.green; // 背景條變為綠色（表示恢復）
            backStaminaBar.fillAmount = sFraction; // 背景條即時更新
            if (fillF < sFraction)
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                frontStaminaBar.fillAmount = Mathf.Lerp(fillF, sFraction, percentComplete);
            }
        }

       
    }

    public void OnStaminaChanged()
    {
        lerpTimer = 0f;
    }
}
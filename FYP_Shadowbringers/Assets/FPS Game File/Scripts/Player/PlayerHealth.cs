using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health;
    private float lerpTimer;

    [Header("Health Bar")]
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Slider frontHealthBar;
    public Slider backHealthBar;
    public Image backHealthFill; // 僅保留 BackSlider -> Fill 的 Image 用於改變顏色

    [Header("Damage Overlay")]
    public Image overlay; // our DamageOverlay Gameobject
    public float duration; // how long the image stays fully opaque
    public float fadeSpeed; // how quickly the image will fade

    private float durationTimer; // timer to check against the duration
    public event Action OnDeath; // 新增死亡事件

    void Start()
    {
        health = maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);

        // 初始更新 Slider
        UpdateHealthUI();
    }

    void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        UpdateHealthUI();

        if (overlay.color.a > 0)
        {
            if (health < 30)
                return;

            durationTimer += Time.deltaTime;
            if (durationTimer > duration)
            {
                // fade the image
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadeSpeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }
    }

    public void UpdateHealthUI()
    {
        // 前景條即時更新為當前 health 值
        if (frontHealthBar != null)
            frontHealthBar.value = health;

        // 背景條根據生命值變化顯示延遲效果
        if (backHealthBar != null)
        {
            float fillB = backHealthBar.value;
            float previousFillB = fillB; // 記錄當前的背景條值，用於判斷變化方向

            // 判斷生命值是減少還是恢復
            bool isHealthDecreasing = health < previousFillB;

            if (isHealthDecreasing && fillB > health) // 生命值減少
            {
                if (backHealthFill != null)
                    backHealthFill.color = Color.red; // 背景條變紅
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete; // 平方效果，使過渡更平滑
                backHealthBar.value = Mathf.Lerp(fillB, health, percentComplete);
            }
            else if (!isHealthDecreasing && fillB < health) // 生命值恢復
            {
                if (backHealthFill != null)
                    backHealthFill.color = Color.green; // 背景條變綠
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;
                backHealthBar.value = Mathf.Lerp(fillB, health, percentComplete);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        lerpTimer = 0f;
        durationTimer = 0;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 1);

        // 檢查血量是否降為 0
        if (health <= 0)
        {
            health = 0;
            OnDeath?.Invoke(); // 觸發死亡事件
            Debug.Log("Player died");
        }
    }

    public void RestoreHealth(float healAmount)
    {
        health += healAmount;
        lerpTimer = 0f;
    }

    public void ResetHealth()
    {
        health = maxHealth;
        lerpTimer = 0f;
        durationTimer = 0;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);
        UpdateHealthUI();
        Debug.Log("Player health reset");
    }

    public bool IsDead()
    {
        return health <= 0;
    }
}
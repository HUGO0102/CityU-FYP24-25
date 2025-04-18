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
    public Image backHealthFill; // �ȫO�d BackSlider -> Fill �� Image �Ω�����C��

    [Header("Damage Overlay")]
    public Image overlay; // our DamageOverlay Gameobject
    public float duration; // how long the image stays fully opaque
    public float fadeSpeed; // how quickly the image will fade

    private float durationTimer; // timer to check against the duration
    public event Action OnDeath; // �s�W���`�ƥ�

    void Start()
    {
        health = maxHealth;
        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);

        // ��l��s Slider
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
        // �e�����Y�ɧ�s����e health ��
        if (frontHealthBar != null)
            frontHealthBar.value = health;

        // �I�����ھڥͩR���ܤ���ܩ���ĪG
        if (backHealthBar != null)
        {
            float fillB = backHealthBar.value;
            float previousFillB = fillB; // �O����e���I�����ȡA�Ω�P�_�ܤƤ�V

            // �P�_�ͩR�ȬO����٬O��_
            bool isHealthDecreasing = health < previousFillB;

            if (isHealthDecreasing && fillB > health) // �ͩR�ȴ��
            {
                if (backHealthFill != null)
                    backHealthFill.color = Color.red; // �I�����ܬ�
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete; // ����ĪG�A�ϹL��󥭷�
                backHealthBar.value = Mathf.Lerp(fillB, health, percentComplete);
            }
            else if (!isHealthDecreasing && fillB < health) // �ͩR�ȫ�_
            {
                if (backHealthFill != null)
                    backHealthFill.color = Color.green; // �I�����ܺ�
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

        // �ˬd��q�O�_���� 0
        if (health <= 0)
        {
            health = 0;
            OnDeath?.Invoke(); // Ĳ�o���`�ƥ�
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
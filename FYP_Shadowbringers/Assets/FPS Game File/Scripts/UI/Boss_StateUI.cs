using UnityEngine;
using UnityEngine.UI;

public class Boss_StateUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider bossHealthBar; // Boss_HealthBar Slider
    [SerializeField] private Slider bossShieldBar; // Boss_ShieldBar Slider

    [Header("Boss Reference")]
    [SerializeField] private Boss_Ai bossAi; // �ޥ� Boss_Ai �}��

    private void Start()
    {
        // �ˬd�ޥάO�_���T���t
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

        // �]�m Slider �d��
        bossHealthBar.minValue = 0;
        bossHealthBar.maxValue = bossAi.maxHealth; // �ϥ� Boss_Ai �� maxHealth
        bossShieldBar.minValue = 0;
        bossShieldBar.maxValue = bossAi.GetMaxShieldHealth(); // �ϥ� Boss_Ai �� maxShieldHealth

        // ��l��s Slider
        UpdateHealthBar();
        UpdateShieldBar();
    }

    private void Update()
    {
        // �C�V��s Slider
        UpdateHealthBar();
        UpdateShieldBar();
    }

    private void UpdateHealthBar()
    {
        if (bossHealthBar != null)
        {
            bossHealthBar.value = bossAi.health; // �����ϥ� health ��
            Debug.Log($"Boss Health: {bossAi.health}/{bossAi.maxHealth}, Slider Value: {bossHealthBar.value}");
        }
    }

    private void UpdateShieldBar()
    {
        if (bossShieldBar != null)
        {
            // �p�G�@�ޥ��E���A�N Slider �ȳ]�� 0
            if (!bossAi.IsShieldActive())
            {
                bossShieldBar.value = 0;
            }
            else
            {
                bossShieldBar.value = bossAi.GetShieldHealth(); // �����ϥ� shieldHealth ��
            }

            // �ʺA��s Slider ���̤j�ȡ]�]�� maxShieldHealth �i��b�C�����ܤơ^
            bossShieldBar.maxValue = bossAi.GetMaxShieldHealth();
            Debug.Log($"Boss Shield Health: {bossAi.GetShieldHealth()}/{bossAi.GetMaxShieldHealth()}, Slider Value: {bossShieldBar.value}");
        }
    }
}
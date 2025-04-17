using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    private float stamina;
    private float lerpTimer;

    [Header("Stamina Bar")]
    public float maxStamina = 100f; // �̤j��O�ȡ]�P PlayerMotor �P�B�^
    public float chipSpeed = 2f; // �I�����l�H�e�������t�ס]����ĪG�^
    public Slider frontStaminaBar;
    public Slider backStaminaBar;
    public Image backStaminaFill; // �ȫO�d BackSlider -> Fill �� Image �Ω�����C��

    private PlayerMotor playerMotor; // �ޥ� PlayerMotor �������O��
    private float previousStamina; // �O���W�@������O�ȡA�Ω�P�_��O�O�W�[�٬O���

    void Start()
    {
        // ��� PlayerMotor �ե�
        playerMotor = FindObjectOfType<PlayerMotor>();
        if (playerMotor == null)
        {
            Debug.LogError("PlayerMotor not found in the scene!");
        }

        // ��l����O��
        stamina = maxStamina;
        previousStamina = stamina;

        // ��l��s Slider
        UpdateStaminaUI();
    }

    void Update()
    {
        // �q PlayerMotor �����e��O��
        if (playerMotor != null)
        {
            stamina = playerMotor.currentStamina;
            maxStamina = playerMotor.maxStamina;
        }

        // �T�O��O�Ȧb�X�z�d��
        stamina = Mathf.Clamp(stamina, 0, maxStamina);
        UpdateStaminaUI();

        // ��s previousStamina�A�H�K�U�@�� Update �ɧP�_��O�ܤƤ�V
        previousStamina = stamina;
    }

    public void UpdateStaminaUI()
    {
        // �e�����Y�ɧ�s����e stamina ��
        if (frontStaminaBar != null)
            frontStaminaBar.value = stamina;

        // �I�����ھ���O���ܤ���ܩ���ĪG
        if (backStaminaBar != null)
        {
            float fillB = backStaminaBar.value;

            // �P�_��O�O����٬O��_�]�ϥ� previousStamina ����^
            bool isStaminaDecreasing = stamina < previousStamina;

            // �u�n fillB ������ stamina�A�NĲ�o����ĪG
            if (Mathf.Abs(fillB - stamina) > 0.01f) // �K�[�p�e�t�A�קK�L�p�t��Ĳ�o
            {
                if (isStaminaDecreasing)
                {
                    if (backStaminaFill != null)
                        backStaminaFill.color = Color.red; // �I�����ܬ�
                }
                else
                {
                    if (backStaminaFill != null)
                        backStaminaFill.color = Color.green; // �I�����ܺ�
                }

                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete; // ����ĪG�A�ϹL��󥭷�
                backStaminaBar.value = Mathf.Lerp(fillB, stamina, percentComplete);
            }
            else
            {
                // �p�G fillB �w�g���� stamina�A���m lerpTimer
                lerpTimer = 0f;
            }
        }
    }

    public void OnStaminaChanged()
    {
        lerpTimer = 0f;
    }
}
using UnityEngine;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    private float stamina;
    private float lerpTimer;

    [Header("Stamina Bar")]
    public float maxStamina = 100f; // �̤j��O�ȡ]�P PlayerMotor �P�B�^
    public float chipSpeed = 2f; // �I�����l�H�e�������t�ס]����ĪG�^
    public Image frontStaminaBar; // �e����O���]���ⳡ���^
    public Image backStaminaBar; // �I����O���]�զⳡ���^

    private PlayerMotor playerMotor; // �ޥ� PlayerMotor �������O��
    private float previousStamina; // �O���W�@������O�ȡA�Ω�P�_��O�O�W�[�٬O���
    private bool refilling;

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
        float fillF = frontStaminaBar.fillAmount; // �e��������R���
        float fillB = backStaminaBar.fillAmount; // �I��������R���
        float sFraction = stamina / maxStamina; // ��O�Ȫ����

        // �P�_��O�O����٬O��_
        bool isStaminaDecreasing = stamina < previousStamina;

        if (isStaminaDecreasing) // ��O��֮ɡGBackStaminaBar �a������ĪG
        {
            frontStaminaBar.fillAmount = sFraction; // �e�����Y�ɧ�s
            backStaminaBar.color = Color.red; // �I�������զ�]���`���p�^

            // �I����������H
            if (fillB > sFraction)
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete; // ����ĪG�A�ϹL��󥭷�
                backStaminaBar.fillAmount = Mathf.Lerp(fillB, sFraction, percentComplete);
            }
        }
        else // ��O��_�ɡGBackStaminaBar �������ܬ����
        {
            backStaminaBar.color = Color.green; // �I�����ܬ����]��ܫ�_�^
            backStaminaBar.fillAmount = sFraction; // �I�����Y�ɧ�s
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
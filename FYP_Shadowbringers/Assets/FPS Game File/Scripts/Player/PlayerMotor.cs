using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Nokobot/Instruments/Player Motor")]
public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private Vector3 moveDirection;
    private bool isGrounded;
    public float speed = 4f;
    public float runSpeed = 6f;
    public float currentSpeed = 0;
    public float gravity = -9.8f;
    public float jumpHeight = 1.5f;
    public float friction = 5f; // �����O�A����D�ʴ�t�t��

    public bool lerpCrouch;
    public bool crouching;
    public bool sprinting;
    public float crouchTimer;

    [SerializeField] public float maxStamina = 100f;
    [SerializeField] public float currentStamina;
    [SerializeField] private float staminaDrainRate = 10f; // �C����Ӫ��h�ҭ�
    [SerializeField] private float staminaRegenRate = 5f;  // �C���_���h�ҭ�

    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float sprinStepMultipler = 0.6f;
    private float footstepTimer = 0;
    private float GetCurrenOffset => sprinting ? baseStepSpeed * sprinStepMultipler : baseStepSpeed;

    [Header("SoundFX")]
    public AudioSource myAudioSource;
    public AudioClip[] myAudioClip;
    [Range(0.1f, 0.5f)]
    public float volumeChangeMultiplier = 0.2f;
    [Range(0.1f, 0.5f)]
    public float pitchChangeMultiplier = 0.2f;

    public bool useFootsteps = true;

    // �ޥ� Stamina UI �}��
    private PlayerStamina staminaUI;

    // ���} currentStamina �M maxStamina�A�Ѩ�L�}���]�p PlayerStaminaUI�^�X��
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = speed;
        currentStamina = maxStamina;

        // ��� PlayerStaminaUI �ե�
        staminaUI = FindObjectOfType<PlayerStamina>();
        if (staminaUI == null)
        {
            Debug.LogError("PlayerStaminaUI not found in the scene!");
        }
    }

    void Update()
    {
        isGrounded = controller.isGrounded;

        float previousStamina = currentStamina; // �O����s�e����O��

        if (sprinting && currentStamina > 0)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            if (currentStamina <= 0)
            {
                sprinting = false; // �h�үӺɡA����]�B
                // �����m currentSpeed�A���D���~��
            }
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        }

        // �p�G��O�ȧ��ܡA�q�� Stamina UI
        if (previousStamina != currentStamina && staminaUI != null)
        {
            staminaUI.OnStaminaChanged();
        }

        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float p = crouchTimer / 1;
            p *= p;
            if (crouching)
                controller.height = Mathf.Lerp(controller.height, 1, p);
            else
                controller.height = Mathf.Lerp(controller.height, 2, p);

            if (p > 1)
            {
                lerpCrouch = false;
                crouchTimer = 0f;
            }
        }

        // �p�G���a�b�a�W�A�B�S�����ʿ�J�A�v����t�]���������^
        if (isGrounded && moveDirection == Vector3.zero && currentSpeed > 0)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, 0, friction * Time.deltaTime);
            if (currentSpeed < 0.01f)
            {
                currentSpeed = 0;
            }
        }
    }

    public void ProcessMove(Vector2 input)
    {
        Vector3 previousMoveDirection = moveDirection; // �O�s���e�����ʤ�V
        moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        // �p�G�����ʿ�J�A��s�t��
        if (moveDirection != Vector3.zero)
        {
            float targetSpeed = sprinting ? runSpeed : speed;
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, 0.1f); // ���ƹL���ؼгt��
        }
        else if (!isGrounded)
        {
            // �p�G���a�b�Ť��A�O�d���e�����ʤ�V�H�O���D��
            moveDirection = previousMoveDirection;
        }

        controller.Move(transform.TransformDirection(moveDirection) * currentSpeed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        if (isGrounded && playerVelocity.y < 0)
            playerVelocity.y = -2f;
        controller.Move(playerVelocity * Time.deltaTime);

        SFX();
    }

    private void SFX()
    {
        if (!isGrounded) return;
        if (moveDirection == Vector3.zero) return;
        if (!useFootsteps) return;

        AudioClip ramdomSFX = myAudioClip[Random.Range(0, 3)];
        myAudioSource.volume = Random.Range(1 - volumeChangeMultiplier, 1);
        myAudioSource.pitch = Random.Range(1 - pitchChangeMultiplier, 1 + pitchChangeMultiplier);

        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0)
        {
            myAudioSource.PlayOneShot(ramdomSFX);
            footstepTimer = GetCurrenOffset;
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }

    public void Crouch()
    {
        crouching = !crouching;
        crouchTimer = 0;
        lerpCrouch = true;
    }

    public void Sprint(bool isPressed)
    {
        sprinting = isPressed;
        // ���������� currentSpeed�A���D�ʳB�z
        if (staminaUI != null)
        {
            staminaUI.OnStaminaChanged();
        }
    }
}
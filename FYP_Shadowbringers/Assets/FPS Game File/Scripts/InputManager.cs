using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    public PlayerInput.OnFootActions onFoot;
    public PlayerInput.OnHandActions onHand;
    private InputAction escapeAction;

    private PlayerMotor motor;
    private PlayerLook look;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        onHand = playerInput.OnHand;

        escapeAction = playerInput.FindAction("Escape");
        if (escapeAction == null)
        {
            Debug.LogError("Escape action not found in PlayerInput!");
        }

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();

        onFoot.Jump.performed += ctx => motor.Jump();
        onFoot.Crouch.performed += ctx => motor.Crouch();
        onFoot.Sprint.performed += ctx => motor.Sprint(true);
        onFoot.Sprint.canceled += ctx => motor.Sprint(false);

        if (escapeAction != null)
        {
            escapeAction.performed += ctx => TogglePause();
        }
    }

    private void Update()
    {
        // �ھڹC���O�_�Ȱ������
        if (GameManager.Instance.IsPaused())
        {
            Cursor.lockState = CursorLockMode.None; // ���깫��
            Cursor.visible = true; // ��ܹ���
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined; // ��w����
            Cursor.visible = false; // ���ù���
        }
    }

    void FixedUpdate()
    {
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    private void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
        onHand.Enable();
        if (escapeAction != null)
        {
            escapeAction.Enable();
        }
    }

    private void OnDisable()
    {
        onFoot.Disable();
        onHand.Disable();
        if (escapeAction != null)
        {
            escapeAction.Disable();
            escapeAction.performed -= ctx => TogglePause();
        }

        onFoot.Jump.performed -= ctx => motor.Jump();
        onFoot.Crouch.performed -= ctx => motor.Crouch();
        onFoot.Sprint.performed -= ctx => motor.Sprint(true);
        onFoot.Sprint.canceled -= ctx => motor.Sprint(false);
    }

    private void TogglePause()
    {
        if (GameManager.Instance.IsPaused())
        {
            GameManager.Instance.ResumeGame();
        }
        else
        {
            GameManager.Instance.PauseGame();
        }
    }
}
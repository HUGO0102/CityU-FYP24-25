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
        // 根據遊戲是否暫停控制鼠標
        if (GameManager.Instance.IsPaused())
        {
            Cursor.lockState = CursorLockMode.None; // 解鎖鼠標
            Cursor.visible = true; // 顯示鼠標
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined; // 鎖定鼠標
            Cursor.visible = false; // 隱藏鼠標
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
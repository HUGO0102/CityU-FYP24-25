using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedField : MonoBehaviour
{
    public GameObject player;
    public float originalSpeed;
    public float originalJumpF;
    public float originalSprint;
    public float speedMultiplier = 2f; // 加速倍數，例如 2 表示速度和跳躍高度加倍

    public void Awake()
    {
        player = GameObject.Find("Player"); // 修正為正確的玩家對象名稱
        if (player == null)
        {
            Debug.LogError("Player (FPS_controller) not found in the scene!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMotor>() == null)
        {
            return;
        }

        // 獲取玩家的 PlayerMotor 組件
        PlayerMotor pm = player.GetComponent<PlayerMotor>();
        if (pm == null)
        {
            Debug.LogError("PlayerMotor component not found on FPS_controller!");
            return;
        }

        // 保存原始速度和跳躍高度
        originalSpeed = pm.speed;
        originalJumpF = pm.jumpHeight;
        originalSprint = pm.runSpeed;

        // 應用加速效果（包括速度和跳躍高度）
        pm.currentSpeed = originalSpeed * speedMultiplier;
        pm.runSpeed = originalSprint * speedMultiplier;
        pm.jumpHeight = originalJumpF * speedMultiplier * 2;
        pm.useFootsteps = false;

        Debug.Log($"Player entered SpeedForceField: Speed increased to {pm.currentSpeed}, Run Speed: {pm.runSpeed}, Jump Height: {pm.jumpHeight}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMotor>() == null)
        {
            return;
        }

        // 獲取玩家的 PlayerMotor 組件
        PlayerMotor pm = player.GetComponent<PlayerMotor>();
        if (pm == null)
        {
            Debug.LogError("PlayerMotor component not found on FPS_controller!");
            return;
        }

        // 恢復原始速度和跳躍高度
        pm.currentSpeed = originalSpeed;
        pm.speed = originalSpeed;
        pm.runSpeed = originalSprint;
        pm.jumpHeight = originalJumpF;
        pm.useFootsteps = true;

        Debug.Log($"Player exited SpeedForceField: Speed restored to {pm.speed}, Run Speed: {pm.runSpeed}, Jump Height: {pm.jumpHeight}");
    }
}
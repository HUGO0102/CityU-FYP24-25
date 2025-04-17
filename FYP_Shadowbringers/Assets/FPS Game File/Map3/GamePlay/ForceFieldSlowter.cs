using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldSlowter : MonoBehaviour
{
    public GameObject player;
    public float originalSpeed;
    public float originalJumpF;
    public float originalSprint;
    public float slowValue = 0.5f;

    public void Awake()
    {
        player = GameObject.Find("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMotor>() == null)
        {
            return;
        }

        // 獲取玩家的 PlayerMotor 組件
        PlayerMotor pm = player.GetComponent<PlayerMotor>();

        // 保存原始速度和跳躍高度
        originalSpeed = pm.speed;
        originalJumpF = pm.jumpHeight;
        originalSprint = pm.runSpeed;

        // 應用減速效果
        pm.speed = originalSpeed * slowValue;
        pm.runSpeed = originalSprint * slowValue;
        pm.jumpHeight = originalJumpF * slowValue;
        pm.useFootsteps = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerMotor>() == null)
        {
            return;
        }

        // 獲取玩家的 PlayerMotor 組件
        PlayerMotor pm = player.GetComponent<PlayerMotor>();

        // 恢復原始速度和跳躍高度
        pm.speed = originalSpeed;
        pm.jumpHeight = originalJumpF;
        pm.runSpeed = originalSprint;
        pm.useFootsteps = true;
    }
}
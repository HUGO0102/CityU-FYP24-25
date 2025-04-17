using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedField : MonoBehaviour
{
    public GameObject player;
    public float originalSpeed;
    public float originalJumpF;
    public float originalSprint;
    public float speedMultiplier = 2f; // �[�t���ơA�Ҧp 2 ��ܳt�שM���D���ץ[��

    public void Awake()
    {
        player = GameObject.Find("Player"); // �ץ������T�����a��H�W��
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

        // ������a�� PlayerMotor �ե�
        PlayerMotor pm = player.GetComponent<PlayerMotor>();
        if (pm == null)
        {
            Debug.LogError("PlayerMotor component not found on FPS_controller!");
            return;
        }

        // �O�s��l�t�שM���D����
        originalSpeed = pm.speed;
        originalJumpF = pm.jumpHeight;
        originalSprint = pm.runSpeed;

        // ���Υ[�t�ĪG�]�]�A�t�שM���D���ס^
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

        // ������a�� PlayerMotor �ե�
        PlayerMotor pm = player.GetComponent<PlayerMotor>();
        if (pm == null)
        {
            Debug.LogError("PlayerMotor component not found on FPS_controller!");
            return;
        }

        // ��_��l�t�שM���D����
        pm.currentSpeed = originalSpeed;
        pm.speed = originalSpeed;
        pm.runSpeed = originalSprint;
        pm.jumpHeight = originalJumpF;
        pm.useFootsteps = true;

        Debug.Log($"Player exited SpeedForceField: Speed restored to {pm.speed}, Run Speed: {pm.runSpeed}, Jump Height: {pm.jumpHeight}");
    }
}
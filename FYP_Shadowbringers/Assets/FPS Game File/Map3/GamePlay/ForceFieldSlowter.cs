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

        // ������a�� PlayerMotor �ե�
        PlayerMotor pm = player.GetComponent<PlayerMotor>();

        // �O�s��l�t�שM���D����
        originalSpeed = pm.speed;
        originalJumpF = pm.jumpHeight;
        originalSprint = pm.runSpeed;

        // ���δ�t�ĪG
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

        // ������a�� PlayerMotor �ե�
        PlayerMotor pm = player.GetComponent<PlayerMotor>();

        // ��_��l�t�שM���D����
        pm.speed = originalSpeed;
        pm.jumpHeight = originalJumpF;
        pm.runSpeed = originalSprint;
        pm.useFootsteps = true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public Camera cam;
    private float xRotation = 0f;

    private float baseXSensitivity = 30f; // 基礎靈敏度
    private float baseYSensitivity = 30f;
    public float xSensitivity = 30f; // 最終靈敏度
    public float ySensitivity = 30f;

    public void ProcessLook(Vector2 input)
    {
        float mouseX = input.x;
        float mouseY = input.y;
        // calculate camera rotation for looking up and down
        xRotation -= (mouseY * Time.deltaTime) * ySensitivity;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        // apply this to our camera transform
        cam.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        // rotate player to look left and right
        transform.Rotate(Vector3.up * (mouseX * Time.deltaTime) * xSensitivity);
    }

    // 修改方法：使用滑塊值作為乘數
    public void SetSensitivity(float multiplier)
    {
        xSensitivity = baseXSensitivity * multiplier;
        ySensitivity = baseYSensitivity * multiplier;
        Debug.Log($"Mouse Sensitivity set to: xSensitivity={xSensitivity}, ySensitivity={ySensitivity} (Multiplier: {multiplier})");
    }
}
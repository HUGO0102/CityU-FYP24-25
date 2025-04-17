using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    private InputManager inputManager;
    public GuitarShoot Shoot;
    public BeatCenter beatCenter;

    // Start is called before the first frame update
    void Start()
    {
        inputManager = GetComponent<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("LeftMousePressing");
            Shoot.Fire();
            bool hitOnBeat = beatCenter.leftBarInCenter && beatCenter.rightBarInCenter; // 直接使用狀態
            Shoot.Shoot(hitOnBeat); // 傳遞 HitOnBeat 條件

            if (hitOnBeat)
            {
                Debug.Log("Hit On Beat!!");
                beatCenter.AnimateImage(); // 觸發中心圓圈動畫
            }
        }
    }
}
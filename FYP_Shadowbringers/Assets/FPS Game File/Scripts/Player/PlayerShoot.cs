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
            bool hitOnBeat = beatCenter.leftBarInCenter && beatCenter.rightBarInCenter; // �����ϥΪ��A
            Shoot.Shoot(hitOnBeat); // �ǻ� HitOnBeat ����

            if (hitOnBeat)
            {
                Debug.Log("Hit On Beat!!");
                beatCenter.AnimateImage(); // Ĳ�o���߶��ʵe
            }
        }
    }
}
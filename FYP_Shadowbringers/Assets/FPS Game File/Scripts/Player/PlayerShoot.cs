using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{

    private InputManager inputManager;
    public SimpleShoot Shoot;
    public BeatCenter beatCenter;


    // Start is called before the first frame update
    void Start()
    {
        inputManager = GetComponent<InputManager>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //inputManager.onHand.Shoot.triggered
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("LeftMousePressing");
            Shoot.Fire();


            if (beatCenter.leftBarInCenter && beatCenter.rightBarInCenter)
            {
                Debug.Log("Hit On Beat!!");
                beatCenter.HitOnBeat();
            }
        }

        
    }
}

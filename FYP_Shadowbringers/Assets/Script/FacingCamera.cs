using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingCamera : MonoBehaviour
{
    public Camera mainCamera; // Reference to the camera

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (mainCamera != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(mainCamera.transform.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
    }
}

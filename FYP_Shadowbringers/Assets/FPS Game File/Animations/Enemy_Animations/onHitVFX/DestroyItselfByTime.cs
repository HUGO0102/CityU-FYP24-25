using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyItselfByTime : MonoBehaviour
{
    public float Time = 2f;


    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyObject", Time);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}

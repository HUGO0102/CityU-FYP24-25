using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemyBullet : MonoBehaviour
{    
    private void Start()
    {
        Destroy(gameObject, 5f);
    }    

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Transform hitTransform = collision.transform;
    //    if (hitTransform.CompareTag("Player"))
    //    {
    //        Debug.Log("Hit Player");
    //        Destroy(gameObject);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        Transform hitTransform = other.transform;
        if (hitTransform.CompareTag("Player"))
        {
            Debug.Log("Hit Player");
            Destroy(gameObject);
        }
    }
}

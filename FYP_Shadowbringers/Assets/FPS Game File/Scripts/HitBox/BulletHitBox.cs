using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitBox : MonoBehaviour
{
    public GameObject Bullet;

    void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Untagged") || (other.gameObject.tag == "Player"))
        {
            DestroySelf();
        }
    }

    void DestroySelf()
    {
        Destroy(Bullet);
        Debug.Log("Bullet Destroyed");
    }
}
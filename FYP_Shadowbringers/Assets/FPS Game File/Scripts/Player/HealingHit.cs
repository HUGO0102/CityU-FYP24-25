using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingHit : MonoBehaviour
{
    public PlayerHealth Health;
    public GameObject gameObj;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObj);
        }
    }
}

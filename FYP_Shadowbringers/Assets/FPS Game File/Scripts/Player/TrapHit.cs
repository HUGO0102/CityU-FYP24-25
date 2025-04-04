using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapHit : MonoBehaviour
{

    public PlayerHealth Health;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Trap")
        {
            Debug.Log("Trap Hit");
            Health.TakeDamage(30);
        }
    }
}
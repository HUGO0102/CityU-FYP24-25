using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingHit : MonoBehaviour
{
    public PlayerHealth Health;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Trap")
        {
            Debug.Log("Trap Hit");
            Health.RestoreHealth(30);
            Destroy(gameObject);
        }
    }
}

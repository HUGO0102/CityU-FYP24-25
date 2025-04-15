using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit_Player : MonoBehaviour
{
    public PlayerHealth Health;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "EnemyBullets")
        {
           Health.TakeDamage(5);
            Debug.Log("Hit Player");
        }

        if (other.gameObject.tag == "SniperBullets")
        {
            Health.TakeDamage(20);
            Debug.Log("Hit Player");
        }

        if (other.gameObject.tag == "MeleeEnemy_Attack")
        {
            Health.TakeDamage(5);
            Debug.Log("Hit Player");
        }
    }
}

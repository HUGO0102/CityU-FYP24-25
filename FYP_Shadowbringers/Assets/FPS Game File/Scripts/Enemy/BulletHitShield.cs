using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitShield : MonoBehaviour
{

    private Boss_Ai boss; // �ޥ� Boss_Ai �}��
    public int bulletDamage = 20;

    private void Awake()
    {
        boss = GetComponentInParent<Boss_Ai>();
        if (boss == null)
        {
            Debug.LogError("Boss_Ai script not found in parent hierarchy!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PlayerBullets" || collision.gameObject.tag == "PlayerBrustBullets")
        {
            // �B�z�@�޶ˮ`
            if (boss != null && boss.IsShieldActive())
            {
                boss.TakeShieldDamage(bulletDamage);
                //Debug.Log("Damaged Shield");
            }
        }
    }
}

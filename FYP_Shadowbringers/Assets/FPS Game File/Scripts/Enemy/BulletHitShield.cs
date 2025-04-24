using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHitShield : MonoBehaviour
{

    private Boss_Ai boss; // 引用 Boss_Ai 腳本
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
            // 處理護盾傷害
            if (boss != null && boss.IsShieldActive())
            {
                boss.TakeShieldDamage(bulletDamage);
                //Debug.Log("Damaged Shield");
            }
        }
    }
}

using UnityEngine;

public class BulletHitBox : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitEffect; // 擊中敵人或玩家的效果
    [SerializeField] private ParticleSystem shieldHitEffect; // 擊中護盾的效果

    // 層索引（僅用於 whatIsGround）
    private int whatIsGroundLayer;

    private void Awake()
    {
        // 初始化 whatIsGround 層索引
        whatIsGroundLayer = LayerMask.NameToLayer("whatIsGround");

        // 檢查層是否存在
        if (whatIsGroundLayer == -1)
        {
            Debug.LogError("Layer 'whatIsGround' not found!");
        }
    }

    // 處理玩家子彈的物理碰撞（使用 OnCollisionEnter）
    private void OnCollisionEnter(Collision collision)
    {
        // 僅處理玩家子彈
        if (gameObject.CompareTag("PlayerBullets"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                if (hitEffect != null)
                {
                    ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
            else if (collision.gameObject.CompareTag("Shield"))
            {
                if (shieldHitEffect != null)
                {
                    // 使用碰撞點作為粒子效果的位置
                    Vector3 contactPoint = collision.contacts[0].point;
                    ParticleSystem effect = Instantiate(shieldHitEffect, contactPoint, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
            else if (collision.gameObject.CompareTag("Untagged") || collision.gameObject.layer == whatIsGroundLayer)
            {
                ReturnToPool();
            }
        }
    }

    // 處理敵人子彈和狙擊手子彈的觸發器碰撞（使用 OnTriggerEnter）
    private void OnTriggerEnter(Collider other)
    {
        // 處理敵人子彈和狙擊手子彈
        if (gameObject.CompareTag("EnemyBullets") || gameObject.CompareTag("SniperBullets"))
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Untagged"))
            {
                if (hitEffect != null)
                {
                    ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
            else if (other.gameObject.CompareTag("Shield"))
            {
                if (shieldHitEffect != null)
                {
                    // 因為是觸發器碰撞，無法直接獲取碰撞點，使用子彈位置作為近似
                    Vector3 contactPoint = transform.position;
                    ParticleSystem effect = Instantiate(shieldHitEffect, contactPoint, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
            //else if (other.gameObject.layer == whatIsGroundLayer) // 添加對 whatIsGround 層的檢測
            //{
            //    if (hitEffect != null)
            //    {
            //        ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            //        Destroy(effect.gameObject, effect.main.duration);
            //    }
            //    ReturnToPool();
            //}
        }
    }

    private void ReturnToPool()
    {
        //Debug.Log("Bullet Returned to Pool: " + gameObject.tag);
        if (BulletPoolManager.Instance != null)
        {
            if (gameObject.CompareTag("PlayerBullets"))
            {
                BulletPoolManager.Instance.ReturnPlayerBullet(gameObject);
            }
            else if (gameObject.CompareTag("EnemyBullets") || gameObject.CompareTag("SniperBullets"))
            {
                BulletPoolManager.Instance.ReturnEnemyBullet(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
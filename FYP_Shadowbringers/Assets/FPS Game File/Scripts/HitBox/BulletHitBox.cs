using UnityEngine;

public class BulletHitBox : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitEffect; // �����ĤH�Ϊ��a���ĪG
    [SerializeField] private ParticleSystem shieldHitEffect; // �����@�ު��ĪG

    private void OnCollisionEnter(Collision collision)
    {
        // ���a�l�u�����ĤH���@��
        if (gameObject.tag == "PlayerBullets")
        {
            if (collision.gameObject.tag == "Enemy")
            {
                if (hitEffect != null)
                {
                    ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
            else if (collision.gameObject.tag == "Shield")
            {
                if (shieldHitEffect != null)
                {
                    // �ϥθI���I�@���ɤl�ĪG����m
                    Vector3 contactPoint = collision.contacts[0].point;
                    ParticleSystem effect = Instantiate(shieldHitEffect, contactPoint, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
        }
        // �ĤH�l�u�������a�B���аO��H���@��
        else if (gameObject.tag == "EnemyBullets")
        {
            if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Untagged")
            {
                if (hitEffect != null)
                {
                    ParticleSystem effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
            else if (collision.gameObject.tag == "Shield")
            {
                if (shieldHitEffect != null)
                {
                    // �ϥθI���I�@���ɤl�ĪG����m
                    Vector3 contactPoint = collision.contacts[0].point;
                    ParticleSystem effect = Instantiate(shieldHitEffect, contactPoint, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
        }
    }

    private void ReturnToPool()
    {
        Debug.Log("Bullet Returned to Pool: " + gameObject.tag);
        if (BulletPoolManager.Instance != null)
        {
            if (gameObject.tag == "PlayerBullets")
            {
                BulletPoolManager.Instance.ReturnPlayerBullet(gameObject);
            }
            else if (gameObject.tag == "EnemyBullets")
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
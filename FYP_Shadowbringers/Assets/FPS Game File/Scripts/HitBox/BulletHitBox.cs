using UnityEngine;

public class BulletHitBox : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitEffect; // �����ĤH�Ϊ��a���ĪG
    [SerializeField] private ParticleSystem shieldHitEffect; // �����@�ު��ĪG

    // �B�z���a�l�u�����z�I���]�ϥ� OnCollisionEnter�^
    private void OnCollisionEnter(Collision collision)
    {
        // �ȳB�z���a�l�u
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
                    // �ϥθI���I�@���ɤl�ĪG����m
                    Vector3 contactPoint = collision.contacts[0].point;
                    ParticleSystem effect = Instantiate(shieldHitEffect, contactPoint, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
        }
    }

    // �B�z�ĤH�l�u��Ĳ�o���I���]�ϥ� OnTriggerEnter�^
    private void OnTriggerEnter(Collider other)
    {
        // �ȳB�z�ĤH�l�u
        if (gameObject.CompareTag("EnemyBullets"))
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
                    // �]���OĲ�o���I���A�L�k��������I���I�A�ϥΤl�u��m�@�����
                    Vector3 contactPoint = transform.position;
                    ParticleSystem effect = Instantiate(shieldHitEffect, contactPoint, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
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
            else if (gameObject.CompareTag("EnemyBullets"))
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
using UnityEngine;

public class BulletHitBox : MonoBehaviour
{
    [SerializeField] private ParticleSystem hitEffect; // �����ĤH�Ϊ��a���ĪG
    [SerializeField] private ParticleSystem shieldHitEffect; // �����@�ު��ĪG

    // �h���ޡ]�ȥΩ� whatIsGround�^
    private int whatIsGroundLayer;

    private void Awake()
    {
        // ��l�� whatIsGround �h����
        whatIsGroundLayer = LayerMask.NameToLayer("whatIsGround");

        // �ˬd�h�O�_�s�b
        if (whatIsGroundLayer == -1)
        {
            Debug.LogError("Layer 'whatIsGround' not found!");
        }
    }

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
            else if (collision.gameObject.CompareTag("Untagged") || collision.gameObject.layer == whatIsGroundLayer)
            {
                ReturnToPool();
            }
        }
    }

    // �B�z�ĤH�l�u�M������l�u��Ĳ�o���I���]�ϥ� OnTriggerEnter�^
    private void OnTriggerEnter(Collider other)
    {
        // �B�z�ĤH�l�u�M������l�u
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
                    // �]���OĲ�o���I���A�L�k��������I���I�A�ϥΤl�u��m�@�����
                    Vector3 contactPoint = transform.position;
                    ParticleSystem effect = Instantiate(shieldHitEffect, contactPoint, Quaternion.identity);
                    Destroy(effect.gameObject, effect.main.duration);
                }
                ReturnToPool();
            }
            //else if (other.gameObject.layer == whatIsGroundLayer) // �K�[�� whatIsGround �h���˴�
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
using System.Collections;
using UnityEngine;

public class ImpactCircle : MonoBehaviour
{
    [SerializeField] private Transform innerCircle; // �����Ρ]�|��j�^
    [SerializeField] private Transform outerCircle; // �~��]�T�w�j�p�^
    [SerializeField] private GameObject explosionVFX; // ExplosionVFX ����
    [SerializeField] private float initialScale = 0f; // ��l�j�p�]�q���߶}�l�^
    [SerializeField] private float maxScale = 8f; // �̤j�j�p�]���P OuterCircle ���j�p�@�P�^
    [SerializeField] private int beatsToExpand = 4; // ��j�һݪ���l�ƶq�]�i�վ����ס^
    [SerializeField] private float damage = 10f; // �缾�a�y�����ˮ`
    [SerializeField] private Color startColor = new Color(1f, 0f, 0f, 0.3f); // �_�l�C��]�b�z������^
    [SerializeField] private Color endColor = new Color(1f, 0f, 0f, 1f); // �����C��]������^

    private float currentScale;
    private int currentBeatCount = 0; // ��e��l�p��
    private bool isExpanding = false;
    private Material circleMaterial;
    private float outerCircleScale; // �x�s OuterCircle ���j�p
    private float scalePerBeat; // �C�窺�Y��W�q
    private bool hasProcessedBeat = false; // �s�W�ܶq�A�T�O�C�Ӹ`��u�B�z�@��

    private void Awake()
    {
        // �ˬd���n�ե�
        if (innerCircle == null)
        {
            Debug.LogError("ImpactCircle: innerCircle is not set!", this);
            return;
        }
        if (outerCircle == null)
        {
            Debug.LogError("ImpactCircle: outerCircle is not set!", this);
            return;
        }
        if (explosionVFX == null)
        {
            Debug.LogError("ImpactCircle: explosionVFX is not set!", this);
            return;
        }

        // ��l�Ƭ����Ϊ��j�p
        currentScale = initialScale;
        innerCircle.localScale = new Vector3(initialScale, innerCircle.localScale.y, initialScale);

        // ��������Ϊ�����
        Renderer renderer = innerCircle.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("ImpactCircle: innerCircle does not have a Renderer component!", this);
            return;
        }
        circleMaterial = renderer.material;
        if (circleMaterial == null)
        {
            Debug.LogError("ImpactCircle: innerCircle does not have a material!", this);
            return;
        }
        circleMaterial.color = startColor;

        // ��� OuterCircle ���j�p�]���] X �M Z �b�Y��ۦP�^
        outerCircleScale = outerCircle.localScale.x;

        // �p��C�窺�Y��W�q
        scalePerBeat = (maxScale - initialScale) / beatsToExpand;

        // �T�O ExplosionVFX ��l�ɬO�T�Ϊ�
        explosionVFX.SetActive(false);
    }

    public void StartExpanding()
    {
        isExpanding = true;
        currentBeatCount = 0;
    }

    private void Update()
    {
        if (!isExpanding) return;

        // �ˬd BeatManager �O�_�s�b
        if (BeatManager.Instance == null)
        {
            Debug.LogWarning("BeatManager instance is null! Please ensure BeatManager is present in the scene.", this);
            return;
        }

        // ���ݸ`����l
        if (BeatManager.Instance.inBeat)
        {
            if (!hasProcessedBeat) // �T�O�C�Ӹ`��u�B�z�@��
            {
                BeatManager.Instance._CheckBeat(); // Ĳ�o��l�ˬd

                currentBeatCount++;
                Debug.Log($"Beat {currentBeatCount}/{beatsToExpand}, Scale: {currentScale}");
                float t = (float)currentBeatCount / beatsToExpand;
                currentScale = Mathf.Lerp(initialScale, maxScale, t);
                innerCircle.localScale = new Vector3(currentScale, innerCircle.localScale.y, currentScale);

                circleMaterial.color = Color.Lerp(startColor, endColor, t);

                if (currentScale >= outerCircleScale)
                {
                    Explode();
                    StartCoroutine(DelayedDestroy(2f));
                }

                hasProcessedBeat = true; // �аO�w�B�z��e�`��
            }
        }
        else
        {
            hasProcessedBeat = false; // ���m�аO�A���ݤU�@�Ӹ`��
        }
    }

    private void Explode()
    {
        explosionVFX.SetActive(true);

        float explosionRadius = (maxScale / 2f) * 1.5f;
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
                else
                {
                    Debug.Log("���a�b�z���d�򤺡A���ʤ� PlayerHealth �ե�I");
                }
            }
        }

        isExpanding = false;
    }

    private IEnumerator DelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = new Vector3(position.x, position.y + 0.05f, position.z);
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    private void OnDestroy()
    {
        if (circleMaterial != null)
        {
            Destroy(circleMaterial);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxScale / 2f);
    }

    public void SetBeatsToExpand(int beats)
    {
        beatsToExpand = beats;
        scalePerBeat = (maxScale - initialScale) / beatsToExpand;
    }
}
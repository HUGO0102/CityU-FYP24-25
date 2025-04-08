using System.Collections;
using UnityEngine;

public class ImpactCircle : MonoBehaviour
{
    [SerializeField] private Transform innerCircle; // 紅色圓形（會放大）
    [SerializeField] private Transform outerCircle; // 外圈（固定大小）
    [SerializeField] private GameObject explosionVFX; // ExplosionVFX 物件
    [SerializeField] private float initialScale = 0f; // 初始大小（從中心開始）
    [SerializeField] private float maxScale = 8f; // 最大大小（應與 OuterCircle 的大小一致）
    [SerializeField] private int beatsToExpand = 4; // 放大所需的拍子數量（可調整難度）
    [SerializeField] private float damage = 10f; // 對玩家造成的傷害
    [SerializeField] private Color startColor = new Color(1f, 0f, 0f, 0.3f); // 起始顏色（半透明紅色）
    [SerializeField] private Color endColor = new Color(1f, 0f, 0f, 1f); // 結束顏色（實色紅色）

    private float currentScale;
    private int currentBeatCount = 0; // 當前拍子計數
    private bool isExpanding = false;
    private Material circleMaterial;
    private float outerCircleScale; // 儲存 OuterCircle 的大小
    private float scalePerBeat; // 每拍的縮放增量
    private bool hasProcessedBeat = false; // 新增變量，確保每個節拍只處理一次

    private void Awake()
    {
        // 檢查必要組件
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

        // 初始化紅色圓形的大小
        currentScale = initialScale;
        innerCircle.localScale = new Vector3(initialScale, innerCircle.localScale.y, initialScale);

        // 獲取紅色圓形的材質
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

        // 獲取 OuterCircle 的大小（假設 X 和 Z 軸縮放相同）
        outerCircleScale = outerCircle.localScale.x;

        // 計算每拍的縮放增量
        scalePerBeat = (maxScale - initialScale) / beatsToExpand;

        // 確保 ExplosionVFX 初始時是禁用的
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

        // 檢查 BeatManager 是否存在
        if (BeatManager.Instance == null)
        {
            Debug.LogWarning("BeatManager instance is null! Please ensure BeatManager is present in the scene.", this);
            return;
        }

        // 等待節奏拍子
        if (BeatManager.Instance.inBeat)
        {
            if (!hasProcessedBeat) // 確保每個節拍只處理一次
            {
                BeatManager.Instance._CheckBeat(); // 觸發拍子檢查

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

                hasProcessedBeat = true; // 標記已處理當前節拍
            }
        }
        else
        {
            hasProcessedBeat = false; // 重置標記，等待下一個節拍
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
                    Debug.Log("玩家在爆炸範圍內，但缺少 PlayerHealth 組件！");
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
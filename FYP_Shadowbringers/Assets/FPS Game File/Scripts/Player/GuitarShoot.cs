using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Nokobot/Instruments/Guitar Shoot")]
public class GuitarShoot : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject bulletPrefab; // 子彈預製體
    public GameObject vfxPrefab; // Barrel_Location 的 VFX Prefab
    public GameObject vfxBurstPrefab; // VFXBurst_Point 的 VFX Prefab

    [Header("Location References")]
    [SerializeField] private Transform barrelLocation; // 普通 VFX 起始位置
    [SerializeField] private Transform vfxBurstPoint; // HitOnBeat 時的額外 VFX 位置

    [Header("Settings")]
    [Tooltip("Bullet Speed")][SerializeField] private float shotPower = 500f;
    [Tooltip("VFX Duration (seconds)")][SerializeField] private float vfxDuration = 1f; // VFX 持續時間
    [SerializeField] private int vfxPoolSize = 10; // 普通 VFX 池大小
    [SerializeField] private int vfxBurstPoolSize = 10; // Burst VFX 池大小
    [Tooltip("Shots per second")][SerializeField] private float fireRate = 5f; // 射擊頻率（每秒射擊次數）

    [Header("Sound")]
    public AudioSource source;
    public AudioClip[] fireSounds; // 修改為陣列，包含 7 個射擊音效

    [SerializeField] private Animator playerAnimator; // 玩家的 Animator 組件
    private GameObject player; // 引用玩家遊戲對象

    private List<GameObject> vfxPool = new List<GameObject>(); // 普通 VFX 物件池
    private List<GameObject> vfxBurstPool = new List<GameObject>(); // Burst VFX 物件池
    private float nextFireTime; // 下一次允許射擊的時間

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        // 查找玩家並獲取其 Animator
        player = GameObject.Find("Player");
        if (player != null && playerAnimator == null)
        {
            playerAnimator = player.GetComponent<Animator>();
            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator not found on Player GameObject!");
            }
        }

        // 初始化兩個 VFX 物件池
        InitializeVFXPool();
        InitializeVFXBurstPool();
        nextFireTime = 0f; // 初始化下一次射擊時間

        // 檢查音效陣列
        if (fireSounds == null || fireSounds.Length == 0)
        {
            Debug.LogWarning("Fire Sounds array is empty or not assigned!");
        }
    }

    private void InitializeVFXPool()
    {
        if (vfxPrefab != null)
        {
            for (int i = 0; i < vfxPoolSize; i++)
            {
                GameObject vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
                vfx.SetActive(false);
                vfxPool.Add(vfx);
            }
        }
    }

    private void InitializeVFXBurstPool()
    {
        if (vfxBurstPrefab != null)
        {
            for (int i = 0; i < vfxBurstPoolSize; i++)
            {
                GameObject vfx = Instantiate(vfxBurstPrefab, transform.position, Quaternion.identity);
                vfx.SetActive(false);
                vfxBurstPool.Add(vfx);
            }
        }
    }

    private GameObject GetVFXFromPool()
    {
        foreach (GameObject vfx in vfxPool)
        {
            if (vfx != null && !vfx.activeInHierarchy)
            {
                vfx.SetActive(true);
                return vfx;
            }
        }

        if (vfxPrefab != null)
        {
            GameObject newVfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
            vfxPool.Add(newVfx);
            return newVfx;
        }
        return null;
    }

    private GameObject GetVFXBurstFromPool()
    {
        foreach (GameObject vfx in vfxBurstPool)
        {
            if (vfx != null && !vfx.activeInHierarchy)
            {
                vfx.SetActive(true);
                return vfx;
            }
        }

        if (vfxBurstPrefab != null)
        {
            GameObject newVfx = Instantiate(vfxBurstPrefab, transform.position, Quaternion.identity);
            vfxBurstPool.Add(newVfx);
            return newVfx;
        }
        return null;
    }

    private void ReturnVFXToPool(GameObject vfx)
    {
        if (vfx != null)
        {
            vfx.SetActive(false);
        }
    }

    private void ReturnVFXBurstToPool(GameObject vfx)
    {
        if (vfx != null)
        {
            vfx.SetActive(false);
        }
    }

    void Update()
    {
        // 無需額外輸入處理，交由 PlayerShoot 控制
    }

    public void Fire()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Attack", true);
            StartCoroutine(ResetAttackBool());
        }
    }

    // 執行射擊邏輯，接受 HitOnBeat 條件
    public void Shoot(bool hitOnBeat = false)
    {
        // 檢查射擊頻率
        if (Time.time < nextFireTime)
        {
            return; // 如果未到下一次射擊時間，退出
        }

        Debug.Log($"Shoot called with hitOnBeat: {hitOnBeat}");

        // 播放隨機射擊音效
        if (source != null && fireSounds != null && fireSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, fireSounds.Length); // 隨機選擇一個音效
            AudioClip selectedSound = fireSounds[randomIndex];
            source.PlayOneShot(selectedSound);
            Debug.Log($"Playing fire sound: {selectedSound.name}");
        }
        else
        {
            Debug.LogWarning("AudioSource or Fire Sounds array is not properly set!");
        }

        // 從池中獲取 VFX 並從 Barrel_Location 播放
        GameObject vfx = GetVFXFromPool();
        if (vfx != null)
        {
            vfx.transform.position = barrelLocation.position;
            vfx.transform.rotation = barrelLocation.rotation;
            StartCoroutine(ReturnVFXToPoolAfterDuration(vfx));
        }
        else
        {
            Debug.LogWarning("No available VFX in pool for Barrel_Location!");
        }

        // 如果 HitOnBeat，額外從 VFXBurst_Point 播放 VFX
        if (hitOnBeat && vfxBurstPoint != null)
        {
            Debug.Log("HitOnBeat triggered, playing VFXBurst_Point");
            GameObject burstVfx = GetVFXBurstFromPool();
            if (burstVfx != null)
            {
                burstVfx.transform.position = vfxBurstPoint.position;
                burstVfx.transform.rotation = vfxBurstPoint.rotation;
                StartCoroutine(ReturnVFXBurstToPoolAfterDuration(burstVfx));
            }
            else
            {
                Debug.LogWarning("No available VFX in pool for VFXBurst_Point!");
            }
        }

        // 從 BulletPoolManager 獲取子彈
        GameObject bullet = BulletPoolManager.Instance.GetPlayerBullet();
        if (bullet != null)
        {
            bullet.SetActive(true);
            bullet.transform.position = barrelLocation.position;

            // 計算準心位置並設置子彈方向與旋轉
            Vector3 aimPoint = GetAimPoint();
            if (aimPoint != Vector3.zero)
            {
                Vector3 shootDirection = (aimPoint - barrelLocation.position).normalized;
                // 設置子彈的 Rotation 朝向瞄準點
                bullet.transform.rotation = Quaternion.LookRotation(shootDirection);
                // 施加力沿著方向發射
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero; // 重置線速度
                    rb.angularVelocity = Vector3.zero; // 重置角速度，防止自轉
                    rb.AddForce(shootDirection * shotPower); // 根據準心方向施加力
                }
            }
            else
            {
                Debug.LogWarning("Failed to get aim point, using forward direction as fallback.");
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    bullet.transform.rotation = Quaternion.LookRotation(barrelLocation.forward); // 備用旋轉
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.AddForce(barrelLocation.forward * shotPower); // 備用：沿前方向射擊
                }
            }
        }

        // 更新下一次射擊時間
        nextFireTime = Time.time + (1f / fireRate);
    }

    // 獲取玩家準心位置
    private Vector3 GetAimPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0)); // 螢幕中心
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log($"Aim point hit at: {hit.point}"); // 調試日誌
            return hit.point;
        }
        else
        {
            // 如果未擊中任何東西，返回一個預設遠處點作為備用
            return Camera.main.transform.position + ray.direction * 100f;
        }
    }

    private IEnumerator ReturnVFXToPoolAfterDuration(GameObject vfx)
    {
        if (vfx != null)
        {
            yield return new WaitForSeconds(vfxDuration);
            if (vfx != null)
            {
                ReturnVFXToPool(vfx);
            }
        }
    }

    private IEnumerator ReturnVFXBurstToPoolAfterDuration(GameObject vfx)
    {
        if (vfx != null)
        {
            yield return new WaitForSeconds(vfxDuration);
            if (vfx != null)
            {
                ReturnVFXBurstToPool(vfx);
            }
        }
    }

    private IEnumerator ResetAttackBool()
    {
        yield return new WaitForSeconds(0.5f);
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Attack", false);
        }
    }

    private void OnDestroy()
    {
        vfxPool.RemoveAll(vfx => vfx == null);
        vfxBurstPool.RemoveAll(vfx => vfx == null);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[AddComponentMenu("Nokobot/Instruments/Guitar Shoot")]
public class GuitarShoot : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject bulletPrefab; // ���q�l�u�w�s��
    public GameObject burstBulletPrefab; // HitOnBeat �l�u�w�s��
    public GameObject vfxPrefab; // Barrel_Location �� VFX Prefab
    public GameObject vfxBurstPrefab; // VFXBurst_Point �� VFX Prefab

    [Header("Location References")]
    [SerializeField] private Transform barrelLocation; // ���q VFX �_�l��m
    [SerializeField] private Transform vfxBurstPoint; // HitOnBeat �ɪ��B�~ VFX ��m

    [Header("Settings")]
    [Tooltip("Bullet Speed")][SerializeField] private float shotPower = 500f;
    [Tooltip("VFX Duration (seconds)")][SerializeField] private float vfxDuration = 1f; // VFX ����ɶ�
    [SerializeField] private int vfxPoolSize = 10; // ���q VFX ���j�p
    [SerializeField] private int vfxBurstPoolSize = 10; // Burst VFX ���j�p
    [Tooltip("Shots per second")][SerializeField] private float fireRate = 5f; // �g���W�v�]�C��g�����ơ^

    [Header("Sound")]
    public AudioSource source;
    public AudioClip[] fireSounds; // ���q�g�����İ}�C
    public AudioClip[] hitOnBeatSounds; // HitOnBeat �ɪ��S�O���İ}�C

    [SerializeField] private Animator playerAnimator; // ���a�� Animator �ե�
    private GameObject player; // �ޥΪ��a�C����H

    private List<GameObject> vfxPool = new List<GameObject>(); // ���q VFX �����
    private List<GameObject> vfxBurstPool = new List<GameObject>(); // Burst VFX �����
    private float nextFireTime; // �U�@�����\�g�����ɶ�

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;

        // �d�䪱�a������� Animator
        player = GameObject.Find("Player");
        if (player != null && playerAnimator == null)
        {
            playerAnimator = player.GetComponent<Animator>();
            if (playerAnimator == null)
            {
                Debug.LogError("Player Animator not found on Player GameObject!");
            }
        }

        // ��l�ƨ�� VFX �����
        InitializeVFXPool();
        InitializeVFXBurstPool();
        nextFireTime = 0f; // ��l�ƤU�@���g���ɶ�

        // �ˬd���İ}�C
        if (fireSounds == null || fireSounds.Length == 0)
        {
            Debug.LogWarning("Fire Sounds array is empty or not assigned!");
        }
        if (hitOnBeatSounds == null || hitOnBeatSounds.Length == 0)
        {
            Debug.LogWarning("Hit On Beat Sounds array is empty or not assigned!");
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
        // �L���B�~��J�B�z�A��� PlayerShoot ����
    }

    public void Fire()
    {
        if (Time.timeScale == 0f) // �ˬd�C���O�_�Ȱ�
        {
            Debug.Log("Game is paused, cannot fire!");
            return; // �p�G�C���Ȱ��A����g��
        }

        if (playerAnimator != null)
        {
            playerAnimator.SetBool("Attack", true);
            StartCoroutine(ResetAttackBool());
        }
    }

    public void Shoot(bool hitOnBeat = false)
    {
        if (Time.timeScale == 0f)
        {
            Debug.Log("Game is paused, cannot shoot!");
            return;
        }

        if (Time.time < nextFireTime)
        {
            return;
        }

        Debug.Log($"Shoot called with hitOnBeat: {hitOnBeat}");

        AudioClip[] selectedSounds = hitOnBeat ? hitOnBeatSounds : fireSounds;
        if (source != null && selectedSounds != null && selectedSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, selectedSounds.Length);
            AudioClip selectedSound = selectedSounds[randomIndex];
            source.PlayOneShot(selectedSound);
            Debug.Log($"Playing {(hitOnBeat ? "HitOnBeat" : "Normal")} fire sound: {selectedSound.name}");
        }
        else
        {
            Debug.LogWarning($"AudioSource or {(hitOnBeat ? "HitOnBeatSounds" : "FireSounds")} array is not properly set!");
        }

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

        // �q BulletPoolManager ����l�u�A�öǻ� hitOnBeat
        GameObject bullet = BulletPoolManager.Instance.GetPlayerBullet(hitOnBeat);
        if (bullet != null)
        {
            bullet.SetActive(true);
            bullet.transform.position = barrelLocation.position;

            Vector3 aimPoint = GetAimPoint();
            if (aimPoint != Vector3.zero)
            {
                Vector3 shootDirection = (aimPoint - barrelLocation.position).normalized;
                bullet.transform.rotation = Quaternion.LookRotation(shootDirection);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.AddForce(shootDirection * shotPower);
                }
            }
            else
            {
                Debug.LogWarning("Failed to get aim point, using forward direction as fallback.");
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    bullet.transform.rotation = Quaternion.LookRotation(barrelLocation.forward);
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.AddForce(barrelLocation.forward * shotPower);
                }
            }

            Debug.Log($"Bullet fired with tag: {bullet.tag}");
        }

        nextFireTime = Time.time + (1f / fireRate);
    }

    private Vector3 GetAimPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        else
        {
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
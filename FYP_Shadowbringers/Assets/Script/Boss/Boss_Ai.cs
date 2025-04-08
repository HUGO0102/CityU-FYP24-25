using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Boss_Ai : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;
    public GameObject TargetLooker;
    private MeshRenderer meshRenderer;
    public Animator animator;

    // Animation bool
    bool isIdle;
    bool isWalking;
    bool isAttack;
    bool isHit;
    bool dead;

    public GameObject DestroyObj;

    // Stats
    public int health;
    [SerializeField] public bool lowHealth;

    // Check for Ground/Obstacles
    public LayerMask whatIsGround, whatIsPlayer;

    // Patroling
    public Vector3 walkPoint;
    public bool walkPointSet;
    public float walkPointRange;

    private TargetLooker targetLooker;
    private Vector3 targetPostition;


    [Header("States")]
    public bool isDead;
    public bool hited;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    [Header("MiniGun Attack")]
    // Attack Player
    public int timeBetweenAttacks = 2;
    public bool alreadyAttacked;
    public int minigun_Fire_Density = 10;
    private int currentFire_Density = 0;
    public float minigun_Fire_Gap = 0.1f;
    private int Attack_SkillsNumber = 0;
    private int RanNum = 0;


    // VFX
    [Header("OnHit VFX")]
    [SerializeField] public ParticleSystem spark;
    [SerializeField] public ParticleSystem onHitVFX;

    [Header("MiniGun VFX")]
    bool vfxIsCreated = false;
    [SerializeField] public GameObject shootingVFXPrefab; // 新的 VFX 預製體，包含 Muzzle Flash（Visual Effect）
    private GameObject rightVFXInstance; // 右邊槍口的 VFX 實例
    private GameObject leftVFXInstance;  // 左邊槍口的 VFX 實例
    private Queue<GameObject> vfxPool = new Queue<GameObject>(); // VFX 對象池
    [SerializeField] private int vfxPoolSize = 4; // 對象池大小

    // Barrel
    [Header("Location Refrences")]
    [SerializeField] private Transform rBarrelLocation;
    [SerializeField] private Transform lBarrelLocation;
    [Header("Barrel Spinners")]
    [SerializeField] private BarrelSpinner rightBarrelSpinner;
    [SerializeField] private BarrelSpinner leftBarrelSpinner;
    [Header("Barrel Audio Sources")]
    [SerializeField] private AudioSource rightGunAudioSource; // 右槍口的 AudioSource
    [SerializeField] private AudioSource leftGunAudioSource;  // 左槍口的 AudioSource
    [Header("Barrel Lights")]
    [SerializeField] private GameObject rightBarrelLight; // 右槍口的 PointLight GameObject
    [SerializeField] private GameObject leftBarrelLight;  // 左槍口的 PointLight GameObject

    [Header("Settings")]
    [Tooltip("Specify time to destroy the VFX object")][SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")][SerializeField] private float shotPower = 500f;
    [Tooltip("Point Light Intensity")][SerializeField] private float maxLightIntensity = 2f;
    [Tooltip("Point Light Fade Duration")][SerializeField] private float lightFadeDuration = 0.5f;


    [Header("Missile Attack")]
    [SerializeField] private GameObject impactCirclePrefab; // 著彈點預製體
    [SerializeField] private int missileCount = 3; // 一次攻擊生成多少個著彈點
    [SerializeField] private float missileLaunchDelay = 0.5f; // 每個著彈點之間的延遲


    // SFX
    [Header("SoundFX")]
    public AudioSource enemyAudioSource;
    public AudioClip fireSound; // MinigunFireLoop
    public AudioClip[] enemyAudioClip;
    private float volumeChangeMultiplier = 0.2f;
    private float pitchChangeMultiplier = 0.2f;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        if (meshRenderer == null)
            meshRenderer = GetComponentInChildren<MeshRenderer>();

        if (targetLooker == null)
            targetLooker = GetComponentInChildren<TargetLooker>();
    }

    private void Start()
    {
        // 初始化 VFX 對象池
        for (int i = 0; i < vfxPoolSize; i++)
        {
            GameObject vfx = Instantiate(shootingVFXPrefab);
            vfx.SetActive(false);
            vfxPool.Enqueue(vfx);
        }
    }

    private void Update()
    {
        SwitchAnimation();

        // 根據血量調整攻擊參數
        if (health <= (health / 2))
        {
            lowHealth = true;
            currentFire_Density = minigun_Fire_Density * 2; // 增加射擊密度
            minigun_Fire_Gap = 0.08f; // 減少射擊間隔
        }
        else
        {
            lowHealth = false;
            currentFire_Density = minigun_Fire_Density;
            minigun_Fire_Gap = 0.1f;
        }

        if (!isDead)
        {
            // Check if Player in sightrange
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);

            // Check if Player in attackrange
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

            targetPostition = new Vector3(player.position.x, this.transform.position.y, player.position.z);

            if (!playerInSightRange && !playerInAttackRange)
            {
                isIdle = true;
                isWalking = false;
                Idleing();
            }
            if (playerInSightRange && !playerInAttackRange)
            {
                isIdle = false;
                isWalking = true;
                ChasePlayer();
            }
            if (playerInAttackRange && playerInSightRange)
            {
                isIdle = true;
                isWalking = false;
                AttackPlayer();
            }
        }
    }

    void SwitchAnimation()
    {
        animator.SetBool("Idle", isIdle);
        animator.SetBool("Walking", isWalking);
        animator.SetBool("Attack", isAttack);
        animator.SetBool("Hit", isHit);
    }

    private void Idleing()
    {
        if (isDead) return;

        agent.speed = 2f;

        if (targetLooker != null)
            TargetLooker.GetComponent<TargetLooker>().targetTrans = null;

        agent.SetDestination(transform.position);

        // Calculates DistanceToWalkPoint
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        // Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        if (targetLooker != null)
            TargetLooker.GetComponent<TargetLooker>().targetTrans = null;

        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        if (isDead) return;

        agent.speed = 5f;

        if (targetLooker != null)
            TargetLooker.GetComponent<TargetLooker>().targetTrans = player;

        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        if (isDead) return;

        agent.speed = 2f;

        if (targetLooker != null)
            TargetLooker.GetComponent<TargetLooker>().targetTrans = player;

        // 使槍口朝向玩家
        rBarrelLocation.LookAt(player.position);
        lBarrelLocation.LookAt(player.position);

        // 確保敵人不移動
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            while (Attack_SkillsNumber == RanNum)
            {
                Attack_SkillsNumber = Random.Range(0, 4); // 增加到 3 種模式
            }
            RanNum = Attack_SkillsNumber;

            switch (Attack_SkillsNumber)
            {
                case 3:
                    MissileAttack(); // 導彈攻擊技能
                    print("Missile Attack");
                    break;
                case 2:
                    GunAttackBoth(); // 同時使用左右槍口
                    print("Both Guns Attack");
                    break;
                case 1:
                    GunAttackRight(); // 僅使用右槍口
                    print("Right Gun Attack");
                    break;
                case 0:
                    GunAttackLeft(); // 僅使用左槍口
                    print("Left Gun Attack");
                    break;
                default:
                    break;
            }
            alreadyAttacked = true;
        }
    }

    private void GunAttackRight()
    {
        isAttack = true;
        StartCoroutine(FireBurst(rBarrelLocation));
    }

    private void GunAttackLeft()
    {
        isAttack = true;
        StartCoroutine(FireBurst(lBarrelLocation));
    }

    private void GunAttackBoth()
    {
        isAttack = true;
        StartCoroutine(FireBurstBoth());
    }

    private IEnumerator FireBurst(Transform barrel)
    {
        int i = 0;
        BarrelSpinner selectedSpinner = (barrel == rBarrelLocation) ? rightBarrelSpinner : leftBarrelSpinner;
        AudioSource selectedAudioSource = (barrel == rBarrelLocation) ? rightGunAudioSource : leftGunAudioSource;
        GameObject selectedLight = (barrel == rBarrelLocation) ? rightBarrelLight : leftBarrelLight;
        GameObject selectedVFXInstance = (barrel == rBarrelLocation) ? rightVFXInstance : leftVFXInstance;
        bool isRightBarrel = (barrel == rBarrelLocation);

        // 開始旋轉
        selectedAudioSource.volume = 1f;
        selectedSpinner.StartSpinning();

        // 等待 Spin up 完成
        yield return StartCoroutine(selectedSpinner.WaitForSpinUp());

        isAttack = true;

        // 從對象池中獲取 VFX
        selectedVFXInstance = GetVFXFromPool(barrel, isRightBarrel);
        if (isRightBarrel)
        {
            rightVFXInstance = selectedVFXInstance;
        }
        else
        {
            leftVFXInstance = selectedVFXInstance;
        }

        // 設置射擊音效為循環並播放
        selectedAudioSource.clip = fireSound;
        selectedAudioSource.loop = true;
        selectedAudioSource.pitch = lowHealth ? 1.1f : 1f;
        selectedAudioSource.Play();

        // 啟用 PointLight GameObject 並淡入
        Light[] lights = null;
        if (selectedLight != null)
        {
            selectedLight.SetActive(true);
            lights = selectedLight.GetComponentsInChildren<Light>();
            foreach (var light in lights)
            {
                light.intensity = 0f;
            }

            StartCoroutine(FadeInLight(lights, lightFadeDuration));
        }

        while (i <= currentFire_Density)
        {
            Shoot(barrel);
            if (lights != null)
            {
                foreach (var light in lights)
                {
                    light.intensity = maxLightIntensity * Random.Range(0.8f, 1f);
                    float t = Random.Range(0f, 1f);
                    light.color = Color.Lerp(new Color(1f, 0.5f, 0f), new Color(1f, 1f, 0f), t);
                }
            }
            i++;
            yield return new WaitForSeconds(minigun_Fire_Gap);
        }


        // 直接停止射擊音效
        if (selectedAudioSource != null)
        {
            selectedAudioSource.Stop();
            selectedAudioSource.volume = 1f;
            //Debug.Log("FireBurst: Stopped AudioSource for barrel: " + (isRightBarrel ? "Right" : "Left"));
        }

        // 停止旋轉並播放 Wind Down Sound
        selectedSpinner.StopSpinning();

        // 淡出 PointLight
        if (lights != null)
        {
            StartCoroutine(FadeOutLight(lights, lightFadeDuration, selectedLight));
        }

        // 將 VFX 放回對象池
        if (selectedVFXInstance != null)
        {
            ReturnVFXToPool(selectedVFXInstance);
            if (isRightBarrel)
            {
                rightVFXInstance = null;
            }
            else
            {
                leftVFXInstance = null;
            }
        }

        isAttack = false;
        Invoke("ResetAttack", timeBetweenAttacks);
    }

    private IEnumerator FadeInLight(Light[] lights, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float intensity = Mathf.Lerp(0f, maxLightIntensity, elapsedTime / duration);
            foreach (var light in lights)
            {
                light.intensity = intensity;
            }
            yield return null;
        }
        foreach (var light in lights)
        {
            light.intensity = maxLightIntensity;
        }
    }

    private IEnumerator FadeOutLight(Light[] lights, float duration, GameObject lightObject)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float intensity = Mathf.Lerp(maxLightIntensity, 0f, elapsedTime / duration);
            foreach (var light in lights)
            {
                light.intensity = intensity;
            }
            yield return null;
        }
        foreach (var light in lights)
        {
            light.intensity = 0f;
        }
        lightObject.SetActive(false);
    }

    private IEnumerator FadeOutAudio(AudioSource audioSource, float fadeDuration)
    {
        float startVolume = audioSource.volume;
        float elapsedFadeTime = 0f;
        while (elapsedFadeTime < fadeDuration)
        {
            elapsedFadeTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedFadeTime / fadeDuration);
            yield return null;
        }
        audioSource.Stop();
        audioSource.volume = startVolume; // 恢復音量
    }


    private IEnumerator FireBurstBoth()
    {
        int i = 0;
        bool useRightBarrel = true;

        // 同時開始左右槍口的 Spin up
        rightGunAudioSource.volume = 0.5f;
        leftGunAudioSource.volume = 0.5f;
        rightBarrelSpinner.StartSpinning();
        leftBarrelSpinner.StartSpinning();

        // 等待兩邊都完成 Spin up
        yield return StartCoroutine(rightBarrelSpinner.WaitForSpinUp());
        yield return StartCoroutine(leftBarrelSpinner.WaitForSpinUp());

        isAttack = true;

        // 從對象池中獲取左右槍口的 VFX
        rightVFXInstance = GetVFXFromPool(rBarrelLocation, true);
        leftVFXInstance = GetVFXFromPool(lBarrelLocation, false);

        // 設置左右槍口的射擊音效為循環並播放
        rightGunAudioSource.clip = fireSound;
        rightGunAudioSource.loop = true;
        rightGunAudioSource.pitch = lowHealth ? 1.1f : 1f; // 低血量時提高音高
        rightGunAudioSource.volume = 0.5f;
        rightGunAudioSource.Play();

        leftGunAudioSource.clip = fireSound;
        leftGunAudioSource.loop = true;
        leftGunAudioSource.pitch = lowHealth ? 1.1f : 1f; // 低血量時提高音高
        leftGunAudioSource.volume = 0.5f;
        leftGunAudioSource.Play();

        // 啟用左右槍口的 PointLight GameObject 並淡入
        Light[] rightLights = null;
        Light[] leftLights = null;
        if (rightBarrelLight != null)
        {
            rightBarrelLight.SetActive(true);
            rightLights = rightBarrelLight.GetComponentsInChildren<Light>();
            foreach (var light in rightLights)
            {
                light.intensity = 0f;
            }
        }
        if (leftBarrelLight != null)
        {
            leftBarrelLight.SetActive(true);
            leftLights = leftBarrelLight.GetComponentsInChildren<Light>();
            foreach (var light in leftLights)
            {
                light.intensity = 0f;
            }
        }

        // 淡入效果
        if (rightLights != null)
        {
            StartCoroutine(FadeInLight(rightLights, lightFadeDuration));
        }
        if (leftLights != null)
        {
            StartCoroutine(FadeInLight(leftLights, lightFadeDuration));
        }

        // 開始交替射擊
        while (i <= currentFire_Density)
        {
            Transform selectedBarrel = useRightBarrel ? rBarrelLocation : lBarrelLocation;
            Light[] selectedLights = useRightBarrel ? rightLights : leftLights;
            Shoot(selectedBarrel);
            // 更新 PointLight 亮度和顏色，模擬閃爍效果
            if (selectedLights != null)
            {
                foreach (var light in selectedLights)
                {
                    light.intensity = maxLightIntensity * Random.Range(0.8f, 1f); // 隨機變化亮度
                    float t = Random.Range(0f, 1f);
                    light.color = Color.Lerp(new Color(1f, 0.5f, 0f), new Color(1f, 1f, 0f), t);
                }
            }
            i++;
            useRightBarrel = !useRightBarrel; // 交替使用左右槍口
            yield return new WaitForSeconds(minigun_Fire_Gap);
        }


        // 直接停止射擊音效，不淡出
        if (rightGunAudioSource != null)
        {
            rightGunAudioSource.Stop();
            rightGunAudioSource.volume = 0.5f;
        }
        if (leftGunAudioSource != null)
        {
            leftGunAudioSource.Stop();
            leftGunAudioSource.volume = 0.5f;
        }

        // 停止旋轉並播放 Wind Down Sound
        rightBarrelSpinner.StopSpinning();
        leftBarrelSpinner.StopSpinning();


        // 淡出 PointLight
        if (rightLights != null)
        {
            StartCoroutine(FadeOutLight(rightLights, lightFadeDuration, rightBarrelLight));
        }
        if (leftLights != null)
        {
            StartCoroutine(FadeOutLight(leftLights, lightFadeDuration, leftBarrelLight));
        }

        // 將左右槍口的 VFX 放回對象池
        if (rightVFXInstance != null)
        {
            ReturnVFXToPool(rightVFXInstance);
            rightVFXInstance = null;
        }
        if (leftVFXInstance != null)
        {
            ReturnVFXToPool(leftVFXInstance);
            leftVFXInstance = null;
        }

        isAttack = false;
        Invoke("ResetAttack", timeBetweenAttacks + 2);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    //=================================================================================================================================================================================

    private void MissileAttack()
    {
        isAttack = true;
        StartCoroutine(LaunchMissiles());
    }

    private IEnumerator LaunchMissiles()
    {
        isAttack = true;

        for (int i = 0; i < missileCount; i++)
        {
            Vector3 targetPosition = player.position;

            CapsuleCollider playerCollider = player.GetComponent<CapsuleCollider>();
            if (playerCollider != null)
            {
                float playerHeight = playerCollider.height;
                Vector3 feetPosition = targetPosition - new Vector3(0f, playerHeight / 2f, 0f);

                RaycastHit hit;
                if (Physics.Raycast(feetPosition + Vector3.up * 0.5f, Vector3.down, out hit, 1f, whatIsGround))
                {
                    targetPosition = hit.point;
                }
            }
            else
            {
                targetPosition -= new Vector3(0f, 1f, 0f);
                RaycastHit hit;
                if (Physics.Raycast(targetPosition + Vector3.up * 1f, Vector3.down, out hit, 2f, whatIsGround))
                {
                    targetPosition = hit.point;
                }
            }

            // 生成著彈點
            GameObject impactCircle = Instantiate(impactCirclePrefab, targetPosition, Quaternion.identity);
            ImpactCircle circleScript = impactCircle.GetComponent<ImpactCircle>();
            if (circleScript != null)
            {
                // 根據 Boss 血量動態調整拍子數量
                int beatsToExpand = health > (health / 2) ? 8 : 4; // 血量高於 50% 時 8 拍，低於 50% 時 4 拍
                circleScript.SetBeatsToExpand(beatsToExpand); // 動態設置拍子數量
                circleScript.SetPosition(targetPosition);
                circleScript.StartExpanding();
            }

            yield return new WaitForSeconds(missileLaunchDelay);
        }

        isAttack = false;
        Invoke("ResetAttack", timeBetweenAttacks + 5);
    }


    //=================================================================================================================================================================================

    private GameObject GetVFXFromPool(Transform barrel, bool isRightBarrel)
    {
        //Debug.Log("Getting VFX from pool. Pool count: " + vfxPool.Count);

        GameObject vfx;
        if (vfxPool.Count > 0)
        {
            vfx = vfxPool.Dequeue();
            vfx.SetActive(true);
        }
        else
        {
            Debug.LogWarning("VFX pool is empty! Instantiating new VFX.");
            vfx = Instantiate(shootingVFXPrefab);
        }

        // 設置位置和旋轉
        vfx.transform.SetParent(barrel);
        vfx.transform.localPosition = Vector3.zero;
        vfx.transform.localRotation = Quaternion.identity;

        // 翻轉左邊槍口的 VFX
        if (!isRightBarrel)
        {
            Vector3 localScale = vfx.transform.localScale;
            vfx.transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
        }

        // 控制 Visual Effect 的播放
        var visualEffect = vfx.GetComponent<VisualEffect>();
        if (visualEffect != null)
        {
            // 重置 Visual Effect
            visualEffect.Stop();
            visualEffect.Reinit();
            //Debug.Log("Visual Effect initialized for barrel: " + (isRightBarrel ? "Right" : "Left"));

            // 設置播放參數（例如 Spawn Rate 或 Delay）
            if (visualEffect.HasFloat("fire_delay"))
            {
                visualEffect.SetFloat("fire_delay", minigun_Fire_Gap);
                //Debug.Log("Set fire_delay to: " + fire_Gap);
            }
            else
            {
                Debug.LogWarning("fire_delay parameter not found in Visual Effect!");
            }

            // 設置粒子生成頻率（如果 VFX Graph 有此參數）
            if (visualEffect.HasFloat("spawnRate"))
            {
                visualEffect.SetFloat("spawnRate", 1f / minigun_Fire_Gap);
                //Debug.Log("Set spawnRate to: " + (1f / fire_Gap));
            }

            // 播放 Visual Effect
            visualEffect.Play();
            //Debug.Log("Visual Effect played for barrel: " + (isRightBarrel ? "Right" : "Left"));
        }
        else
        {
            Debug.LogError("VisualEffect component not found on VFX object!");
        }

        return vfx;
    }

    private void ReturnVFXToPool(GameObject vfx)
    {
        //Debug.Log("Returning VFX to pool.");

        // 停止 Visual Effect
        var visualEffect = vfx.GetComponent<VisualEffect>();
        if (visualEffect != null)
        {
            visualEffect.Stop();
            //Debug.Log("Visual Effect stopped.");
            StartCoroutine(DelayedDeactivation(vfx, 1f)); // 延遲 1 秒
        }
        else
        {
            DeactivateVFX(vfx);
        }
    }

    private IEnumerator DelayedDeactivation(GameObject vfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        DeactivateVFX(vfx);
    }

    private void DeactivateVFX(GameObject vfx)
    {
        vfx.transform.SetParent(null);
        vfx.transform.localPosition = Vector3.zero;
        vfx.transform.localRotation = Quaternion.identity;
        vfx.transform.localScale = Vector3.one;

        vfx.SetActive(false);
        vfxPool.Enqueue(vfx);

        //Debug.Log("VFX returned to pool. Pool count: " + vfxPool.Count);
    }

    //=================================================================================================================================================================================

    public void TakeDamage(int damage)
    {
        hited = true;

        // SFX
        AudioClip ramdomSFX = enemyAudioClip[Random.Range(0, 3)];
        enemyAudioSource.volume = Random.Range(1 - volumeChangeMultiplier, 1);
        enemyAudioSource.pitch = Random.Range(1 - pitchChangeMultiplier, 1 + pitchChangeMultiplier);
        enemyAudioSource.PlayOneShot(ramdomSFX);

        if (animator != null)
        {
            // VFX
            if (!vfxIsCreated)
            {
                if (onHitVFX != null)
                {
                    Instantiate(onHitVFX, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), transform.rotation);
                }
            }

            animator.SetTrigger("isHited");
        }

        health -= damage;
        Debug.Log("Boss Health: " + health);
        if (health <= 0)
        {
            isDead = true;

            if (animator != null)
            {
                DestroyAnimation();
            }
        }
    }

    private void DestroyAnimation()
    {
        gameObject.GetComponent<NavMeshAgent>().enabled = false;
        animator.Play("Dead");

        enemyAudioSource.PlayOneShot(enemyAudioClip[4]);

        // VFX
        if (!vfxIsCreated)
        {
            Instantiate(spark, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), transform.rotation);
            vfxIsCreated = true;
        }

        // Delay 10sec
        Invoke("DestroyObject", 10);
    }

    public void DestroyObject()
    {
        Destroy(DestroyObj);
    }

    //=================================================================================================================================================================================

    public void Shoot(Transform barrel)
    {
        // 從對象池中獲取子彈
        GameObject bullet = BulletPoolManager.Instance.GetEnemyBullet();
        bullet.SetActive(true);
        bullet.transform.position = barrel.position;

        // 計算朝向玩家的方向
        Vector3 directionToPlayer = (player.position - barrel.position).normalized;
        bullet.transform.rotation = Quaternion.LookRotation(directionToPlayer);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(directionToPlayer * shotPower); // 使用計算出的方向，而不是 barrel.forward
    }

    //=================================================================================================================================================================================

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
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
    [SerializeField] public int health;
    [SerializeField] private int maxHealth;
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

    // VFX
    [Header("OnHit VFX")]
    [SerializeField] public ParticleSystem spark;
    [SerializeField] public ParticleSystem onHitVFX;


    //===================================================================================================================================================================================================




    [Header("MiniGun Attack")]
    // Attack Player
    public int timeBetweenAttacks = 2;
    public bool alreadyAttacked;
    public int minigun_Fire_Density = 20;
    private int currentFire_Density = 0;
    public float minigun_Fire_Gap = 0.1f;
    private int Attack_SkillsNumber = 0;
    private int RanNum = 0;

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
    [Tooltip("Bullet Speed")][SerializeField] private float shotPower = 600f;
    [Tooltip("Point Light Intensity")][SerializeField] private float maxLightIntensity = 2f;
    [Tooltip("Point Light Fade Duration")][SerializeField] private float lightFadeDuration = 0.5f;

    //===================================================================================================================================================================================================


    [Header("Missile Attack")]
    [SerializeField] private GameObject impactCirclePrefab; // 著彈點預製體
    [SerializeField] private int missileCount = 3; // 一次攻擊生成多少個著彈點
    [SerializeField] private float missileLaunchDelay = 1.5f; // 每個導彈之間的發射間隔
    [SerializeField] private int missileBeatsToExpand = 2; // 控制 ImpactCircle 的 beatsToExpand

    [Header("Missile Launch Position")]
    [SerializeField] private Transform missileLaunchPosition; // 導彈發射位置（例如槍口）

    [Header("Missile Launch VFX")]
    [SerializeField] private GameObject missileLaunchVFXPrefab; // MissileLaunchVFX 預製體
    private Queue<GameObject> missileVFXPool = new Queue<GameObject>(); // MissileLaunchVFX 對象池
    [SerializeField] private int missileVFXPoolSize = 3; // 對象池大小
    [SerializeField] private float missileVFXDuration = 5f; // MissileLaunchVFX 的播放時長

    [Header("Explosion VFX")]
    [SerializeField] private GameObject explosionVFXPrefab; // ExplosionVFX 預製體
    private Queue<GameObject> explosionVFXPool = new Queue<GameObject>(); // ExplosionVFX 對象池
    [SerializeField] private int explosionVFXPoolSize = 3; // 對象池大小
    [SerializeField] private float explosionVFXDuration = 5f; // ExplosionVFX 的播放時長


    //===================================================================================================================================================================================================


    [Header("Mini Missile Attack")]
    [SerializeField] private GameObject miniImpactCirclePrefab; // 小型導彈的著彈點預製體
    [SerializeField] private int miniMissileCount = 10; // 一次攻擊生成多少個小型導彈
    [SerializeField] private float miniMissileLaunchDelay = 0.2f; // 每個小型導彈之間的發射間隔
    [SerializeField] private int miniMissileBeatsToExpand = 8; // 小型導彈的 ImpactCircle 的 beatsToExpand
    [SerializeField] private float randomRadius = 10f; // 小型導彈著陸點的隨機半徑（以玩家為中心）
    [SerializeField] private float minDistanceBetweenCircles = 2f; // 著彈點之間的最小間距（根據 MiniImpactCircle 的爆炸範圍調整）
    [SerializeField] private float innerRadiusFactor = 0.5f; // 內圈半徑比例（相對於 randomRadius）
    [SerializeField] private float innerCircleRatio = 0.3f; // 內圈著彈點的比例（0 到 1）
    [SerializeField] private float innerConcentrationFactor = 0.3f; // 內圈的集中因子（值越小越集中）
    [SerializeField] private float outerConcentrationFactor = 0.7f; // 外圈的集中因子（值越大越分散）
    [SerializeField] private float innerOuterGap = 0.7f; // 內圈和外圈之間的時間差

    [Header("Mini Missile Attack (LowHealth Mode)")]
    [SerializeField] private int lowHealthMiniMissileCount = 20; // 低血量時的導彈數量
    [SerializeField] private float lowHealthMiniMissileLaunchDelay = 0.125f; // 低血量時的發射間隔
    [SerializeField] private float lowHealthInnerCircleRatio = 0.1f; // 低血量時內圈著彈點的比例
    [SerializeField] private float lowHealthInnerConcentrationFactor = 0.1f; // 低血量時內圈的集中因子
    [SerializeField] private float lowHealthInnerOuterGap = 0.3f; // 內圈和外圈之間的時間差

    [Header("Mini Missile Launch Positions")]
    [SerializeField] private Transform leftMiniMissileSpawnPoint; // 左側小型導彈發射點 (L_MINIMISSILE_SPAWNPOINT)
    [SerializeField] private Transform rightMiniMissileSpawnPoint; // 右側小型導彈發射點 (R_MINIMISSILE_SPAWNPOINT)

    [Header("Mini Missile Launch VFX")]
    [SerializeField] private GameObject miniMissileLaunchVFXPrefab; // MiniMissileSwarm VFX 預製體
    private Queue<GameObject> miniMissileVFXPool = new Queue<GameObject>(); // MiniMissileSwarm VFX 對象池
    [SerializeField] private int miniMissileVFXPoolSize = 10; // 對象池大小
    [SerializeField] private float miniMissileVFXDuration = 3f; // MiniMissileSwarm VFX 的播放時長

    [Header("Mini Explosion VFX")]
    [SerializeField] private GameObject miniExplosionVFXPrefab; // MiniExplosion VFX 預製體
    private Queue<GameObject> miniExplosionVFXPool = new Queue<GameObject>(); // MiniExplosion VFX 對象池
    [SerializeField] private int miniExplosionVFXPoolSize = 10; // 對象池大小
    [SerializeField] private float miniExplosionVFXDuration = 3f; // MiniExplosion VFX 的播放時長



    //===================================================================================================================================================================================================


    // SFX
    [Header("SoundFX")]
    public AudioSource enemyAudioSource;
    public AudioClip fireSound; // MinigunFireLoop
    public AudioClip[] enemyAudioClip;
    private float volumeChangeMultiplier = 0.2f;
    private float pitchChangeMultiplier = 0.2f;


    //===================================================================================================================================================================================================

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
        maxHealth = health;

        // 初始化 Muzzle Flash VFX 對象池
        for (int i = 0; i < vfxPoolSize; i++)
        {
            GameObject vfx = Instantiate(shootingVFXPrefab);
            vfx.SetActive(false);
            vfxPool.Enqueue(vfx);
        }

        // 初始化 MissileLaunchVFX 對象池
        for (int i = 0; i < missileVFXPoolSize; i++)
        {
            GameObject missileVFX = Instantiate(missileLaunchVFXPrefab);
            missileVFX.SetActive(false);
            missileVFXPool.Enqueue(missileVFX);
        }

        // 初始化 ExplosionVFX 對象池
        for (int i = 0; i < explosionVFXPoolSize; i++)
        {
            GameObject explosionVFX = Instantiate(explosionVFXPrefab);
            explosionVFX.SetActive(false);
            explosionVFXPool.Enqueue(explosionVFX);
        }

        // 初始化 MiniMissileSwarm VFX 對象池
        for (int i = 0; i < miniMissileVFXPoolSize; i++)
        {
            GameObject miniMissileVFX = Instantiate(miniMissileLaunchVFXPrefab);
            miniMissileVFX.SetActive(false);
            miniMissileVFXPool.Enqueue(miniMissileVFX);
        }

        // 初始化 MiniExplosion VFX 對象池
        for (int i = 0; i < miniExplosionVFXPoolSize; i++)
        {
            GameObject miniExplosionVFX = Instantiate(miniExplosionVFXPrefab);
            miniExplosionVFX.SetActive(false);
            miniExplosionVFXPool.Enqueue(miniExplosionVFX);
        }
    }

    private void Update()
    {
        SwitchAnimation();

        // 根據血量調整攻擊參數
        if (health <= (maxHealth / 2))
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

        // 確保Boss不移動
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            while (Attack_SkillsNumber == RanNum)
            {
                Attack_SkillsNumber = Random.Range(0, 5); // 增加到 5 種模式
            }
            RanNum = Attack_SkillsNumber;

            switch (Attack_SkillsNumber)
            {
                case 4:
                    MiniMissileSwarmAttack(); // 小型導彈群攻擊
                    print("Mini Missile Swarm Attack");
                    break;
                case 3:
                    MissileAttack(); // 導彈攻擊技能
                    print("Missile Attack");
                    break;
                case 2:
                    MiniMissileSwarmAttack();
                    //GunAttackBoth(); // 同時使用左右槍口
                    print("Both Guns Attack");
                    break;
                case 1:
                    MiniMissileSwarmAttack();
                    //GunAttackRight(); // 僅使用右槍口
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




    //=================================================================================================================================================================================



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

        currentFire_Density = currentFire_Density * 2;

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



    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------



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



    private void MissileAttack()
    {
        isAttack = true;
        int beats = lowHealth ? missileBeatsToExpand/2+1 : missileBeatsToExpand+1;
        StartCoroutine(LaunchMissiles(beats)); // 傳遞 beatsToExpand
    }

    private IEnumerator LaunchMissiles(int beatsToExpand)
    {
        isAttack = true;

        for (int i = 0; i < missileCount; i++)
        {
            // 計算玩家的當前位置
            Vector3 targetPosition = player.position;

            CharacterController playerController = player.GetComponent<CharacterController>();
            if (playerController != null)
            {
                float playerHeight = playerController.height;
                Vector3 feetPosition = targetPosition - new Vector3(0f, playerHeight / 2f, 0f);

                RaycastHit hit;
                if (Physics.Raycast(feetPosition + Vector3.up * 0.5f, Vector3.down, out hit, 1f, whatIsGround))
                {
                    targetPosition = hit.point;
                }
                else
                {
                    if (Physics.Raycast(targetPosition + Vector3.up * 2f, Vector3.down, out hit, 3f, whatIsGround))
                    {
                        targetPosition = hit.point;
                    }
                }
            }
            else
            {
                RaycastHit hit;
                if (Physics.Raycast(targetPosition + Vector3.up * 2f, Vector3.down, out hit, 3f, whatIsGround))
                {
                    targetPosition = hit.point;
                }
            }

            // 啟動一個獨立的協程來處理 MissileLaunchVFX 和 ImpactCircle
            StartCoroutine(HandleMissileLaunch(targetPosition, beatsToExpand));

            // 等待 missileLaunchDelay 後發射下一個導彈
            yield return new WaitForSeconds(missileLaunchDelay);
        }

        isAttack = false;
        Invoke("ResetAttack", timeBetweenAttacks + 5);
    }



    private IEnumerator HandleMissileLaunch(Vector3 targetPosition, int beatsToExpand)
    {
        // 播放 MissileLaunchVFX
        GameObject missileVFX = GetMissileVFXFromPool(transform);
        if (missileVFX != null)
        {
            yield return new WaitForSeconds(missileVFXDuration); // 等待 VFX 播放完成（5 秒）
            ReturnMissileVFXToPool(missileVFX);
        }

        // 在生成 ImpactCircle 時，重新計算玩家的當前位置
        targetPosition = player.position;

        CharacterController playerController = player.GetComponent<CharacterController>();
        if (playerController != null)
        {
            float playerHeight = playerController.height;
            Vector3 feetPosition = targetPosition - new Vector3(0f, playerHeight / 2f, 0f);

            RaycastHit hit;
            if (Physics.Raycast(feetPosition + Vector3.up * 0.5f, Vector3.down, out hit, 1f, whatIsGround))
            {
                targetPosition = hit.point;
            }
            else
            {
                if (Physics.Raycast(targetPosition + Vector3.up * 2f, Vector3.down, out hit, 3f, whatIsGround))
                {
                    targetPosition = hit.point;
                }
            }
        }
        else
        {
            RaycastHit hit;
            if (Physics.Raycast(targetPosition + Vector3.up * 2f, Vector3.down, out hit, 3f, whatIsGround))
            {
                targetPosition = hit.point;
            }
        }

        // 生成 ImpactCircle
        GameObject impactCircle = Instantiate(impactCirclePrefab, targetPosition, Quaternion.identity);
        ImpactCircle circleScript = impactCircle.GetComponent<ImpactCircle>();
        if (circleScript != null)
        {
            circleScript.SetBeatsToExpand(beatsToExpand);
            circleScript.SetPosition(targetPosition);
            circleScript.StartExpanding();
        }
    }




    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------




    private GameObject GetMissileVFXFromPool(Transform launchPosition)
    {
        GameObject vfx;
        if (missileVFXPool.Count > 0)
        {
            vfx = missileVFXPool.Dequeue();
            vfx.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Missile VFX pool is empty! Instantiating new VFX.");
            vfx = Instantiate(missileLaunchVFXPrefab);
        }

        // 設置位置和旋轉
        vfx.transform.position = missileLaunchPosition != null ? missileLaunchPosition.position : launchPosition.position;
        vfx.transform.rotation = missileLaunchPosition != null ? missileLaunchPosition.rotation : launchPosition.rotation;

        // 播放 Visual Effect
        var visualEffect = vfx.GetComponent<VisualEffect>();
        if (visualEffect != null)
        {
            visualEffect.Stop();
            visualEffect.Reinit();
            visualEffect.Play();
        }
        else
        {
            Debug.LogError("VisualEffect component not found on MissileLaunchVFX object!");
        }

        return vfx;
    }




    private void ReturnMissileVFXToPool(GameObject vfx)
    {
        var visualEffect = vfx.GetComponent<VisualEffect>();
        if (visualEffect != null)
        {
            visualEffect.Stop();
            StartCoroutine(DelayedDeactivationMissileVFX(vfx, 1f)); // 延遲 1 秒以確保粒子消失
        }
        else
        {
            DeactivateMissileVFX(vfx);
        }
    }


    private IEnumerator DelayedDeactivationMissileVFX(GameObject vfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        DeactivateMissileVFX(vfx);
    }

    private void DeactivateMissileVFX(GameObject vfx)
    {
        vfx.transform.position = Vector3.zero;
        vfx.transform.rotation = Quaternion.identity;
        vfx.SetActive(false);
        missileVFXPool.Enqueue(vfx);
    }


    public GameObject GetExplosionVFXFromPool(Vector3 position)
    {
        if (explosionVFXPrefab == null)
        {
            Debug.LogError("ExplosionVFXPrefab is not set in Boss_Ai!");
            return null;
        }

        GameObject vfx;
        if (explosionVFXPool.Count > 0)
        {
            vfx = explosionVFXPool.Dequeue();
            vfx.SetActive(true);
            Debug.Log("Reusing ExplosionVFX from pool.");
        }
        else
        {
            Debug.LogWarning("Explosion VFX pool is empty! Instantiating new VFX.");
            vfx = Instantiate(explosionVFXPrefab);
        }


        vfx.transform.position = position + new Vector3(0f, 1f, 0f);
        vfx.transform.rotation = Quaternion.identity;

        var visualEffect = vfx.GetComponent<VisualEffect>();
        if (visualEffect != null)
        {
            visualEffect.Stop();
            visualEffect.Reinit();
            visualEffect.Play();
            Debug.Log("ExplosionVFX played at position: " + vfx.transform.position);
        }
        else
        {
            Debug.LogError("VisualEffect component not found on ExplosionVFX object!");
        }

        StartCoroutine(ReturnExplosionVFXAfterDuration(vfx, explosionVFXDuration));
        return vfx;
    }

    private IEnumerator ReturnExplosionVFXAfterDuration(GameObject vfx, float duration)
    {
        yield return new WaitForSeconds(duration); // 直接設置 5 秒
        ReturnExplosionVFXToPool(vfx);
    }

    private void ReturnExplosionVFXToPool(GameObject vfx)
    {
        var visualEffect = vfx.GetComponent<VisualEffect>();
        if (visualEffect != null)
        {
            visualEffect.Stop();
        }

        vfx.transform.position = Vector3.zero;
        vfx.transform.rotation = Quaternion.identity;
        vfx.SetActive(false);
        explosionVFXPool.Enqueue(vfx);
    }




    //=================================================================================================================================================================================



    private void MiniMissileSwarmAttack()
    {
        isAttack = true;
        int beats = lowHealth ? miniMissileBeatsToExpand / 2 + 1 : miniMissileBeatsToExpand + 1;
        StartCoroutine(LaunchMiniMissileSwarm(beats));
    }


    private IEnumerator LaunchMiniMissileSwarm(int beatsToExpand)
    {
        isAttack = true;
        bool useRightSpawnPoint = true; // 從右側開始發射

        // 用於記錄已生成的著彈點位置
        List<Vector3> usedPositions = new List<Vector3>();
        int maxPlacementAttempts = 50; // 最大嘗試次數，避免無限循環

        // 根據 lowHealth 動態選擇參數
        int currentMiniMissileCount = lowHealth ? lowHealthMiniMissileCount : miniMissileCount;
        float currentMiniMissileLaunchDelay = lowHealth ? lowHealthMiniMissileLaunchDelay : miniMissileLaunchDelay;
        float currentInnerCircleRatio = lowHealth ? lowHealthInnerCircleRatio : innerCircleRatio;
        float currentInnerConcentrationFactor = lowHealth ? lowHealthInnerConcentrationFactor : innerConcentrationFactor;

        // 動態調整最小間距
        float adjustedMinDistance = Mathf.Min(minDistanceBetweenCircles, randomRadius / Mathf.Sqrt(currentMiniMissileCount));
        Debug.Log($"Adjusted Min Distance: {adjustedMinDistance} (Original: {minDistanceBetweenCircles}, Random Radius: {randomRadius}, Missile Count: {currentMiniMissileCount})");

        // 計算內圈和外圈的著彈點數量
        int innerCircleCount = Mathf.RoundToInt(currentMiniMissileCount * currentInnerCircleRatio); // 內圈著彈點數量
        int outerCircleCount = currentMiniMissileCount - innerCircleCount; // 外圈著彈點數量
        float innerRadius = randomRadius * innerRadiusFactor; // 內圈半徑

        Debug.Log($"Inner Circle Count: {innerCircleCount}, Outer Circle Count: {outerCircleCount}, Inner Radius: {innerRadius}");

        // 先生成內圈的著彈點
        for (int i = 0; i < innerCircleCount; i++)
        {
            // 選擇當前的發射點
            Transform selectedSpawnPoint = useRightSpawnPoint ? rightMiniMissileSpawnPoint : leftMiniMissileSpawnPoint;

            // 計算玩家的當前位置
            Vector3 playerPosition = player.position;

            // 隨機生成一個著陸點（內圈），確保不重疊
            Vector3 targetPosition = Vector3.zero;
            bool positionFound = false;
            int attempts = 0;

            while (!positionFound && attempts < maxPlacementAttempts)
            {
                // 隨機生成一個角度和距離，使用平方根分佈使著彈點更集中
                float randomAngle = Random.Range(0f, 360f);
                float randomValue = Random.Range(0f, 1f);
                float randomDistance = innerRadius * Mathf.Sqrt(randomValue) * (1f - currentInnerConcentrationFactor) + innerRadius * currentInnerConcentrationFactor * randomValue;

                Vector3 offset = new Vector3(
                    Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomDistance,
                    0f,
                    Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomDistance
                );
                targetPosition = playerPosition + offset;

                // 確保著陸點在地面上
                CharacterController playerController = player.GetComponent<CharacterController>();
                if (playerController != null)
                {
                    float playerHeight = playerController.height;
                    Vector3 feetPosition = targetPosition - new Vector3(0f, playerHeight / 2f, 0f);

                    RaycastHit hit;
                    if (Physics.Raycast(feetPosition + Vector3.up * 0.5f, Vector3.down, out hit, 1f, whatIsGround))
                    {
                        targetPosition = hit.point;
                    }
                    else
                    {
                        if (Physics.Raycast(targetPosition + Vector3.up * 2f, Vector3.down, out hit, 3f, whatIsGround))
                        {
                            targetPosition = hit.point;
                        }
                    }
                }
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(targetPosition + Vector3.up * 2f, Vector3.down, out hit, 3f, whatIsGround))
                    {
                        targetPosition = hit.point;
                    }
                }

                // 檢查與已有著彈點的距離
                positionFound = true;
                foreach (Vector3 usedPos in usedPositions)
                {
                    if (Vector3.Distance(targetPosition, usedPos) < adjustedMinDistance)
                    {
                        positionFound = false;
                        break;
                    }
                }

                attempts++;
            }

            // 如果無法找到不重疊的位置，使用最後一次嘗試的位置（避免無限循環）
            if (!positionFound)
            {
                Debug.LogWarning($"Could not find non-overlapping position for Inner MiniImpactCircle {i + 1} after {maxPlacementAttempts} attempts. Using last position.");
            }

            // 記錄當前著彈點位置
            usedPositions.Add(targetPosition);

            // 啟動一個獨立的協程來處理 MiniMissileSwarm VFX 和 MiniImpactCircle
            StartCoroutine(HandleMiniMissileLaunch(selectedSpawnPoint, targetPosition, beatsToExpand));

            // 等待動態計算的 miniMissileLaunchDelay 後發射下一個小型導彈
            yield return new WaitForSeconds(currentMiniMissileLaunchDelay);

            // 交替使用左右發射點
            useRightSpawnPoint = !useRightSpawnPoint;
        }



        // 內圈生成後等待一段時間
        float waitSeconds = lowHealth ? innerOuterGap : lowHealthInnerOuterGap;
        yield return new WaitForSeconds(waitSeconds); // 內圈和外圈之間的時間差


        // 再生成外圈的著彈點
        for (int i = 0; i < outerCircleCount; i++)
        {
            // 選擇當前的發射點
            Transform selectedSpawnPoint = useRightSpawnPoint ? rightMiniMissileSpawnPoint : leftMiniMissileSpawnPoint;

            // 計算玩家的當前位置
            Vector3 playerPosition = player.position;

            // 隨機生成一個著陸點（外圈），確保不重疊
            Vector3 targetPosition = Vector3.zero;
            bool positionFound = false;
            int attempts = 0;

            while (!positionFound && attempts < maxPlacementAttempts)
            {
                // 隨機生成一個角度和距離，使用平方根分佈
                float randomAngle = Random.Range(0f, 360f);
                float randomValue = Random.Range(0f, 1f);
                float randomDistance = randomRadius * Mathf.Sqrt(randomValue) * (1f - outerConcentrationFactor) + randomRadius * outerConcentrationFactor * randomValue;

                // 確保外圈的點不會進入內圈（限制最小距離）
                if (randomDistance < innerRadius)
                {
                    randomDistance = Mathf.Lerp(innerRadius, randomRadius, randomValue);
                }

                Vector3 offset = new Vector3(
                    Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomDistance,
                    0f,
                    Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomDistance
                );
                targetPosition = playerPosition + offset;

                // 確保著陸點在地面上
                CharacterController playerController = player.GetComponent<CharacterController>();
                if (playerController != null)
                {
                    float playerHeight = playerController.height;
                    Vector3 feetPosition = targetPosition - new Vector3(0f, playerHeight / 2f, 0f);

                    RaycastHit hit;
                    if (Physics.Raycast(feetPosition + Vector3.up * 0.5f, Vector3.down, out hit, 1f, whatIsGround))
                    {
                        targetPosition = hit.point;
                    }
                    else
                    {
                        if (Physics.Raycast(targetPosition + Vector3.up * 2f, Vector3.down, out hit, 3f, whatIsGround))
                        {
                            targetPosition = hit.point;
                        }
                    }
                }
                else
                {
                    RaycastHit hit;
                    if (Physics.Raycast(targetPosition + Vector3.up * 2f, Vector3.down, out hit, 3f, whatIsGround))
                    {
                        targetPosition = hit.point;
                    }
                }

                // 檢查與已有著彈點的距離
                positionFound = true;
                foreach (Vector3 usedPos in usedPositions)
                {
                    if (Vector3.Distance(targetPosition, usedPos) < adjustedMinDistance)
                    {
                        positionFound = false;
                        break;
                    }
                }

                attempts++;
            }

            // 如果無法找到不重疊的位置，使用最後一次嘗試的位置（避免無限循環）
            if (!positionFound)
            {
                Debug.LogWarning($"Could not find non-overlapping position for Outer MiniImpactCircle {i + 1} after {maxPlacementAttempts} attempts. Using last position.");
            }

            // 記錄當前著彈點位置
            usedPositions.Add(targetPosition);

            // 啟動一個獨立的協程來處理 MiniMissileSwarm VFX 和 MiniImpactCircle
            StartCoroutine(HandleMiniMissileLaunch(selectedSpawnPoint, targetPosition, beatsToExpand));

            // 等待動態計算的 miniMissileLaunchDelay 後發射下一個小型導彈
            yield return new WaitForSeconds(currentMiniMissileLaunchDelay);

            // 交替使用左右發射點
            useRightSpawnPoint = !useRightSpawnPoint;
        }

        isAttack = false;
        Invoke("ResetAttack", timeBetweenAttacks + 5);
    }


    private IEnumerator HandleMiniMissileLaunch(Transform spawnPoint, Vector3 targetPosition, int beatsToExpand)
    {
        // 播放 MiniMissileSwarm VFX（使用新的 miniMissileVFXPool）
        GameObject miniMissileVFX = GetMiniMissileLaunchVFXFromPool(spawnPoint);
        if (miniMissileVFX != null)
        {
            yield return new WaitForSeconds(miniMissileVFXDuration); // 等待 VFX 播放完成（3 秒）
            ReturnMiniMissileSwarmVFXToPool(miniMissileVFX);
        }

        // 生成 MiniImpactCircle
        GameObject miniImpactCircle = Instantiate(miniImpactCirclePrefab, targetPosition, Quaternion.identity);
        ImpactCircle circleScript = miniImpactCircle.GetComponent<ImpactCircle>();
        if (circleScript != null)
        {
            circleScript.SetBeatsToExpand(beatsToExpand);
            circleScript.SetPosition(targetPosition);
            circleScript.StartExpanding();
        }
    }

    private GameObject GetMiniMissileLaunchVFXFromPool(Transform launchPosition)
    {
        GameObject vfx;
        if (miniMissileVFXPool.Count > 0)
        {
            vfx = miniMissileVFXPool.Dequeue();
            vfx.SetActive(true);
        }
        else
        {
            Debug.LogWarning("MiniMissileSwarm VFX pool is empty! Instantiating new VFX.");
            vfx = Instantiate(miniMissileLaunchVFXPrefab);
        }

        // 設置位置和旋轉
        vfx.transform.position = launchPosition.position;
        vfx.transform.rotation = launchPosition.rotation;

        // 播放 Visual Effect
        var visualEffect = vfx.GetComponent<VisualEffect>();
        if (visualEffect != null)
        {
            visualEffect.Stop();
            visualEffect.Reinit();
            visualEffect.Play();
        }
        else
        {
            Debug.LogError("VisualEffect component not found on MiniMissileSwarm VFX object!");
        }

        return vfx;
    }



    private void ReturnMiniMissileSwarmVFXToPool(GameObject vfx)
    {
        var visualEffect = vfx.GetComponent<VisualEffect>();
        if (visualEffect != null)
        {
            visualEffect.Stop();
            StartCoroutine(DelayedDeactivationMiniMissileSwarmVFX(vfx, 1f)); // 延遲 1 秒以確保粒子消失
        }
        else
        {
            DeactivateMiniMissileSwarmVFX(vfx);
        }
    }

    private IEnumerator DelayedDeactivationMiniMissileSwarmVFX(GameObject vfx, float delay)
    {
        yield return new WaitForSeconds(delay);
        DeactivateMiniMissileSwarmVFX(vfx);
    }

    private void DeactivateMiniMissileSwarmVFX(GameObject vfx)
    {
        vfx.transform.position = Vector3.zero;
        vfx.transform.rotation = Quaternion.identity;
        vfx.SetActive(false);
        miniMissileVFXPool.Enqueue(vfx);
    }




    //-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------



    public GameObject GetMiniExplosionVFXFromPool(Vector3 position)
    {
        if (miniExplosionVFXPrefab == null)
        {
            Debug.LogError("MiniExplosionVFXPrefab is not set in Boss_Ai!");
            return null;
        }

        GameObject vfx;
        if (miniExplosionVFXPool.Count > 0)
        {
            vfx = miniExplosionVFXPool.Dequeue();
            vfx.SetActive(true);
            Debug.Log("Reusing MiniExplosionVFX from pool.");
        }
        else
        {
            Debug.LogWarning("MiniExplosion VFX pool is empty! Instantiating new VFX.");
            vfx = Instantiate(miniExplosionVFXPrefab);
        }

        vfx.transform.position = position + new Vector3(0f, 0.35f, 0f);
        vfx.transform.rotation = Quaternion.identity;

        var visualEffect = vfx.GetComponent<VisualEffect>();
        if (visualEffect != null)
        {
            visualEffect.Stop();
            visualEffect.Reinit();
            visualEffect.Play();
            Debug.Log("MiniExplosionVFX played at position: " + vfx.transform.position);
        }
        else
        {
            Debug.LogError("VisualEffect component not found on MiniExplosionVFX object!");
        }

        StartCoroutine(ReturnMiniExplosionVFXAfterDuration(vfx, miniExplosionVFXDuration));
        return vfx;
    }

    private IEnumerator ReturnMiniExplosionVFXAfterDuration(GameObject vfx, float duration)
    {
        yield return new WaitForSeconds(duration); // 直接設置為 miniExplosionVFXDuration
        ReturnMiniExplosionVFXToPool(vfx);
    }

    private void ReturnMiniExplosionVFXToPool(GameObject vfx)
    {
        var visualEffect = vfx.GetComponent<VisualEffect>();
        if (visualEffect != null)
        {
            visualEffect.Stop();
        }

        vfx.transform.position = Vector3.zero;
        vfx.transform.rotation = Quaternion.identity;
        vfx.SetActive(false);
        miniExplosionVFXPool.Enqueue(vfx);
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
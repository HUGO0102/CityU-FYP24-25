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
    [SerializeField] public GameObject shootingVFXPrefab; // �s�� VFX �w�s��A�]�t Muzzle Flash�]Visual Effect�^
    private GameObject rightVFXInstance; // �k��j�f�� VFX ���
    private GameObject leftVFXInstance;  // ����j�f�� VFX ���
    private Queue<GameObject> vfxPool = new Queue<GameObject>(); // VFX ��H��
    [SerializeField] private int vfxPoolSize = 4; // ��H���j�p

    // Barrel
    [Header("Location Refrences")]
    [SerializeField] private Transform rBarrelLocation;
    [SerializeField] private Transform lBarrelLocation;
    [Header("Barrel Spinners")]
    [SerializeField] private BarrelSpinner rightBarrelSpinner;
    [SerializeField] private BarrelSpinner leftBarrelSpinner;
    [Header("Barrel Audio Sources")]
    [SerializeField] private AudioSource rightGunAudioSource; // �k�j�f�� AudioSource
    [SerializeField] private AudioSource leftGunAudioSource;  // ���j�f�� AudioSource
    [Header("Barrel Lights")]
    [SerializeField] private GameObject rightBarrelLight; // �k�j�f�� PointLight GameObject
    [SerializeField] private GameObject leftBarrelLight;  // ���j�f�� PointLight GameObject

    [Header("Settings")]
    [Tooltip("Specify time to destroy the VFX object")][SerializeField] private float destroyTimer = 2f;
    [Tooltip("Bullet Speed")][SerializeField] private float shotPower = 600f;
    [Tooltip("Point Light Intensity")][SerializeField] private float maxLightIntensity = 2f;
    [Tooltip("Point Light Fade Duration")][SerializeField] private float lightFadeDuration = 0.5f;

    //===================================================================================================================================================================================================


    [Header("Missile Attack")]
    [SerializeField] private GameObject impactCirclePrefab; // �ۼu�I�w�s��
    [SerializeField] private int missileCount = 3; // �@�������ͦ��h�֭ӵۼu�I
    [SerializeField] private float missileLaunchDelay = 1.5f; // �C�Ӿɼu�������o�g���j
    [SerializeField] private int missileBeatsToExpand = 2; // ���� ImpactCircle �� beatsToExpand

    [Header("Missile Launch Position")]
    [SerializeField] private Transform missileLaunchPosition; // �ɼu�o�g��m�]�Ҧp�j�f�^

    [Header("Missile Launch VFX")]
    [SerializeField] private GameObject missileLaunchVFXPrefab; // MissileLaunchVFX �w�s��
    private Queue<GameObject> missileVFXPool = new Queue<GameObject>(); // MissileLaunchVFX ��H��
    [SerializeField] private int missileVFXPoolSize = 3; // ��H���j�p
    [SerializeField] private float missileVFXDuration = 5f; // MissileLaunchVFX ������ɪ�

    [Header("Explosion VFX")]
    [SerializeField] private GameObject explosionVFXPrefab; // ExplosionVFX �w�s��
    private Queue<GameObject> explosionVFXPool = new Queue<GameObject>(); // ExplosionVFX ��H��
    [SerializeField] private int explosionVFXPoolSize = 3; // ��H���j�p
    [SerializeField] private float explosionVFXDuration = 5f; // ExplosionVFX ������ɪ�


    //===================================================================================================================================================================================================


    [Header("Mini Missile Attack")]
    [SerializeField] private GameObject miniImpactCirclePrefab; // �p���ɼu���ۼu�I�w�s��
    [SerializeField] private int miniMissileCount = 10; // �@�������ͦ��h�֭Ӥp���ɼu
    [SerializeField] private float miniMissileLaunchDelay = 0.2f; // �C�Ӥp���ɼu�������o�g���j
    [SerializeField] private int miniMissileBeatsToExpand = 8; // �p���ɼu�� ImpactCircle �� beatsToExpand
    [SerializeField] private float randomRadius = 10f; // �p���ɼu�۳��I���H���b�|�]�H���a�����ߡ^
    [SerializeField] private float minDistanceBetweenCircles = 2f; // �ۼu�I�������̤p���Z�]�ھ� MiniImpactCircle ���z���d��վ�^
    [SerializeField] private float innerRadiusFactor = 0.5f; // ����b�|��ҡ]�۹�� randomRadius�^
    [SerializeField] private float innerCircleRatio = 0.3f; // ����ۼu�I����ҡ]0 �� 1�^
    [SerializeField] private float innerConcentrationFactor = 0.3f; // ���骺�����]�l�]�ȶV�p�V�����^
    [SerializeField] private float outerConcentrationFactor = 0.7f; // �~�骺�����]�l�]�ȶV�j�V�����^
    [SerializeField] private float innerOuterGap = 0.7f; // ����M�~�餧�����ɶ��t

    [Header("Mini Missile Attack (LowHealth Mode)")]
    [SerializeField] private int lowHealthMiniMissileCount = 20; // �C��q�ɪ��ɼu�ƶq
    [SerializeField] private float lowHealthMiniMissileLaunchDelay = 0.125f; // �C��q�ɪ��o�g���j
    [SerializeField] private float lowHealthInnerCircleRatio = 0.1f; // �C��q�ɤ���ۼu�I�����
    [SerializeField] private float lowHealthInnerConcentrationFactor = 0.1f; // �C��q�ɤ��骺�����]�l
    [SerializeField] private float lowHealthInnerOuterGap = 0.3f; // ����M�~�餧�����ɶ��t

    [Header("Mini Missile Launch Positions")]
    [SerializeField] private Transform leftMiniMissileSpawnPoint; // �����p���ɼu�o�g�I (L_MINIMISSILE_SPAWNPOINT)
    [SerializeField] private Transform rightMiniMissileSpawnPoint; // �k���p���ɼu�o�g�I (R_MINIMISSILE_SPAWNPOINT)

    [Header("Mini Missile Launch VFX")]
    [SerializeField] private GameObject miniMissileLaunchVFXPrefab; // MiniMissileSwarm VFX �w�s��
    private Queue<GameObject> miniMissileVFXPool = new Queue<GameObject>(); // MiniMissileSwarm VFX ��H��
    [SerializeField] private int miniMissileVFXPoolSize = 10; // ��H���j�p
    [SerializeField] private float miniMissileVFXDuration = 3f; // MiniMissileSwarm VFX ������ɪ�

    [Header("Mini Explosion VFX")]
    [SerializeField] private GameObject miniExplosionVFXPrefab; // MiniExplosion VFX �w�s��
    private Queue<GameObject> miniExplosionVFXPool = new Queue<GameObject>(); // MiniExplosion VFX ��H��
    [SerializeField] private int miniExplosionVFXPoolSize = 10; // ��H���j�p
    [SerializeField] private float miniExplosionVFXDuration = 3f; // MiniExplosion VFX ������ɪ�



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

        // ��l�� Muzzle Flash VFX ��H��
        for (int i = 0; i < vfxPoolSize; i++)
        {
            GameObject vfx = Instantiate(shootingVFXPrefab);
            vfx.SetActive(false);
            vfxPool.Enqueue(vfx);
        }

        // ��l�� MissileLaunchVFX ��H��
        for (int i = 0; i < missileVFXPoolSize; i++)
        {
            GameObject missileVFX = Instantiate(missileLaunchVFXPrefab);
            missileVFX.SetActive(false);
            missileVFXPool.Enqueue(missileVFX);
        }

        // ��l�� ExplosionVFX ��H��
        for (int i = 0; i < explosionVFXPoolSize; i++)
        {
            GameObject explosionVFX = Instantiate(explosionVFXPrefab);
            explosionVFX.SetActive(false);
            explosionVFXPool.Enqueue(explosionVFX);
        }

        // ��l�� MiniMissileSwarm VFX ��H��
        for (int i = 0; i < miniMissileVFXPoolSize; i++)
        {
            GameObject miniMissileVFX = Instantiate(miniMissileLaunchVFXPrefab);
            miniMissileVFX.SetActive(false);
            miniMissileVFXPool.Enqueue(miniMissileVFX);
        }

        // ��l�� MiniExplosion VFX ��H��
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

        // �ھڦ�q�վ�����Ѽ�
        if (health <= (maxHealth / 2))
        {
            lowHealth = true;
            currentFire_Density = minigun_Fire_Density * 2; // �W�[�g���K��
            minigun_Fire_Gap = 0.08f; // ��֮g�����j
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

        // �Ϻj�f�¦V���a
        rBarrelLocation.LookAt(player.position);
        lBarrelLocation.LookAt(player.position);

        // �T�OBoss������
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            while (Attack_SkillsNumber == RanNum)
            {
                Attack_SkillsNumber = Random.Range(0, 5); // �W�[�� 5 �ؼҦ�
            }
            RanNum = Attack_SkillsNumber;

            switch (Attack_SkillsNumber)
            {
                case 4:
                    MiniMissileSwarmAttack(); // �p���ɼu�s����
                    print("Mini Missile Swarm Attack");
                    break;
                case 3:
                    MissileAttack(); // �ɼu�����ޯ�
                    print("Missile Attack");
                    break;
                case 2:
                    MiniMissileSwarmAttack();
                    //GunAttackBoth(); // �P�ɨϥΥ��k�j�f
                    print("Both Guns Attack");
                    break;
                case 1:
                    MiniMissileSwarmAttack();
                    //GunAttackRight(); // �ȨϥΥk�j�f
                    print("Right Gun Attack");
                    break;
                case 0:
                    GunAttackLeft(); // �ȨϥΥ��j�f
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

        // �}�l����
        selectedAudioSource.volume = 1f;
        selectedSpinner.StartSpinning();

        // ���� Spin up ����
        yield return StartCoroutine(selectedSpinner.WaitForSpinUp());

        isAttack = true;

        // �q��H������� VFX
        selectedVFXInstance = GetVFXFromPool(barrel, isRightBarrel);
        if (isRightBarrel)
        {
            rightVFXInstance = selectedVFXInstance;
        }
        else
        {
            leftVFXInstance = selectedVFXInstance;
        }

        // �]�m�g�����Ĭ��`���ü���
        selectedAudioSource.clip = fireSound;
        selectedAudioSource.loop = true;
        selectedAudioSource.pitch = lowHealth ? 1.1f : 1f;
        selectedAudioSource.Play();

        // �ҥ� PointLight GameObject �òH�J
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


        // ��������g������
        if (selectedAudioSource != null)
        {
            selectedAudioSource.Stop();
            selectedAudioSource.volume = 1f;
            //Debug.Log("FireBurst: Stopped AudioSource for barrel: " + (isRightBarrel ? "Right" : "Left"));
        }

        // �������ü��� Wind Down Sound
        selectedSpinner.StopSpinning();

        // �H�X PointLight
        if (lights != null)
        {
            StartCoroutine(FadeOutLight(lights, lightFadeDuration, selectedLight));
        }

        // �N VFX ��^��H��
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
        audioSource.volume = startVolume; // ��_���q
    }


    private IEnumerator FireBurstBoth()
    {
        int i = 0;
        bool useRightBarrel = true;

        // �P�ɶ}�l���k�j�f�� Spin up
        rightGunAudioSource.volume = 0.5f;
        leftGunAudioSource.volume = 0.5f;
        rightBarrelSpinner.StartSpinning();
        leftBarrelSpinner.StartSpinning();

        // ���ݨ��䳣���� Spin up
        yield return StartCoroutine(rightBarrelSpinner.WaitForSpinUp());
        yield return StartCoroutine(leftBarrelSpinner.WaitForSpinUp());

        isAttack = true;

        // �q��H����������k�j�f�� VFX
        rightVFXInstance = GetVFXFromPool(rBarrelLocation, true);
        leftVFXInstance = GetVFXFromPool(lBarrelLocation, false);

        // �]�m���k�j�f���g�����Ĭ��`���ü���
        rightGunAudioSource.clip = fireSound;
        rightGunAudioSource.loop = true;
        rightGunAudioSource.pitch = lowHealth ? 1.1f : 1f; // �C��q�ɴ�������
        rightGunAudioSource.volume = 0.5f;
        rightGunAudioSource.Play();

        leftGunAudioSource.clip = fireSound;
        leftGunAudioSource.loop = true;
        leftGunAudioSource.pitch = lowHealth ? 1.1f : 1f; // �C��q�ɴ�������
        leftGunAudioSource.volume = 0.5f;
        leftGunAudioSource.Play();

        // �ҥΥ��k�j�f�� PointLight GameObject �òH�J
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

        // �H�J�ĪG
        if (rightLights != null)
        {
            StartCoroutine(FadeInLight(rightLights, lightFadeDuration));
        }
        if (leftLights != null)
        {
            StartCoroutine(FadeInLight(leftLights, lightFadeDuration));
        }

        currentFire_Density = currentFire_Density * 2;

        // �}�l����g��
        while (i <= currentFire_Density)
        {
            Transform selectedBarrel = useRightBarrel ? rBarrelLocation : lBarrelLocation;
            Light[] selectedLights = useRightBarrel ? rightLights : leftLights;
            Shoot(selectedBarrel);
            // ��s PointLight �G�שM�C��A�����{�{�ĪG
            if (selectedLights != null)
            {
                foreach (var light in selectedLights)
                {
                    light.intensity = maxLightIntensity * Random.Range(0.8f, 1f); // �H���ܤƫG��
                    float t = Random.Range(0f, 1f);
                    light.color = Color.Lerp(new Color(1f, 0.5f, 0f), new Color(1f, 1f, 0f), t);
                }
            }
            i++;
            useRightBarrel = !useRightBarrel; // ����ϥΥ��k�j�f
            yield return new WaitForSeconds(minigun_Fire_Gap);
        }


        // ��������g�����ġA���H�X
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

        // �������ü��� Wind Down Sound
        rightBarrelSpinner.StopSpinning();
        leftBarrelSpinner.StopSpinning();


        // �H�X PointLight
        if (rightLights != null)
        {
            StartCoroutine(FadeOutLight(rightLights, lightFadeDuration, rightBarrelLight));
        }
        if (leftLights != null)
        {
            StartCoroutine(FadeOutLight(leftLights, lightFadeDuration, leftBarrelLight));
        }

        // �N���k�j�f�� VFX ��^��H��
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

        // �]�m��m�M����
        vfx.transform.SetParent(barrel);
        vfx.transform.localPosition = Vector3.zero;
        vfx.transform.localRotation = Quaternion.identity;

        // ½�४��j�f�� VFX
        if (!isRightBarrel)
        {
            Vector3 localScale = vfx.transform.localScale;
            vfx.transform.localScale = new Vector3(-localScale.x, localScale.y, localScale.z);
        }

        // ���� Visual Effect ������
        var visualEffect = vfx.GetComponent<VisualEffect>();
        if (visualEffect != null)
        {
            // ���m Visual Effect
            visualEffect.Stop();
            visualEffect.Reinit();
            //Debug.Log("Visual Effect initialized for barrel: " + (isRightBarrel ? "Right" : "Left"));

            // �]�m����Ѽơ]�Ҧp Spawn Rate �� Delay�^
            if (visualEffect.HasFloat("fire_delay"))
            {
                visualEffect.SetFloat("fire_delay", minigun_Fire_Gap);
                //Debug.Log("Set fire_delay to: " + fire_Gap);
            }
            else
            {
                Debug.LogWarning("fire_delay parameter not found in Visual Effect!");
            }

            // �]�m�ɤl�ͦ��W�v�]�p�G VFX Graph �����Ѽơ^
            if (visualEffect.HasFloat("spawnRate"))
            {
                visualEffect.SetFloat("spawnRate", 1f / minigun_Fire_Gap);
                //Debug.Log("Set spawnRate to: " + (1f / fire_Gap));
            }

            // ���� Visual Effect
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

        // ���� Visual Effect
        var visualEffect = vfx.GetComponent<VisualEffect>();
        if (visualEffect != null)
        {
            visualEffect.Stop();
            //Debug.Log("Visual Effect stopped.");
            StartCoroutine(DelayedDeactivation(vfx, 1f)); // ���� 1 ��
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
        StartCoroutine(LaunchMissiles(beats)); // �ǻ� beatsToExpand
    }

    private IEnumerator LaunchMissiles(int beatsToExpand)
    {
        isAttack = true;

        for (int i = 0; i < missileCount; i++)
        {
            // �p�⪱�a����e��m
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

            // �Ұʤ@�ӿW�ߪ���{�ӳB�z MissileLaunchVFX �M ImpactCircle
            StartCoroutine(HandleMissileLaunch(targetPosition, beatsToExpand));

            // ���� missileLaunchDelay ��o�g�U�@�Ӿɼu
            yield return new WaitForSeconds(missileLaunchDelay);
        }

        isAttack = false;
        Invoke("ResetAttack", timeBetweenAttacks + 5);
    }



    private IEnumerator HandleMissileLaunch(Vector3 targetPosition, int beatsToExpand)
    {
        // ���� MissileLaunchVFX
        GameObject missileVFX = GetMissileVFXFromPool(transform);
        if (missileVFX != null)
        {
            yield return new WaitForSeconds(missileVFXDuration); // ���� VFX ���񧹦��]5 ��^
            ReturnMissileVFXToPool(missileVFX);
        }

        // �b�ͦ� ImpactCircle �ɡA���s�p�⪱�a����e��m
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

        // �ͦ� ImpactCircle
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

        // �]�m��m�M����
        vfx.transform.position = missileLaunchPosition != null ? missileLaunchPosition.position : launchPosition.position;
        vfx.transform.rotation = missileLaunchPosition != null ? missileLaunchPosition.rotation : launchPosition.rotation;

        // ���� Visual Effect
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
            StartCoroutine(DelayedDeactivationMissileVFX(vfx, 1f)); // ���� 1 ��H�T�O�ɤl����
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
        yield return new WaitForSeconds(duration); // �����]�m 5 ��
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
        bool useRightSpawnPoint = true; // �q�k���}�l�o�g

        // �Ω�O���w�ͦ����ۼu�I��m
        List<Vector3> usedPositions = new List<Vector3>();
        int maxPlacementAttempts = 50; // �̤j���զ��ơA�קK�L���`��

        // �ھ� lowHealth �ʺA��ܰѼ�
        int currentMiniMissileCount = lowHealth ? lowHealthMiniMissileCount : miniMissileCount;
        float currentMiniMissileLaunchDelay = lowHealth ? lowHealthMiniMissileLaunchDelay : miniMissileLaunchDelay;
        float currentInnerCircleRatio = lowHealth ? lowHealthInnerCircleRatio : innerCircleRatio;
        float currentInnerConcentrationFactor = lowHealth ? lowHealthInnerConcentrationFactor : innerConcentrationFactor;

        // �ʺA�վ�̤p���Z
        float adjustedMinDistance = Mathf.Min(minDistanceBetweenCircles, randomRadius / Mathf.Sqrt(currentMiniMissileCount));
        Debug.Log($"Adjusted Min Distance: {adjustedMinDistance} (Original: {minDistanceBetweenCircles}, Random Radius: {randomRadius}, Missile Count: {currentMiniMissileCount})");

        // �p�⤺��M�~�骺�ۼu�I�ƶq
        int innerCircleCount = Mathf.RoundToInt(currentMiniMissileCount * currentInnerCircleRatio); // ����ۼu�I�ƶq
        int outerCircleCount = currentMiniMissileCount - innerCircleCount; // �~��ۼu�I�ƶq
        float innerRadius = randomRadius * innerRadiusFactor; // ����b�|

        Debug.Log($"Inner Circle Count: {innerCircleCount}, Outer Circle Count: {outerCircleCount}, Inner Radius: {innerRadius}");

        // ���ͦ����骺�ۼu�I
        for (int i = 0; i < innerCircleCount; i++)
        {
            // ��ܷ�e���o�g�I
            Transform selectedSpawnPoint = useRightSpawnPoint ? rightMiniMissileSpawnPoint : leftMiniMissileSpawnPoint;

            // �p�⪱�a����e��m
            Vector3 playerPosition = player.position;

            // �H���ͦ��@�ӵ۳��I�]����^�A�T�O�����|
            Vector3 targetPosition = Vector3.zero;
            bool positionFound = false;
            int attempts = 0;

            while (!positionFound && attempts < maxPlacementAttempts)
            {
                // �H���ͦ��@�Ө��שM�Z���A�ϥΥ���ڤ��G�ϵۼu�I�󶰤�
                float randomAngle = Random.Range(0f, 360f);
                float randomValue = Random.Range(0f, 1f);
                float randomDistance = innerRadius * Mathf.Sqrt(randomValue) * (1f - currentInnerConcentrationFactor) + innerRadius * currentInnerConcentrationFactor * randomValue;

                Vector3 offset = new Vector3(
                    Mathf.Cos(randomAngle * Mathf.Deg2Rad) * randomDistance,
                    0f,
                    Mathf.Sin(randomAngle * Mathf.Deg2Rad) * randomDistance
                );
                targetPosition = playerPosition + offset;

                // �T�O�۳��I�b�a���W
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

                // �ˬd�P�w���ۼu�I���Z��
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

            // �p�G�L�k��줣���|����m�A�ϥγ̫�@�����ժ���m�]�קK�L���`���^
            if (!positionFound)
            {
                Debug.LogWarning($"Could not find non-overlapping position for Inner MiniImpactCircle {i + 1} after {maxPlacementAttempts} attempts. Using last position.");
            }

            // �O����e�ۼu�I��m
            usedPositions.Add(targetPosition);

            // �Ұʤ@�ӿW�ߪ���{�ӳB�z MiniMissileSwarm VFX �M MiniImpactCircle
            StartCoroutine(HandleMiniMissileLaunch(selectedSpawnPoint, targetPosition, beatsToExpand));

            // ���ݰʺA�p�⪺ miniMissileLaunchDelay ��o�g�U�@�Ӥp���ɼu
            yield return new WaitForSeconds(currentMiniMissileLaunchDelay);

            // ����ϥΥ��k�o�g�I
            useRightSpawnPoint = !useRightSpawnPoint;
        }



        // ����ͦ��ᵥ�ݤ@�q�ɶ�
        float waitSeconds = lowHealth ? innerOuterGap : lowHealthInnerOuterGap;
        yield return new WaitForSeconds(waitSeconds); // ����M�~�餧�����ɶ��t


        // �A�ͦ��~�骺�ۼu�I
        for (int i = 0; i < outerCircleCount; i++)
        {
            // ��ܷ�e���o�g�I
            Transform selectedSpawnPoint = useRightSpawnPoint ? rightMiniMissileSpawnPoint : leftMiniMissileSpawnPoint;

            // �p�⪱�a����e��m
            Vector3 playerPosition = player.position;

            // �H���ͦ��@�ӵ۳��I�]�~��^�A�T�O�����|
            Vector3 targetPosition = Vector3.zero;
            bool positionFound = false;
            int attempts = 0;

            while (!positionFound && attempts < maxPlacementAttempts)
            {
                // �H���ͦ��@�Ө��שM�Z���A�ϥΥ���ڤ��G
                float randomAngle = Random.Range(0f, 360f);
                float randomValue = Random.Range(0f, 1f);
                float randomDistance = randomRadius * Mathf.Sqrt(randomValue) * (1f - outerConcentrationFactor) + randomRadius * outerConcentrationFactor * randomValue;

                // �T�O�~�骺�I���|�i�J����]����̤p�Z���^
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

                // �T�O�۳��I�b�a���W
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

                // �ˬd�P�w���ۼu�I���Z��
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

            // �p�G�L�k��줣���|����m�A�ϥγ̫�@�����ժ���m�]�קK�L���`���^
            if (!positionFound)
            {
                Debug.LogWarning($"Could not find non-overlapping position for Outer MiniImpactCircle {i + 1} after {maxPlacementAttempts} attempts. Using last position.");
            }

            // �O����e�ۼu�I��m
            usedPositions.Add(targetPosition);

            // �Ұʤ@�ӿW�ߪ���{�ӳB�z MiniMissileSwarm VFX �M MiniImpactCircle
            StartCoroutine(HandleMiniMissileLaunch(selectedSpawnPoint, targetPosition, beatsToExpand));

            // ���ݰʺA�p�⪺ miniMissileLaunchDelay ��o�g�U�@�Ӥp���ɼu
            yield return new WaitForSeconds(currentMiniMissileLaunchDelay);

            // ����ϥΥ��k�o�g�I
            useRightSpawnPoint = !useRightSpawnPoint;
        }

        isAttack = false;
        Invoke("ResetAttack", timeBetweenAttacks + 5);
    }


    private IEnumerator HandleMiniMissileLaunch(Transform spawnPoint, Vector3 targetPosition, int beatsToExpand)
    {
        // ���� MiniMissileSwarm VFX�]�ϥηs�� miniMissileVFXPool�^
        GameObject miniMissileVFX = GetMiniMissileLaunchVFXFromPool(spawnPoint);
        if (miniMissileVFX != null)
        {
            yield return new WaitForSeconds(miniMissileVFXDuration); // ���� VFX ���񧹦��]3 ��^
            ReturnMiniMissileSwarmVFXToPool(miniMissileVFX);
        }

        // �ͦ� MiniImpactCircle
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

        // �]�m��m�M����
        vfx.transform.position = launchPosition.position;
        vfx.transform.rotation = launchPosition.rotation;

        // ���� Visual Effect
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
            StartCoroutine(DelayedDeactivationMiniMissileSwarmVFX(vfx, 1f)); // ���� 1 ��H�T�O�ɤl����
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
        yield return new WaitForSeconds(duration); // �����]�m�� miniExplosionVFXDuration
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
        // �q��H��������l�u
        GameObject bullet = BulletPoolManager.Instance.GetEnemyBullet();
        bullet.SetActive(true);
        bullet.transform.position = barrel.position;

        // �p��¦V���a����V
        Vector3 directionToPlayer = (player.position - barrel.position).normalized;
        bullet.transform.rotation = Quaternion.LookRotation(directionToPlayer);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(directionToPlayer * shotPower); // �ϥέp��X����V�A�Ӥ��O barrel.forward
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
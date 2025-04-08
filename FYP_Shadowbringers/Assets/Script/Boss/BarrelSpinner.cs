using UnityEngine;
using System.Collections;

public class BarrelSpinner : MonoBehaviour
{
    [SerializeField] private float spinUpTime = 1f; // Spin up 時間
    [SerializeField] private float spinDownTime = 1f; // Spin down 時間
    [SerializeField] private float maxSpinSpeed = 360f; // 最大旋轉速度（度/秒）
    [SerializeField] private AudioSource audioSource; // 用於播放 Spin Up、Spin Loop 和 Wind Down 音效
    [SerializeField] private AudioClip spinUpSound; // Spin Up 音效
    [SerializeField] private AudioClip spinLoopSound; // Spin Loop 音效
    [SerializeField] private AudioClip windDownSound; // Wind Down 音效

    private enum SpinnerState { Idle, SpinningUp, Spinning, SpinningDown }
    private SpinnerState currentState = SpinnerState.Idle;
    private float currentSpinSpeed = 0f;
    private Coroutine spinCoroutine;

    public void StartSpinning()
    {
        if (currentState == SpinnerState.Idle || currentState == SpinnerState.SpinningDown)
        {
            if (spinCoroutine != null)
            {
                StopCoroutine(spinCoroutine);
            }
            spinCoroutine = StartCoroutine(SpinUp());
        }
    }

    public void StopSpinning()
    {
        //Debug.Log("StopSpinning called at: " + Time.time);
        if (currentState == SpinnerState.Spinning || currentState == SpinnerState.SpinningUp)
        {
            if (spinCoroutine != null)
            {
                StopCoroutine(spinCoroutine);
            }
            spinCoroutine = StartCoroutine(SpinDown());
        }
    }

    public IEnumerator WaitForSpinUp()
    {
        yield return new WaitForSeconds(spinUpTime);
    }

    private IEnumerator SpinUp()
    {
        //Debug.Log("BarrelSpinner: Starting Spin Up");
        currentState = SpinnerState.SpinningUp;

        // 播放 Spin Up 音效
        if (audioSource != null && spinUpSound != null)
        {
            audioSource.clip = spinUpSound;
            audioSource.loop = false;
            audioSource.Play();
            //Debug.Log("Playing SpinUpSound. Duration: " + spinUpSound.length);
        }

        float elapsedTime = 0f;
        bool hasPlayedSpinLoop = false; // 標記是否已經播放 SpinLoopSound

        while (elapsedTime < spinUpTime)
        {
            elapsedTime += Time.deltaTime;
            currentSpinSpeed = Mathf.Lerp(0f, maxSpinSpeed, elapsedTime / spinUpTime);
            transform.Rotate(Vector3.up, currentSpinSpeed * Time.deltaTime);

            // 檢查 SpinUpSound 是否播放完畢
            if (audioSource != null && spinUpSound != null && !hasPlayedSpinLoop)
            {
                if (!audioSource.isPlaying || elapsedTime >= spinUpSound.length)
                {
                    // SpinUpSound 播放完畢，立即播放 SpinLoopSound
                    if (spinLoopSound != null)
                    {
                        audioSource.clip = spinLoopSound;
                        audioSource.loop = true;
                        audioSource.Play();
                        //Debug.Log("SpinUpSound finished. Playing SpinLoopSound.");
                        hasPlayedSpinLoop = true;
                    }
                }
            }

            yield return null;
        }
        currentSpinSpeed = maxSpinSpeed;

        // 確保進入 Spinning 狀態
        //Debug.Log("BarrelSpinner: Entering Spinning State");
        currentState = SpinnerState.Spinning;

        // 如果 SpinLoopSound 還未播放（例如 SpinUpSound 比 spinUpTime 長），則在此播放
        if (!hasPlayedSpinLoop && audioSource != null && spinLoopSound != null)
        {
            audioSource.clip = spinLoopSound;
            audioSource.loop = true;
            audioSource.Play();
            //Debug.Log("Playing SpinLoopSound after SpinUpTime.");
        }
    }

    private IEnumerator SpinDown()
    {
        //Debug.Log("BarrelSpinner: Starting Spin Down");
        currentState = SpinnerState.SpinningDown;

        // 停止 SpinLoopSound 並立即播放 WindDownSound
        if (audioSource != null)
        {
            // 停止當前音效（SpinLoopSound）
            audioSource.Stop();
            //Debug.Log("SpinLoopSound stopped.");

            // 立即播放 WindDownSound
            if (windDownSound != null)
            {
                audioSource.clip = windDownSound;
                audioSource.loop = false;
                audioSource.Play();
                //Debug.Log("Playing WindDownSound. Duration: " + windDownSound.length);
                //Debug.Log("AudioSource isPlaying: " + audioSource.isPlaying);
            }
            else
            {
                Debug.LogWarning("WindDownSound is null!");
            }
        }
        else
        {
            Debug.LogWarning("AudioSource is null!");
        }

        float elapsedTime = 0f;
        while (elapsedTime < spinDownTime)
        {
            elapsedTime += Time.deltaTime;
            currentSpinSpeed = Mathf.Lerp(maxSpinSpeed, 0f, elapsedTime / spinDownTime);
            transform.Rotate(Vector3.up, currentSpinSpeed * Time.deltaTime);
            yield return null;
        }
        currentSpinSpeed = 0f;

        // 確保 WindDownSound 播放完畢
        if (audioSource != null && windDownSound != null && audioSource.isPlaying)
        {
            yield return new WaitForSeconds(windDownSound.length - elapsedTime);
        }

        //Debug.Log("BarrelSpinner: Entering Idle State");
        currentState = SpinnerState.Idle;
    }

    private void Update()
    {
        if (currentState == SpinnerState.Spinning || currentState == SpinnerState.SpinningUp || currentState == SpinnerState.SpinningDown)
        {
            transform.Rotate(Vector3.up, currentSpinSpeed * Time.deltaTime);
        }
    }
}
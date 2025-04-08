using UnityEngine;
using System.Collections;

public class BarrelSpinner : MonoBehaviour
{
    [SerializeField] private float spinUpTime = 1f; // Spin up �ɶ�
    [SerializeField] private float spinDownTime = 1f; // Spin down �ɶ�
    [SerializeField] private float maxSpinSpeed = 360f; // �̤j����t�ס]��/��^
    [SerializeField] private AudioSource audioSource; // �Ω󼽩� Spin Up�BSpin Loop �M Wind Down ����
    [SerializeField] private AudioClip spinUpSound; // Spin Up ����
    [SerializeField] private AudioClip spinLoopSound; // Spin Loop ����
    [SerializeField] private AudioClip windDownSound; // Wind Down ����

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

        // ���� Spin Up ����
        if (audioSource != null && spinUpSound != null)
        {
            audioSource.clip = spinUpSound;
            audioSource.loop = false;
            audioSource.Play();
            //Debug.Log("Playing SpinUpSound. Duration: " + spinUpSound.length);
        }

        float elapsedTime = 0f;
        bool hasPlayedSpinLoop = false; // �аO�O�_�w�g���� SpinLoopSound

        while (elapsedTime < spinUpTime)
        {
            elapsedTime += Time.deltaTime;
            currentSpinSpeed = Mathf.Lerp(0f, maxSpinSpeed, elapsedTime / spinUpTime);
            transform.Rotate(Vector3.up, currentSpinSpeed * Time.deltaTime);

            // �ˬd SpinUpSound �O�_���񧹲�
            if (audioSource != null && spinUpSound != null && !hasPlayedSpinLoop)
            {
                if (!audioSource.isPlaying || elapsedTime >= spinUpSound.length)
                {
                    // SpinUpSound ���񧹲��A�ߧY���� SpinLoopSound
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

        // �T�O�i�J Spinning ���A
        //Debug.Log("BarrelSpinner: Entering Spinning State");
        currentState = SpinnerState.Spinning;

        // �p�G SpinLoopSound �٥�����]�Ҧp SpinUpSound �� spinUpTime ���^�A�h�b������
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

        // ���� SpinLoopSound �åߧY���� WindDownSound
        if (audioSource != null)
        {
            // �����e���ġ]SpinLoopSound�^
            audioSource.Stop();
            //Debug.Log("SpinLoopSound stopped.");

            // �ߧY���� WindDownSound
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

        // �T�O WindDownSound ���񧹲�
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
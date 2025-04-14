using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    [SerializeField] public float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;

    [Header("BeatChecker")]
    public bool inBeat;
    public float fadeOutBeat;

    public static BeatManager Instance;
    private bool isCheckingBeat = false; // 新增變量，防止重複啟動協程

    private void Awake()
    {
        MakeInstance();

        // 確保 AudioSource 不為 null
        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                Debug.LogError("AudioSource component is missing on BeatManager!", this);
            }
        }

        // 檢查 Intervals 數組
        if (_intervals == null || _intervals.Length == 0)
        {
            Debug.LogWarning("BeatManager: Intervals array is not set or empty!", this);
        }
    }

    void MakeInstance()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (_audioSource == null || _audioSource.clip == null || _intervals == null)
        {
            Debug.LogWarning("BeatManager: AudioSource, AudioClip, or Intervals array is not set!", this);
            return;
        }

        foreach (Intervals interval in _intervals)
        {
            float sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm)));
            interval.CheckForNewInterval(sampledTime);
        }
    }

    public void _CheckBeat()
    {
        if (!isCheckingBeat) // 確保不會重複啟動協程
        {
            StartCoroutine(CheckBeat());
        }
    }

    private IEnumerator CheckBeat()
    {
        isCheckingBeat = true;
        inBeat = true;
        //Debug.Log("Beat Start: inBeat = " + inBeat);
        yield return new WaitForSeconds(fadeOutBeat);
        inBeat = false;
        //Debug.Log("Beat End: inBeat = " + inBeat);
        isCheckingBeat = false;
    }

    [System.Serializable]
    public class Intervals
    {
        [SerializeField] private float _steps;
        [SerializeField] private UnityEvent _trigger;
        private int _lastInterval;
        public bool inBeat;

        public float GetIntervalLength(float bpm)
        {
            return 60f / (bpm * _steps);
        }

        public void CheckForNewInterval(float interval)
        {
            if (Mathf.FloorToInt(interval) != _lastInterval)
            {
                _lastInterval = Mathf.FloorToInt(interval);
                if (_trigger != null)
                {
                    _trigger.Invoke();
                }
                else
                {
                    Debug.LogWarning("BeatManager: _trigger is null in CheckForNewInterval!");
                }
            }
        }
    }
}
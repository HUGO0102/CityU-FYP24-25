using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;

    [Header("BeatChecker")]
    public bool inBeat;
    public float fadeOutBeat;

    public static BeatManager Instance;

    private void Awake()
    {
        MakeInstance();
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
        foreach (Intervals interval in _intervals)
        {
            float sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * interval.GetIntervalLength(_bpm)));
            interval.CheckForNewInterval(sampledTime);
        }
    }

    public void _CheckBeat()
    {
        StartCoroutine(CheckBeat());
    }

    private IEnumerator CheckBeat()
    {
        inBeat = true;

        //Debug.Log(inBeat);

        yield return new WaitForSeconds(fadeOutBeat);

        inBeat = false;

        //Debug.Log(inBeat);
    }
 
    [System.Serializable]
    public class Intervals
    {
        // 1 step means every beat
        // 0.5 step mean every 2 beat
        // 0.25 step mean every 4 beat
        [SerializeField] private float _steps;
        [SerializeField] private UnityEvent _trigger;
        private int _lastInterval;
        public bool inBeat;

        public float GetIntervalLength(float bpm)
        {
            return 60f / (bpm * _steps);
        }

        public void CheckForNewInterval (float interval)
        {
            if (Mathf.FloorToInt(interval) != _lastInterval)
            {
                _lastInterval = Mathf.FloorToInt(interval);
                _trigger.Invoke();
            }
        }
    }
}

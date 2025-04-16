using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatVisualizer : MonoBehaviour
{
    [SerializeField] private BeatManager _beatManager;
    [SerializeField] private GameObject _beatBarPrefab;
    [SerializeField] private Transform _leftSpawnPoint;
    [SerializeField] private Transform _rightSpawnPoint;

    [SerializeField] private Transform _centerPoint; // The center position for beat bars
    [SerializeField] private Canvas canvas;

    public TriggerController triggerControl;

    private bool barSpawned = false;
    private void Start()
    {
        _beatManager._CheckBeat();
    }

    private void Update()
    {
        if (_beatManager.inBeat && !barSpawned && triggerControl.beatStart == true)
        {
            SpawnBar(_leftSpawnPoint.position, Vector3.right, false);
            SpawnBar(_rightSpawnPoint.position, Vector3.left, true);
            barSpawned = true;
        }
        else if (!_beatManager.inBeat)
        {
            barSpawned = false;
        }        
    }

    private void SpawnBar(Vector3 position, Vector3 direction, bool flip)
    {
        GameObject bar = Instantiate(_beatBarPrefab, position, Quaternion.identity, canvas.transform);
        BeatBar_fadeout fadeoutCode = bar.GetComponent<BeatBar_fadeout>();

        if (flip)
        {
            Vector3 scale = bar.transform.localScale;
            scale.x *= -1; // Flip horizontally
            bar.transform.localScale = scale;

            bar.tag = "LeftBeatBar";
        }
        else
        {
            bar.tag = "RightBeatBar";
        }

        float distanceToCenter = Vector3.Distance(position, _centerPoint.position);

        //StartCoroutine(MoveAndDestroyBar(bar, direction));
        fadeoutCode.BarMovement(direction, distanceToCenter);
    }

    /*
    private IEnumerator MoveAndDestroyBar(GameObject bar, Vector3 direction)
    {
        float elapsedTime = 0f;
        while (elapsedTime < _barLifetime)
        {
            Debug.Log(_moveSpeed);
            bar.transform.Translate(direction * _moveSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Destroy(bar);
    }
    */
}
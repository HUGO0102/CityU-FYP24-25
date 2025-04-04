using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject[] TargetPrefab;

    public GameObject Target;

    public Vector3 spawnValues;
    public float spawnWait;
    public float spawnMostWait;
    public float spawnLeastWait;
    public int startWait;
    public bool stop;

    int randTarget;

    void Start()
    {
        StartCoroutine(waitSpawner());
    }

    void Update()
    {
        spawnWait = Random.Range(spawnLeastWait, spawnMostWait);
    }

    IEnumerator waitSpawner()
    {
        yield return new WaitForSeconds(startWait);

        while (!stop)
        {
            randTarget = Random.Range(0, 1);

            Vector3 spawnPosition = new Vector3(Random.Range(-spawnValues.x, spawnValues.x), Random.Range(-spawnValues.y, spawnValues.y), Random.Range(-spawnValues.z, spawnValues.z));

            Target = Instantiate(TargetPrefab[randTarget], spawnPosition + transform.TransformPoint(0, 0, 0), gameObject.transform.rotation);

            Destroy(Target, 10);


            yield return new WaitForSeconds(spawnWait);
        }
    }
}
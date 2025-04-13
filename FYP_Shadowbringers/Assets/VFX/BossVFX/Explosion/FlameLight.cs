using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameLight : MonoBehaviour
{
    private Light _light;
    public float multiplier = 1000f;
    public float fadeSpeed = 50;


    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        _light.intensity = Random.Range(1,5) * multiplier;
        multiplier -= fadeSpeed * Time.deltaTime;
    }
}

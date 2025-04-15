using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager_Pakour : MonoBehaviour
{
    public Animator EyeBlink;
    public Animation OxygenTank;
    public Animation LookAround;
    private bool inCutscene;

    // Start is called before the first frame update
    void Start()
    {
        EyeBlink.SetTrigger("isBlink");
        OxygenTank.Play();
        StartCoroutine("Cutscene");
    }

    private IEnumerator Cutscene()
    {
        inCutscene = true;
        yield return new WaitForSeconds(7.5f);

        inCutscene = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SceneManager_Pakour : MonoBehaviour
{
    public PlayableDirector WakeUpCutscene;
    //public GameObject controlPanel;
    private bool inCutscene;

    // Start is called before the first frame update
    private void Awake()
    {
        //WakeUpCutscene = GetComponent<PlayableDirector>();
        StartCoroutine("Cutscene");
    }

    private void Start()
    {
        WakeUpCutscene.Play();
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

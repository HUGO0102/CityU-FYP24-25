using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] Animator transitionAnim;
    public GameObject BlackScreen;
    public string NextScene;

    // Start is called before the first frame update
    public void NextLevel()
    {
        StartCoroutine(LoadNextScene());
    }
    IEnumerator LoadNextScene()
    {
        gameObject.SetActive(true);
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(NextScene);
        transitionAnim.SetTrigger("Start");
    }

    private void OnTriggerEnter(Collider other)
    {
        NextLevel();
    }
}

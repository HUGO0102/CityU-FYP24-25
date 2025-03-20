using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Manager")]
    [SerializeField] Animator transitionAnim;
    public string MainGame;
    
    public void NextLevel()
    {
        StartCoroutine(LoadScene());
    }
    IEnumerator LoadScene()
    {
        transitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(MainGame);
        transitionAnim.SetTrigger("Start");
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void ButtonClick(string sfxName)
    {
        SFXManager.Instance.PlaySFX(sfxName);
    }
}

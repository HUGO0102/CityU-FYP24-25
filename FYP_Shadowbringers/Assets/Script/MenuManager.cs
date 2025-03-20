using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Manager")]
    public string MainGame;
    
    public void LoadScene()
    {
        SceneManager.LoadScene(MainGame);
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

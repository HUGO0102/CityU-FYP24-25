using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour, ISelectHandler
{
    public string _sfxName;
    public void OnSelect(BaseEventData eventData)
    {
        ButtonClick(_sfxName);
    }

    public void ButtonClick(string sfxName)
    {
        SFXManager.Instance.PlaySFX(sfxName);
    }
}

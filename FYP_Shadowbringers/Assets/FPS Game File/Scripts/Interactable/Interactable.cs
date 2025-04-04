using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    //Add or remove an InterActionEvent component to this gameobject.
    public bool useEvents;
    [SerializeField]
    public string promptMessage;

    public virtual string Onlook()
    {
        return promptMessage;
    }
    public void BaseInteract()
    {
        if (useEvents)
            GetComponent<InterractionEvent>().OnInteract.Invoke();
        Interact();
    }

    protected virtual void Interact()
    {
        //we wont have an code written in this function
        //this is a template function to be overridden by our subclasses
    }
}

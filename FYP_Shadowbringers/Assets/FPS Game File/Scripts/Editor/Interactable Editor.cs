using UnityEditor;

[CustomEditor(typeof(Interactable), true)]
public class InteractableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Interactable interactable = (Interactable)target;
        if (target.GetType() == typeof(EventOnlyInteractable))
        {
            interactable.promptMessage = EditorGUILayout.TextField("Prompt Message", interactable.promptMessage);
            EditorGUILayout.HelpBox("EventOnlyInteract can ONLY use UnityEvents", MessageType.Info);
            if (interactable.GetComponent<InterractionEvent>() == null)
            {
                interactable.useEvents = true;
                interactable.gameObject.AddComponent<InterractionEvent>();
            }

        }
        else
        {
            base.OnInspectorGUI();
            if (interactable.useEvents)
            {
                //we are using events. add the component
                if (interactable.GetComponent<InterractionEvent>() == null)
                    interactable.gameObject.AddComponent<InterractionEvent>();
            }
            else
            {
                //we are not using events. remove the component.
                if (interactable.GetComponent<InterractionEvent>() != null)
                    DestroyImmediate(interactable.GetComponent<InterractionEvent>());
            }
        }
    }
}

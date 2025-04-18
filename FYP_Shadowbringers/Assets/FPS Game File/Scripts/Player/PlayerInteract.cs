using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float distance = 3f; // �`�N�ץ����g�Gdistence -> distance
    [SerializeField]
    private LayerMask mask;
    private PlayerUI playerUI;
    private InputManager inputManager;

    void Start()
    {
        cam = GetComponent<PlayerLook>().cam;
        playerUI = GetComponent<PlayerUI>();
        inputManager = GetComponent<InputManager>();
    }

    void Update()
    {
        playerUI.UpdateText(string.Empty);
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * distance);

        // �ϥ� Physics.RaycastAll �˴��Ҧ��I����A�]�AĲ�o��
        RaycastHit[] hits = Physics.RaycastAll(ray, distance, mask);

        // �M���Ҧ��������I����
        foreach (RaycastHit hitInfo in hits)
        {
            Interactable interactable = hitInfo.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                playerUI.UpdateText(interactable.promptMessage);
                if (inputManager.onFoot.Interact.triggered)
                {
                    Debug.Log("E_Pressing");
                    interactable.BaseInteract();
                }
                break; // ���Ĥ@�ӥi�椬��H��h�X�`���]�i�ھڻݨD�վ�^
            }
        }
    }
}
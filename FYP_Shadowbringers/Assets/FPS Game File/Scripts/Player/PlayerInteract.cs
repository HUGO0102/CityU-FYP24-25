using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    private Camera cam;
    [SerializeField]
    private float distance = 3f; // 注意修正拼寫：distence -> distance
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

        // 使用 Physics.RaycastAll 檢測所有碰撞體，包括觸發器
        RaycastHit[] hits = Physics.RaycastAll(ray, distance, mask);

        // 遍歷所有擊中的碰撞體
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
                break; // 找到第一個可交互對象後退出循環（可根據需求調整）
            }
        }
    }
}
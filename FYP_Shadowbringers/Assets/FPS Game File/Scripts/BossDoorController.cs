using UnityEngine;

public class BossDoorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Boss_Ai bossAi; // 引用 Boss_Ai 腳本
    [SerializeField] private GameObject doorWall;
    [SerializeField] private GameObject BoxWall;// 引用門對象（BOSS_DoorWall）

    private bool isDoorActive = false; // 記錄門是否已激活

    private void Start()
    {
        // 檢查引用是否正確分配
        if (bossAi == null)
        {
            Debug.LogError("Boss_Ai reference is not assigned in BossDoorController!");
            return;
        }

        if (doorWall == null)
        {
            Debug.LogError("DoorWall reference is not assigned in BossDoorController!");
            return;
        }

        // 初始關閉門
        doorWall.SetActive(false);
        BoxWall.SetActive(true);
        isDoorActive = false;
    }

    private void Update()
    {
        // 檢查玩家是否觸發 Boss
        if (!isDoorActive && bossAi.playerInSightRange)
        {
            // 玩家進入 Boss 的視野範圍，激活門
            doorWall.SetActive(true);
            BoxWall.SetActive(false);
            isDoorActive = true;
            Debug.Log("Boss Door activated: Player triggered the boss!");
        }

        // 檢查 Boss 是否死亡
        if (isDoorActive && bossAi.isDead)
        {
            // Boss 死亡，關閉門
            doorWall.SetActive(false);
            BoxWall.SetActive(false);
            isDoorActive = false;
            Debug.Log("Boss Door deactivated: Boss is dead!");
        }
    }
}
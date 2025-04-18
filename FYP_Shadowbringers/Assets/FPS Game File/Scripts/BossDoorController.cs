using UnityEngine;

public class BossDoorController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Boss_Ai bossAi; // �ޥ� Boss_Ai �}��
    [SerializeField] private GameObject doorWall;
    [SerializeField] private GameObject BoxWall;// �ޥΪ���H�]BOSS_DoorWall�^

    private bool isDoorActive = false; // �O�����O�_�w�E��

    private void Start()
    {
        // �ˬd�ޥάO�_���T���t
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

        // ��l������
        doorWall.SetActive(false);
        BoxWall.SetActive(true);
        isDoorActive = false;
    }

    private void Update()
    {
        // �ˬd���a�O�_Ĳ�o Boss
        if (!isDoorActive && bossAi.playerInSightRange)
        {
            // ���a�i�J Boss �������d��A�E����
            doorWall.SetActive(true);
            BoxWall.SetActive(false);
            isDoorActive = true;
            Debug.Log("Boss Door activated: Player triggered the boss!");
        }

        // �ˬd Boss �O�_���`
        if (isDoorActive && bossAi.isDead)
        {
            // Boss ���`�A������
            doorWall.SetActive(false);
            BoxWall.SetActive(false);
            isDoorActive = false;
            Debug.Log("Boss Door deactivated: Boss is dead!");
        }
    }
}
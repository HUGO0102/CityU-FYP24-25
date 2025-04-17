using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public BoxCollider Collider01;
    public BoxCollider Collider02;
    public BoxCollider Collider03;
    public BoxCollider Collider04;
    public BoxCollider Collider05;
    public BoxCollider Collider06;
    public EnemyManager enemyController;
    //public EquipScript equip;
    public int triggerIndex;
    public GameObject beatUI;

    //public bool beatStart = false;

    /*[Header("Trigger")]
    public bool isDoorTrigger01;
    public bool isDoorTrigger02;
    public bool isEnemyTriggerTUT;
    public bool isItemTrigger;*/

    [Header("Animation")]
    public Animator Door01;
    public Animator Gate01;
    public Animator Gate02;

    [Header("GameObject")]
    public GameObject AttackTips;
    public GameObject BattleArea01;
    public GameObject BattleArea02;
    public GameObject BattleArea03;
    public GameObject BattleArea04;
    public GameObject BattleArea05;
    public GameObject BattleArea06;

    public bool canSpawn1;
    public bool canSpawn2;
    public bool canSpawn4;
    // Start is called before the first frame update
    void Start()
    {
        beatUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerIndex == 0)
            {
                Door01.SetBool("Open", true);
            }

            /*if (triggerIndex == 1)
            {
                //Door02.SetBool();
            }*/

            if (triggerIndex == 1)
            {
                beatUI.SetActive(true);
                AttackTips.SetActive(true);

                canSpawn1 = true;
                BattleArea01.SetActive(true);
                BattleArea02.SetActive(true);
            }

            if (triggerIndex == 2)
            {
                BattleArea03.SetActive(true);
                BattleArea04.SetActive(true);
                canSpawn2 = true;
            }

            if (triggerIndex == 3)
            {
                Gate01.SetBool("Open", true);
            }

            if (triggerIndex == 4)
            {
                Gate02.SetBool("Open", true);

                //canSpawn4 = true;
            }

            if (triggerIndex == 5)
            {
                Gate02.SetBool("Open", true);
                BattleArea05.SetActive(true);
                BattleArea06.SetActive(true);
                canSpawn4 = true;
            }

            if (triggerIndex == 6)
            {
                
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (triggerIndex == 0)
            {
                Door01.SetBool("Open", false);
            }

            if (triggerIndex == 3)
            {
                Gate01.SetBool("Open", false);
            }

            if (triggerIndex == 4)
            {
                Gate02.SetBool("Open", false);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerController : MonoBehaviour
{
    public EnemyManager enemyController;
    public int triggerIndex;

    public bool beatStart = false;

    /*[Header("Trigger")]
    public bool isDoorTrigger01;
    public bool isDoorTrigger02;
    public bool isEnemyTriggerTUT;
    public bool isItemTrigger;*/

    [Header("Animation")]
    public Animator Door01;
    public Animator Gate01;

    [Header("GameObject")]
    public GameObject AttackTips;
    public GameObject BattleArea01;
    public GameObject BattleArea02;
    public GameObject BattleArea03;
    public GameObject BattleArea04;

    // Start is called before the first frame update
    void Start()
    {
        
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
                beatStart = true;
                AttackTips.SetActive(true);
                //BattleArea01.SetActive(true);
                //BattleArea02.SetActive(true);
            }

            if (triggerIndex == 2)
            {
                BattleArea03.SetActive(true);
                BattleArea04.SetActive(true);
            }

            if (triggerIndex == 3)
            {
                Gate01.SetBool("Open", true);
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
        }
    }
}

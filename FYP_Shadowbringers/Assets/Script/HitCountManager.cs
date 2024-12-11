using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitCountManager : MonoBehaviour
{
    [Header("Text")]
    public TMP_Text Hit_Combo_Text;

    [Header("Bool")]
    public bool Hited;
    [Header("CountDown Bool")]
    public bool start_Count;

    [Header("Animation")]
    public Animator Anim;

    public float Hit_Num;
    public float Hit_Time_Left = 5f;

    public static HitCountManager Instance;

    void Awake()
    {
        MakeInstance();
    }

    void MakeInstance()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Hit_Combo_Text.enabled = false;
        Hited = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Hited != false)
        {
            start_Count = true;
            StartCountDown();


            if (Hit_Time_Left < 1)
            {
                Hit_Num = 0;
                Hit_Combo_Text.enabled = false;
                Hit_Time_Left = 5f;
                Hited = false;
                start_Count = false;
            }

            if (Hit_Time_Left < 3)
            {
                Anim.Play("Hit_Fade_Out");
            }
        }
    }

    private void FixedUpdate()
    {
        Hit_Combo_Text.text = Hit_Num + " Hit";
    }

    private void StartCountDown()
    {
        if (start_Count == true)
        {
            Hit_Time_Left -= Time.deltaTime;
        }
    }

    public void Hit_plus()
    {
        Hit_Num += 1;
        Anim.Play("Hit_Text");

        Hit_Combo_Text.enabled = true;
        Hited = true;

        Hit_Time_Left = 5f;
    }
}

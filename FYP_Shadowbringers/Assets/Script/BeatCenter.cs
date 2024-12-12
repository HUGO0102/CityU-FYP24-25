using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatCenter : MonoBehaviour
{
    public bool leftBarInCenter;
    public bool rightBarInCenter;

    public GameObject CurrentHitedLeft;
    public GameObject CurrentHitedRight;

    public bool HitInBeat = false;

    public static BeatCenter Instance;

    private void Awake()
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("LeftBeatBar"))
        {
            leftBarInCenter = true;
            CurrentHitedLeft = other.gameObject;
        }
        else if (other.CompareTag("RightBeatBar"))
        {
            rightBarInCenter = true;
            CurrentHitedRight = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("LeftBeatBar"))
        {
            leftBarInCenter = false;
        }
        else if (other.CompareTag("RightBeatBar"))
        {
            rightBarInCenter = false;
        }
    }

    public void HitOnBeat()
    {
        if(CurrentHitedLeft != null)
            ChangeHittedImage(CurrentHitedLeft);

        if (CurrentHitedRight != null)
            ChangeHittedImage(CurrentHitedRight);

        AttackHitInBeat();
    }

    private void ChangeHittedImage(GameObject HitedObj)
    {
        HitedObj.GetComponent<BoxCollider2D>().enabled = false;
        Image HitedImg = HitedObj.GetComponent<Image>();
        HitedImg.color = Color.red;
    }

    public void AttackHitInBeat()
    {
        HitInBeat = true;
        StartCoroutine(ResetHit());
    }

    private IEnumerator ResetHit()
    {
        yield return new WaitForSeconds(1f);
        HitInBeat = false;
        //Debug.Log("reset beat!");
    }
}

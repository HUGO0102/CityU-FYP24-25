using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatBar_fadeout : MonoBehaviour
{
    private bool barTouched;
    private Animator barAnim;
    private Vector3 moveDirection;
    private float moveSpeed;            
    private float beatDuration;

    private void Start()
    {
        barAnim = gameObject.GetComponent<Animator>();

        beatDuration = 60f / BeatManager.Instance._bpm;

        //elapsedTime = 0f;
    }
    private void Update()
    {
        moveSpeed = 320f / beatDuration;
        if ( !barTouched)
        {
            float normalizedSpeed = moveSpeed * (Screen.width / 1920f);
            this.transform.Translate(moveDirection * normalizedSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        barTouched = true;
        barAnim.SetTrigger("Fade");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        barTouched = false;
    }
    
    public void BarMovement(Vector3 direction, float distanceToCenter)
    {
        moveDirection = direction;
        moveSpeed = distanceToCenter / beatDuration;
    }
    public void DestroyBar()
    {
        Destroy(gameObject);
    }
}

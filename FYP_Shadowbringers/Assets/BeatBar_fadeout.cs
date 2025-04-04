using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatBar_fadeout : MonoBehaviour
{
    private bool barTouched;
    private Animator barAnim;
    private Vector3 moveDirection;
    private float moveSpeed;
    private float _moveSpeed;
    private float bpm;               // Beats Per Minute of the song
    private float beatDuration;      // Duration of a single beat (seconds)

    private void Start()
    {
        barAnim = gameObject.GetComponent<Animator>();
        // Set the default BPM (you can adjust this in the inspector or dynamically)
        bpm = 120f;
        beatDuration = 60f / bpm;

        //elapsedTime = 0f;
    }
    private void Update()
    {
        moveSpeed = 160f / beatDuration;
        if ( !barTouched)
        {
            this.transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
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
    
    public void BarMovement(Vector3 direction, float _moveSpeed)
    {
        moveDirection = direction;
        moveSpeed = _moveSpeed;
    }
    public void DestroyBar()
    {
        Destroy(gameObject);
    }
}

using UnityEngine;

public class BarrelSpinner : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 360f; // ����t�ס]��/��^
    private bool isSpinning = false;

    public void StartSpinning()
    {
        isSpinning = true;
    }

    public void StopSpinning()
    {
        isSpinning = false;
    }

    private void Update()
    {
        if (isSpinning)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
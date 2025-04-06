using UnityEngine;

public class BarrelSpinner : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 360f; // 旋轉速度（度/秒）
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
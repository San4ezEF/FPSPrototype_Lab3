using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float bobAmplitude = 0.25f;
    public float bobFrequency = 1f;

    private Vector3 startPos;

    void Start() => startPos = transform.position;

    void Update()
    {
        // Вращение
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        // Движение вверх-вниз
        float newY = startPos.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
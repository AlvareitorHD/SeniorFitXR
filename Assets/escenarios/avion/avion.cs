using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    public Vector3 centerPoint = Vector3.zero;

    public float distance = 5f;
    public float minY = 1f;
    public float maxY = 3f;

    public float minSpeed = 2f;
    public float maxSpeed = 15f;

    public float lateralAmplitude = 1.5f;
    public float verticalAmplitude = 0.5f;

    private float currentAngle = 90f; // inicia en medio del arco
    private float angularSpeed = 45f;

    private float speedChangeTimer = 0f;
    private float speedChangeInterval = 3f;

    private Vector3 previousPosition;

    private float startDelay = 5f;

    public float tiltAmount = 20f;
    public float tiltSmoothness = 5f;

    private int direction = 1; // 1 para avanzar, -1 para retroceder

    private float verticalNoiseSeed;

    void Start()
    {
        previousPosition = transform.position;
        verticalNoiseSeed = Random.Range(0f, 100f);
    }

    void Update()
    {
        startDelay -= Time.deltaTime;
        if (startDelay > 0f) return;

        float deltaTime = Time.deltaTime;

        // Cambia velocidad aleatoriamente
        speedChangeTimer += deltaTime;
        if (speedChangeTimer >= speedChangeInterval)
        {
            speedChangeTimer = 0f;
            angularSpeed = Random.Range(minSpeed, maxSpeed);
            speedChangeInterval = Random.Range(2f, 5f);
        }

        // Movimiento dentro de 0° a 180°
        currentAngle += direction * angularSpeed * deltaTime;
        if (currentAngle >= 180f)
        {
            currentAngle = 180f;
            direction = -1;
        }
        else if (currentAngle <= 0f)
        {
            currentAngle = 0f;
            direction = 1;
        }

        // Posición circular
        Vector3 offset = Quaternion.Euler(0f, currentAngle - 90f, 0f) * Vector3.forward * distance;

        // Oscilación lateral
        float time = Time.time;
        offset += Vector3.right * Mathf.Sin(time * 1.5f) * lateralAmplitude;

        // Movimiento vertical suavizado
        float verticalNoise = Mathf.PerlinNoise(time * 0.3f, verticalNoiseSeed); // valor entre 0 y 1
        float verticalY = Mathf.Lerp(minY, maxY, verticalNoise); // mapea a minY-maxY
        offset.y = verticalY - centerPoint.y; // offset relativo a centerPoint

        // Posición y orientación
        Vector3 targetPosition = centerPoint + offset;
        transform.position = targetPosition;

        Vector3 movementDir = (transform.position - previousPosition).normalized;
        if (movementDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(movementDir, Vector3.up);

            // Inclinación lateral (eje Z) en curvas
            Vector3 toCenter = (centerPoint - transform.position).normalized;
            float turnSign = Vector3.SignedAngle(movementDir, toCenter, Vector3.up);
            float zTilt = Mathf.Clamp(-turnSign, -tiltAmount, tiltAmount);

            Quaternion tiltRotation = Quaternion.Euler(0, targetRot.eulerAngles.y, zTilt);
            transform.rotation = Quaternion.Slerp(transform.rotation, tiltRotation, tiltSmoothness * deltaTime);
        }

        previousPosition = transform.position;
    }
}

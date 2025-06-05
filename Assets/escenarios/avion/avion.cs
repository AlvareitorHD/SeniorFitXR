using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    public Vector3 centerPoint = Vector3.zero;

    public float distance = 5f;
    public float minY = 1f;
    public float maxY = 3.5f;

    public float minSpeed = 2f;
    public float maxSpeed = 90f;

    public float lateralAmplitude = 1.5f;
    public float verticalAmplitude = 0.5f;

    private float currentAngle = 0f;
    private float angularSpeed = 45f;

    private float speedChangeTimer = 0f;
    private float speedChangeInterval = 3f;

    private float verticalMoveTimer = 0f;
    private float verticalMoveDuration = 2f;
    private float verticalMoveInterval = 5f;
    private float verticalTargetOffset = 0f;

    private float verticalOffsetSeed;
    private float lateralOffsetSeed;

    private Vector3 previousPosition;

    private float startDelay = 10f;

    void Start()
    {
        previousPosition = transform.position;
        verticalOffsetSeed = Random.Range(0f, 100f);
        lateralOffsetSeed = Random.Range(0f, 100f);
    }

    void Update()
    {
        startDelay -= Time.deltaTime;
        if (startDelay > 0f)
            return;

        float deltaTime = Time.deltaTime;

        // Velocidad angular aleatoria
        speedChangeTimer += deltaTime;
        if (speedChangeTimer >= speedChangeInterval)
        {
            speedChangeTimer = 0f;
            angularSpeed = Random.Range(minSpeed, maxSpeed);
            speedChangeInterval = Random.Range(2f, 5f);
        }

        // Ángulo limitado a 180°
        currentAngle += angularSpeed * deltaTime;
        if (currentAngle > 180f) currentAngle -= 180f;

        // Dirección circular
        Vector3 offset = Quaternion.Euler(0f, currentAngle - 90f, 0f) * Vector3.forward * distance;

        // Oscilación lateral
        float time = Time.time;
        offset += Vector3.right * Mathf.Sin(time + lateralOffsetSeed) * lateralAmplitude;

        // Movimiento vertical ocasional
        verticalMoveTimer += deltaTime;
        if (verticalMoveTimer >= verticalMoveInterval)
        {
            verticalMoveTimer = 0f;
            verticalMoveInterval = Random.Range(4f, 8f); // próximo cambio
            verticalTargetOffset = Random.Range(-verticalAmplitude, verticalAmplitude);
        }

        float smoothVertical = Mathf.Lerp(0f, verticalTargetOffset, Mathf.PingPong(time, verticalMoveDuration) / verticalMoveDuration);
        float height = Mathf.Clamp(centerPoint.y + smoothVertical, minY, maxY);
        offset.y = height - centerPoint.y;

        // Posición y orientación
        Vector3 targetPosition = centerPoint + offset;
        transform.position = targetPosition;

        Vector3 movementDir = (transform.position - previousPosition).normalized;
        if (movementDir.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(movementDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 5f * deltaTime);
        }

        previousPosition = transform.position;
    }
}

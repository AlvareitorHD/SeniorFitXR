using UnityEngine;

public class AirplaneController : MonoBehaviour
{
    public Transform playerCamera;       // Cámara del jugador
    public float distance = 5f;          // Distancia frente al jugador
    public float heightMin = 1.5f;       // Altura mínima
    public float heightMax = 3.5f;       // Altura máxima
    public float rotationSpeed = 60f;    // Velocidad angular (°/s)
    public float lateralAmplitude = 2f;  // Oscilación lateral
    public float verticalAmplitude = 1f; // Oscilación vertical

    private float currentAngle = 0f;
    private float targetAngle = 0f;
    private float switchTimer = 0f;
    private float switchInterval = 5f;

    private Vector3 previousPosition;

    void Start()
    {
        previousPosition = transform.position;
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;

        // Cambiar el targetAngle cada cierto tiempo
        switchTimer += deltaTime;
        if (switchTimer >= switchInterval)
        {
            switchTimer = 0f;
            targetAngle = Random.Range(-90f, 90f); // Solo 180° en frente
            switchInterval = Random.Range(3f, 7f); // Cambios aleatorios
        }

        // Interpolación suave hacia targetAngle
        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, rotationSpeed * deltaTime);

        // Calcular posición en semicírculo frente al jugador
        Vector3 forward = playerCamera.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = playerCamera.right;

        Vector3 offset = Quaternion.Euler(0, currentAngle, 0) * forward * distance;

        // Movimiento senoide para suavidad
        float time = Time.time;
        offset += right * Mathf.Sin(time) * lateralAmplitude;
        offset.y = Mathf.Sin(time * 0.5f) * verticalAmplitude;

        // Limitar altura
        float desiredHeight = Mathf.Clamp(playerCamera.position.y + offset.y, heightMin, heightMax);
        offset.y = desiredHeight - playerCamera.position.y;

        // Nueva posición final
        Vector3 targetPosition = playerCamera.position + offset;

        // Actualizar posición del avión
        transform.position = targetPosition;

        // Calcular la dirección de movimiento (trayectoria real)
        Vector3 movementDirection = (transform.position - previousPosition).normalized;

        if (movementDirection.sqrMagnitude > 0.0001f)
        {
            // Orientar el avión hacia su trayectoria
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 5f * deltaTime);
        }

        // Guardar posición para el siguiente frame
        previousPosition = transform.position;
    }
}

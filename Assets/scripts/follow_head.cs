using UnityEngine;

public class FollowPlayerHead : MonoBehaviour
{
    [Tooltip("Cámara del jugador (cabeza). Se autoconfigura si está en blanco).")]
    public Transform playerHead;

    [Header("Ajustes de posición")]
    public float followDistance = 1.5f;
    public Vector3 offset = Vector3.zero;
    public float moveSmoothness = 1f;

    [Header("Rotación")]
    public bool facePlayer = true;
    public bool keepUpright = true;

    [Header("Ángulo de actualización")]
    [Range(0, 180)] public float angleThreshold = 50f;

    private Vector3 lastTargetPosition;

    void Start()
    {
        // Autoasignar la cámara principal si no se ha configurado
        if (playerHead == null && Camera.main != null)
        {
            playerHead = Camera.main.transform;
        }

        // Inicializar la posición actual
        if (playerHead != null)
        {
            lastTargetPosition = playerHead.position + playerHead.forward * followDistance + offset;
            transform.position = lastTargetPosition;
        }
    }

    void LateUpdate()
    {
        if (playerHead == null) return;

        // Dirección desde el jugador hacia el canvas
        Vector3 toCanvas = (transform.position - playerHead.position).normalized;
        Vector3 playerForward = playerHead.forward;

        float angle = Vector3.Angle(playerForward, toCanvas);

        // Si el ángulo supera el umbral, mover el canvas
        if (angle > angleThreshold)
        {
            Vector3 targetPos = playerHead.position + playerHead.forward * followDistance + offset;
            lastTargetPosition = targetPos;
        }

        // Movimiento suave
        transform.position = Vector3.Lerp(transform.position, lastTargetPosition, Time.deltaTime * moveSmoothness);

        // Mirar al jugador
        if (facePlayer)
        {
            Vector3 lookDir = transform.position - playerHead.position;
            if (keepUpright) lookDir.y = 0f;
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }
}

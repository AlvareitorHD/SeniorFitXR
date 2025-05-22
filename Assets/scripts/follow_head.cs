using UnityEngine;

public class FollowPlayerHead : MonoBehaviour
{
    [Tooltip("C�mara del jugador (cabeza). Se autoconfigura si est� en blanco).")]
    public Transform playerHead;

    [Header("Ajustes de posici�n")]
    public float followDistance = 1.5f;
    public Vector3 offset = Vector3.zero;
    public float moveSmoothness = 1f;

    [Header("Rotaci�n")]
    public bool facePlayer = true;
    public bool keepUpright = true;

    private Vector3 targetPosition;

    Camera mainCamera;

    void Start()
    {
        // Autoasignar la c�mara principal si no se ha configurado
        if (playerHead == null && Camera.main != null)
        {
            playerHead = Camera.main.transform;
        }

        mainCamera = Camera.main;

        // Posici�n inicial
        if (playerHead != null)
        {
            targetPosition = playerHead.position + playerHead.forward * followDistance + offset;
            transform.position = targetPosition;
        }
    }

    void LateUpdate()
    {
        if (playerHead == null) return;

        // Verificar si el objeto est� fuera del campo de visi�n
        Vector3 viewportPos = mainCamera.WorldToViewportPoint(transform.position);
        bool isVisible = viewportPos.z > 0 && viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1;

        if (!isVisible)
        {
            // Reposicionar delante del jugador si ya no se ve
            Vector3 desiredPosition = playerHead.position + playerHead.forward * followDistance + offset;

            // Solo actualizar la posici�n objetivo si el jugador NO se ha acercado demasiado
            float currentDistance = Vector3.Distance(playerHead.position, transform.position);
            if (currentDistance > followDistance * 0.75f) // permite acercarse sin que se aleje
            {
                targetPosition = desiredPosition;
            }
        }

        // Movimiento suave
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSmoothness);

        // Rotar hacia el jugador si est� habilitado
        if (facePlayer)
        {
            Vector3 lookDir = transform.position - playerHead.position;
            if (keepUpright) lookDir.y = 0f;
            transform.rotation = Quaternion.LookRotation(lookDir);
        }
    }
}

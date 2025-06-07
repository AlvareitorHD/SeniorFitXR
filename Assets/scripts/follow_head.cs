using UnityEngine;

public class FollowPlayerHeadImproved : MonoBehaviour
{
    [Tooltip("Referencia a la cabeza del jugador (se autoconfigura si está vacía).")]
    public Transform playerHead;

    [Header("Posición del menú")]
    public float idealDistance = 0.6f;
    public float verticalOffset = -0.2f;
    public Vector3 additionalOffset = Vector3.zero;
    public float positionLerpSpeed = 5f;

    [Header("Rotación")]
    public bool facePlayer = true;
    public bool keepUpright = true;

    [Header("Umbral de movimiento horizontal")]
    [Tooltip("Ángulo mínimo (en grados) que se debe superar para mover horizontalmente el menú.")]
    public float horizontalAngleThreshold = 90f;

    private Vector3 previousForwardFlat; // Dirección anterior (solo eje Y)
    private Camera mainCamera;

    void Start()
    {
        if (playerHead == null && Camera.main != null)
        {
            playerHead = Camera.main.transform;
        }

        mainCamera = Camera.main;

        if (playerHead != null)
        {
            previousForwardFlat = new Vector3(playerHead.forward.x, 0, playerHead.forward.z).normalized;
        }
    }

    void LateUpdate()
    {
        if (playerHead == null) return;

        // Dirección horizontal del jugador
        Vector3 currentForwardFlat = new Vector3(playerHead.forward.x, 0, playerHead.forward.z).normalized;

        // Ángulo entre direcciones horizontales
        float angle = Vector3.Angle(previousForwardFlat, currentForwardFlat);

        if (angle > horizontalAngleThreshold)
        {
            previousForwardFlat = currentForwardFlat;
        }

        // Verificar si el jugador ha pasado detrás del panel
        Vector3 menuToPlayer = playerHead.position - transform.position;
        Vector3 menuForward = (transform.position - playerHead.position).normalized;
        float dot = Vector3.Dot(menuForward, menuToPlayer.normalized);

        bool playerBehindPanel = dot < 0f; // Producto escalar negativo => jugador detrás

        Vector3 desiredPosition = transform.position;

        if (playerBehindPanel)
        {
            // Reposicionar solo si el jugador ha pasado detrás
            desiredPosition = playerHead.position + previousForwardFlat * idealDistance;
            desiredPosition.y += verticalOffset;
            desiredPosition += additionalOffset;
        }

        // Mover suavemente
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * positionLerpSpeed);

        // Rotar hacia el jugador
        if (facePlayer)
        {
            Vector3 lookDir = transform.position - playerHead.position;
            if (keepUpright) lookDir.y = 0;
            if (lookDir.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * positionLerpSpeed);
            }
        }
    }
}

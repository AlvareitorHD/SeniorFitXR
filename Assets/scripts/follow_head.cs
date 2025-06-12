using UnityEngine;

public class FollowPlayerHeadImproved : MonoBehaviour
{
    [Tooltip("Referencia a la cabeza del jugador (CenterEyeAnchor del XR Rig).")]
    public Transform playerHead;

    [Header("Posición del menú")]
    public float idealDistance = 0.6f;
    public float verticalOffset = -0.2f;
    public Vector3 additionalOffset = Vector3.zero;
    public float positionLerpSpeed = 5f;

    [Header("Límites verticales")]
    public float minY = 0.0f;  // Altura mínima permitida
    public float maxY = 2.0f;  // Altura máxima permitida

    [Header("Rotación")]
    public bool facePlayer = true;
    public bool keepUpright = true;

    [Header("Recolocación")]
    public float maxDistance = 2.5f;          // Recolocar si se aleja más de esto
    public float minDotThreshold = -0.3f;     // Recolocar solo si está claramente detrás (dot < -0.3)

    private Vector3 lastStablePosition;

    void Start()
    {
        if (playerHead == null && Camera.main != null)
        {
            playerHead = Camera.main.transform;
        }
    }

    void LateUpdate()
    {
        if (playerHead == null) return;

        Vector3 toPanel = transform.position - playerHead.position;
        float distance = toPanel.magnitude;

        Vector3 toPanelDir = toPanel.normalized;
        Vector3 playerForward = new Vector3(playerHead.forward.x, 0, playerHead.forward.z).normalized;
        float dot = Vector3.Dot(playerForward, toPanelDir);

        if (distance > maxDistance || dot < minDotThreshold)
        {
            RepositionSmoothly();
        }

        if (facePlayer)
        {
            Vector3 lookDir = transform.position - playerHead.position;
            if (keepUpright) lookDir.y = 0;
            // Si la dirección de la mirada es significativa, rotar suavemente hacia ella
            if (lookDir.sqrMagnitude > 0.3f)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * positionLerpSpeed);
            }
        }
    }

    void RepositionSmoothly()
    {
        Vector3 targetPos = playerHead.position + playerHead.forward * idealDistance;
        targetPos.y += verticalOffset;
        targetPos += additionalOffset;

        // 🔒 Aplicar límite de altura
        targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

        StopAllCoroutines();
        StartCoroutine(SmoothMove(targetPos));
    }

    System.Collections.IEnumerator SmoothMove(Vector3 targetPos)
    {
        float t = 0;
        Vector3 startPos = transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * positionLerpSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
        lastStablePosition = targetPos;
    }
}

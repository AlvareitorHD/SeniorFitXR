using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class WalkingInPlacePath : MonoBehaviour
{
    public Transform headTransform;
    public float movementThreshold = 0.02f;
    public float speed = 1.5f;

    public Transform[] pathPoints; // Define el camino con empty GameObjects

    private CharacterController characterController;
    private Vector3 lastHeadPosition;
    private float accumulatedVerticalMovement;

    private int currentSegment = 0;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        lastHeadPosition = headTransform.localPosition;
    }

    void Update()
    {
        Vector3 headDelta = headTransform.localPosition - lastHeadPosition;
        accumulatedVerticalMovement += Mathf.Abs(headDelta.y);

        if (accumulatedVerticalMovement > movementThreshold)
        {
            Vector3 pathDir = GetPathDirection();

            if (pathDir != Vector3.zero)
            {
                characterController.Move(pathDir.normalized * speed * Time.deltaTime);
                Debug.Log("Paso detectado – caminando sobre trayectoria");
            }

            accumulatedVerticalMovement = 0f;
        }

        lastHeadPosition = headTransform.localPosition;
    }

    Vector3 GetPathDirection()
    {
        if (pathPoints == null || pathPoints.Length < 2) return Vector3.zero;

        // Posición actual del jugador
        Vector3 pos = transform.position;

        // Encuentra segmento más cercano
        float minDist = float.MaxValue;
        int nearestIndex = currentSegment;

        for (int i = 0; i < pathPoints.Length - 1; i++)
        {
            Vector3 a = pathPoints[i].position;
            Vector3 b = pathPoints[i + 1].position;
            float dist = DistanceToSegment(pos, a, b);
            if (dist < minDist)
            {
                minDist = dist;
                nearestIndex = i;
            }
        }

        currentSegment = nearestIndex;

        // Dirección entre puntos
        Vector3 direction = pathPoints[nearestIndex + 1].position - pathPoints[nearestIndex].position;
        return new Vector3(direction.x, 0, direction.z).normalized;
    }

    float DistanceToSegment(Vector3 point, Vector3 a, Vector3 b)
    {
        Vector3 ap = point - a;
        Vector3 ab = b - a;
        float magnitudeAB = ab.sqrMagnitude;
        float ab_ap = Vector3.Dot(ap, ab);
        float t = Mathf.Clamp01(ab_ap / magnitudeAB);
        Vector3 closest = a + ab * t;
        return Vector3.Distance(point, closest);
    }
}

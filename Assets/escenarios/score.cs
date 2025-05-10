using UnityEngine;
using TMPro;

public class LookAtPlaneScore : MonoBehaviour
{
    public Transform playerCamera;
    public Transform plane;
    public float viewAngle = 15f;
    public float checkInterval = 0.1f;
    public int pointsPerInterval = 1;
    public TextMeshProUGUI scoreText; // <-- tu texto TMP

    private float timer = 0f;
    private int score = 0;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            timer = 0f;

            Vector3 directionToPlane = (plane.position - playerCamera.position).normalized;
            float dot = Vector3.Dot(playerCamera.forward, directionToPlane);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (angle <= viewAngle)
            {
                score += pointsPerInterval;
                scoreText.text = "Puntos: " + score;
            }
        }
    }
}

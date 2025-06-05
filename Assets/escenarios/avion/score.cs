using UnityEngine;
using TMPro;

public class LookAtPlaneScore : MonoBehaviour
{
    public Transform playerCamera;
    public Transform plane;
    public float viewAngle = 15f;
    public float checkInterval = 1f;
    public int pointsPerInterval = 1;
    public ContadorPuntos scoreManager; // Referencia al script de puntuación

    private float timer = 0f;

    // Tiempo de 10 segundos para que aún no cuente el primer punto
    private float initialDelay = 10f;
    private bool empieza = false;

    void Update()
    {
        timer += Time.deltaTime;
        // Si el temporizador es menor que el retraso inicial, no hacemos nada
        if (timer < initialDelay && !empieza)
        {
            return;
        }
        else
        {
            empieza = true; // A partir de aquí ya se cuentan los puntos
        }

        if (timer >= checkInterval)
        {
            timer = 0f;

            Vector3 directionToPlane = (plane.position - playerCamera.position).normalized;
            float dot = Vector3.Dot(playerCamera.forward, directionToPlane);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (angle <= viewAngle)
            {
                scoreManager.SumarPunto(); // Añadir puntos al contador
                Debug.Log("¡Buen trabajo! Has ganado " + pointsPerInterval + " punto(s). Ángulo: " + angle);
            }
            else
            {
                Debug.Log("No estás mirando el avión correctamente. Ángulo: " + angle);
            }
        }
    }

}

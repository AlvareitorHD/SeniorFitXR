using UnityEngine;
using TMPro;

public class LookAtPlaneScore : MonoBehaviour
{
    public Transform playerCamera;
    public Transform plane;
    public float viewAngle = 10f;
    public float checkInterval = 1f;
    public int pointsPerInterval = 1;
    public ContadorPuntos scoreManager; // Referencia al script de puntuaci�n
    public GameTimer gameTimer; // Referencia al script de temporizador del juego
    public BalloonManager balloonManager; // Referencia al script de globos
    // Audio para reproducir la mitad de juego
    public AudioClip mitad;
    private bool mitadReproducida = false; // Para evitar reproducir el audio m�s de una vez

    private float timer = 0f;

    // Tiempo de 10 segundos para que a�n no cuente el primer punto
    private bool empieza = false;

    private float mitadJuego = 0f;

    private void Start()
    {
        mitadJuego = gameTimer.gameDuration*2 / 3f; // Guardamos la mitad del juego para saber cu�ndo activar los globos (2/3 del tiempo total del juego)
        balloonManager.enabled = false; // Desactivar el script de globos al inicio
    }

    void Update()
    {
        timer += Time.deltaTime;
        // Si el temporizador es menor que el retraso inicial, no hacemos nada
        if ((timer < gameTimer.initialDelay && !empieza) || gameTimer.IsPaused || gameTimer.IsFinished)
        {
            return;
        }
        else if (gameTimer.IsFinished)
        {
            balloonManager.OnDisable(); // Desactivar los globos al finalizar el juego
            empieza = false; // Reiniciar el estado de inicio para el pr�ximo juego
            return; // No hacer nada si el juego ha terminado
        }
        else
        {
            empieza = true; // A partir de aqu� ya se cuentan los puntos
        }

        // Vamos a poner que durante la primera mitad del juego s�lo est� el avi�n y no los globos
        if (gameTimer.RemainingTime > mitadJuego)
        {
            comprobarAvion(); // Comprobar si el jugador est� mirando al avi�n
        }
        else
        {
            if (!mitadReproducida)
            {
                AudioSource.PlayClipAtPoint(mitad, playerCamera.position); // Reproducir el audio de mitad de juego
                mitadReproducida = true; // Evitar que se reproduzca m�s de una vez
            }
            balloonManager.enabled = true; // Activar el script de globos
            // Desactivar el avion
            plane.gameObject.SetActive(false);
        }

    }

    void comprobarAvion()
    {
        if (timer >= checkInterval)
        {
            timer = 0f;

            Vector3 directionToPlane = (plane.position - playerCamera.position).normalized;
            float dot = Vector3.Dot(playerCamera.forward, directionToPlane);
            float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

            if (angle <= viewAngle)
            {
                scoreManager.SumarPunto(); // A�adir puntos al contador
                Debug.Log("�Buen trabajo! Has ganado " + pointsPerInterval + " punto(s). �ngulo: " + angle);
            }
            else
            {
                Debug.Log("No est�s mirando el avi�n correctamente. �ngulo: " + angle);
            }
        }
    }

}

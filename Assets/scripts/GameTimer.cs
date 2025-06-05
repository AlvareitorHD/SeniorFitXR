using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    public float gameDuration = 300f; // Duración modificable
    public TextMeshProUGUI timerText;

    public GameObject canvasRoot;    // 👉 Asigna aquí el objeto raíz del Canvas en el Inspector
    public GameObject endPanel;
    public GameObject pausePanel;
    public GameObject gestorPuntos;

    private float remainingTime;
    private bool isPaused;

    // Tiempo de 10 segundos antes de que empiece a contar
    private float initialDelay = 10f;

    void Start()
    {
        isPaused = false;
        remainingTime = gameDuration;

        if (canvasRoot != null)
            canvasRoot.SetActive(false);

        endPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    void Update()
    {
        initialDelay -= Time.deltaTime;
        // Si el temporizador es menor que el retraso inicial, no hacemos nada
        if (initialDelay > 0f)
        {
            timerText.text = "El juego comenzará en: " + Mathf.CeilToInt(initialDelay).ToString() + " segundos";
            return; // No actualizamos el temporizador hasta que pase el retraso inicial
        }

        // comprobar si el jugador ha hecho con la mano el boton de three
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
            TogglePause();
        }

        if (isPaused) return;

        remainingTime -= Time.deltaTime;

        UpdateTimerUI();

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            EndGame();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        timerText.text = $"Tiempo: {minutes:00}:{seconds:00}";
    }

    // Método para calcular el tiempo jugado en minutos
    private float CalcularTiempoJugado()
    {
        return (gameDuration - remainingTime) / 60f; // Devuelve el tiempo jugado en minutos
    }

    public void TogglePause()
    {
        // Pausamos el gestor de puntos si existe
        if (gestorPuntos != null)
        {
            isPaused = !isPaused; // Cambiamos el estado de pausa
            ContadorPuntos contador = gestorPuntos.GetComponent<ContadorPuntos>();
            if (contador != null)
            {
                contador.PausarContador(isPaused); // Pausamos o reanudamos el contador de puntos
            }
            else
            {
                Debug.LogError("No se encontró el componente ContadorPuntos en GestorPuntos.");
            }
        }
        else
        {
            Debug.LogError("GestorPuntos no está asignado.");
        }

        Debug.Log("Pausa: " + isPaused);

        pausePanel.SetActive(isPaused);

        if (canvasRoot != null)
            canvasRoot.SetActive(isPaused);
    }

    public void EndGame()
    {
        // Ejecutar el audio source
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No se encontró un componente AudioSource en el objeto.");
        }

        pausePanel.SetActive(false);
        isPaused = true;
        timerText.text = "Tiempo: 00:00";
        endPanel.SetActive(true);

        // Mostrar puntos finales
        TextMeshProUGUI totalPuntosText = endPanel.transform.Find("total_puntos").GetComponent<TextMeshProUGUI>();
        ContadorPuntos contador;
        int puntosGanados = 0;

        if (gestorPuntos != null)
        {
            contador = gestorPuntos.GetComponent<ContadorPuntos>();
            if (contador != null)
            {
                puntosGanados = contador.Puntos;
                totalPuntosText.text = "Puntos Conseguidos: " + puntosGanados.ToString();
            }
            else
            {
                Debug.LogError("No se encontró el componente ContadorPuntos en GestorPuntos.");
            }
        }
        else
        {
            Debug.LogError("GestorPuntos no está asignado.");
        }

        if (canvasRoot != null)
            canvasRoot.SetActive(true);

        // ✅ NUEVO: Actualizar usuarioActual con puntos y tiempo
        float tiempoJugadoMin = CalcularTiempoJugado(); // Asegúrate de tener esta función si no la tienes

        if (UsuarioGlobal.Instance != null)
        {
            UsuarioGlobal.Instance.AgregarTiempo(tiempoJugadoMin);
            UsuarioGlobal.Instance.SumarPuntos(puntosGanados);

            Debug.Log($"Tiempo jugado: {tiempoJugadoMin} minutos, Puntos ganados: {puntosGanados}");

            // Actualizar datos del usuario en el servidor
            UsuarioGlobal.Instance.ActualizarDatosUsuario();
            Debug.Log("Datos del usuario actualizados correctamente.");
        }
        else
        {
            Debug.LogWarning("Usuario actual no está definido.");
        }
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("inicio");
    }

    public void TerminarDesdePausa()
    {
        pausePanel.SetActive(false);
        EndGame();
    }

    public void Reanudar()
    {
        TogglePause();
    }
}

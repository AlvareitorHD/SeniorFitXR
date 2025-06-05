using UnityEngine;

public class PosicionarFrenteDerecha : MonoBehaviour
{
    public Transform headTransform;     // Cámara del jugador
    public Transform cabezaRobot;       // Cabeza del robot

    public float distanciaFrente = 1f;  // Más cerca que antes
    public float distanciaDerecha = 0.5f;
    public float alturaRobot = 0.5f;

    public float velocidadMovimiento = 2.0f;     // Velocidad de movimiento suave
    public float velocidadRotacion = 5.0f;       // Velocidad de rotación suave
    public float umbralAnguloReubicacion = 160f; // Cuánto tiene que girar el jugador para mover el robot

    private Vector3 posicionDeseada;
    private Quaternion rotacionDeseada;
    private bool debeReubicar = true;

    void Start()
    {
        CalcularNuevaPosicionYRotacion();
        transform.position = posicionDeseada;
        transform.rotation = rotacionDeseada;
    }

    void Update()
    {
        // Calcular el angulo de vision del jugador respecto al robot
        if (headTransform == null) return;
        Vector3 direccionAlRobot = transform.position - headTransform.position;
        direccionAlRobot.y = 0; // Ignorar la altura
        Vector3 direccionAlJugador = headTransform.forward;
        direccionAlJugador.y = 0; // Ignorar la altura
        float angulo = Vector3.Angle(direccionAlRobot, direccionAlJugador);

        if (angulo > umbralAnguloReubicacion)
        {
            debeReubicar = true;
        }

        if (debeReubicar)
        {
            CalcularNuevaPosicionYRotacion();
            transform.position = Vector3.Lerp(transform.position, posicionDeseada, Time.deltaTime * velocidadMovimiento);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, Time.deltaTime * velocidadRotacion);

            // Cuando ya está suficientemente cerca, deja de moverse
            if (Vector3.Distance(transform.position, posicionDeseada) < 0.01f)
            {
                debeReubicar = false;
            }
        }

        // Cabeza del robot sigue al jugador en el eje Y (mirada suave)
        if (cabezaRobot != null)
        {
            Vector3 objetivoMirada = headTransform.position;
            objetivoMirada.y = cabezaRobot.position.y;

            Quaternion rotacionCabeza = Quaternion.LookRotation(objetivoMirada - cabezaRobot.position);
            cabezaRobot.rotation = Quaternion.Slerp(cabezaRobot.rotation, rotacionCabeza, Time.deltaTime * velocidadRotacion);
        }
    }

    void CalcularNuevaPosicionYRotacion()
    {
        if (headTransform == null) return;

        Vector3 forward = headTransform.forward;
        Vector3 right = headTransform.right;

        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        posicionDeseada = headTransform.position +
                          forward * distanciaFrente +
                          right * distanciaDerecha;
        posicionDeseada.y = alturaRobot;

        Vector3 direccionAlJugador = headTransform.position - posicionDeseada;
        direccionAlJugador.y = 0;

        if (direccionAlJugador != Vector3.zero)
        {
            Quaternion rotacionBase = Quaternion.LookRotation(direccionAlJugador);
            rotacionDeseada = rotacionBase * Quaternion.Euler(-90f, 0f, 0f); // Compensación por el modelo
        }
    }
}

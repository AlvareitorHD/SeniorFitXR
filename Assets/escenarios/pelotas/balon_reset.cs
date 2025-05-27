using UnityEngine;
using Oculus.Interaction;

public class BalonReset : MonoBehaviour
{
    public float tiempoParaReset = 5f;

    private Rigidbody rb;
    private Grabbable grabbable;
    private Vector3 posicionInicial;
    private Quaternion rotacionInicial;

    private float tiempoFuera = 0f;
    private bool haTocadoSuelo = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        grabbable = GetComponent<Grabbable>();

        posicionInicial = transform.position;
        rotacionInicial = transform.rotation;
    }

    private void Update()
    {
        // Si el jugador lo tiene agarrado, reseteamos el temporizador
        if (grabbable != null && grabbable.SelectingPointsCount > 0)
        {
            tiempoFuera = 0f;
            return;
        }

        // Si ha tocado suelo y no está en la posición original
        if (haTocadoSuelo && Vector3.Distance(transform.position, posicionInicial) > 0.1f)
        {
            tiempoFuera += Time.deltaTime;

            if (tiempoFuera >= tiempoParaReset)
            {
                ReiniciarBalon();
            }
        }
    }

    private void ReiniciarBalon()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.position = posicionInicial;
        transform.rotation = rotacionInicial;

        tiempoFuera = 0f;
        haTocadoSuelo = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Suelo"))
        {
            haTocadoSuelo = true;
        }
    }
}

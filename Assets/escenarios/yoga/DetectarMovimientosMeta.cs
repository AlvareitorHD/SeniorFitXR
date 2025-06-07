using UnityEngine;
using System.Collections;

public class DetectorMovimientosMeta : MonoBehaviour
{
    public GameTimer gameTimer;
    public ContadorPuntos contadorPuntos;

    [Header("Referencias de tracking")]
    public Transform cabeza; // asigna el transform del Head dentro de CameraRig
    public Transform manoIzquierda; // puedes usar OVRHand.transform
    public Transform manoDerecha;

    [Header("Parámetros de detección")]
    public float umbralRotacion = 45f; // grados
    public float alturaMinBrazos = 1.1f;
    public float alturaMaxBrazos = 2.5f;

    private bool puedeDetectar = true;
    private bool haGiradoIzquierda = false;
    private bool brazosArriba = false;

    void Update()
    {
        if (gameTimer == null || contadorPuntos == null) return;
        if (gameTimer.EstaPausado()) return;
        if (!puedeDetectar) return;

        float tiempoRestante = gameTimer.TiempoRestante();
        float mitad = gameTimer.DuracionTotal() / 2f;

        if (tiempoRestante > mitad)
        {
            DetectarRotacionCabeza();
        }
        else
        {
            DetectarMovimientoBrazos();
        }
    }

    void DetectarRotacionCabeza()
    {
        float rotY = cabeza.eulerAngles.y;
        if (rotY > 180f) rotY -= 360f; // Convertir a rango -180 a 180

        if (!haGiradoIzquierda && rotY <= -umbralRotacion)
        {
            haGiradoIzquierda = true;
        }
        else if (haGiradoIzquierda && rotY >= umbralRotacion)
        {
            haGiradoIzquierda = false;
            contadorPuntos.SumarPunto();
            StartCoroutine(ResetearDeteccion());
        }
    }

    void DetectarMovimientoBrazos()
    {
        float alturaL = manoIzquierda.position.y;
        float alturaR = manoDerecha.position.y;

        bool ambasArriba = alturaL > alturaMinBrazos && alturaR > alturaMinBrazos &&
                           alturaL < alturaMaxBrazos && alturaR < alturaMaxBrazos;

        if (!brazosArriba && ambasArriba)
        {
            brazosArriba = true;
        }
        else if (brazosArriba && alturaL < alturaMinBrazos && alturaR < alturaMinBrazos)
        {
            brazosArriba = false;
            contadorPuntos.SumarPunto();
            StartCoroutine(ResetearDeteccion());
        }
    }

    IEnumerator ResetearDeteccion()
    {
        puedeDetectar = false;
        yield return new WaitForSeconds(3f);
        puedeDetectar = true;
    }
}

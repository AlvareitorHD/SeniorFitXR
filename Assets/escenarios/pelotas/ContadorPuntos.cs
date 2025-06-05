using UnityEngine;
using TMPro;

public class ContadorPuntos : MonoBehaviour
{
    public TextMeshProUGUI textMesh; // Asigna el componente TextMeshProUGUI desde el Inspector
    private int puntos = 0;
    public AudioClip sonidoPunto; // Asigna el clip de audio desde el Inspector
    private bool pausado = false;
    // Getter de puntos
    public int Puntos
    {
        get { return puntos; }
    }

    public void SumarPunto()
    {
        if (pausado) return; // Si el juego está pausado, no sumar puntos
        puntos++;
        textMesh.text = "Puntos: " + puntos.ToString();
        ReproducirSonido();
    }

    public void SumarDoble()
    {
        if (pausado) return; // Si el juego está pausado, no sumar puntos
        puntos += 2;
        textMesh.text = "Puntos: " + puntos.ToString();
        ReproducirSonido();
    }
    public void Sumar5()
    {
        if (pausado) return; // Si el juego está pausado, no sumar puntos
        puntos += 5;
        textMesh.text = "Puntos: " + puntos.ToString();
        ReproducirSonido();
    }

    public void Start()
    {
        textMesh.text = "Puntos: " + puntos.ToString();
    }

    // Método para pausar el contador de puntos
    public void PausarContador(bool pausar)
    {
        pausado = pausar;
        if (pausado)
        {
            Debug.Log("Contador de puntos pausado.");
        }
        else
        {
            Debug.Log("Contador de puntos reanudado.");
        }
    }

    // Funcion para reproducir el sonido al sumar puntos
    public void ReproducirSonido()
    {
        if (sonidoPunto != null)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.PlayOneShot(sonidoPunto);
            }
            else
            {
                Debug.LogWarning("No se encontró un componente AudioSource en el objeto.");
            }
        }
        else
        {
            Debug.LogWarning("No se ha asignado un clip de audio para el sonido de puntos.");
        }
    }
}

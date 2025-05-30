using UnityEngine;
using TMPro;

public class ContadorPuntos : MonoBehaviour
{
    public TextMeshProUGUI textMesh; // Asigna el componente TextMeshProUGUI desde el Inspector
    private int puntos = 0;

    // Getter de puntos
    public int Puntos
    {
        get { return puntos; }
    }

    public void SumarPunto()
    {
        puntos++;
        textMesh.text = "Puntos: " + puntos.ToString();
    }

    public void SumarDoble()
    {
        puntos += 2;
        textMesh.text = "Puntos: " + puntos.ToString();
    }
    public void Sumar5()
    {
        puntos += 5;
        textMesh.text = "Puntos: " + puntos.ToString();
    }

    public void Start()
    {
        textMesh.text = "Puntos: " + puntos.ToString();
    }
}

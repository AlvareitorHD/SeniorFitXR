using UnityEngine;
using UnityEngine.SceneManagement;

public class CambiarEscena : MonoBehaviour
{
    public string nombreEscena; // Nombre de la escena a cargar

    public void Cambiar()
    {
        SceneManager.LoadScene(nombreEscena);
    }
}

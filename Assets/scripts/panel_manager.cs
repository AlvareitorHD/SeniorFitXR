using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [Header("Paneles de UI")]
    public GameObject panelInicio;
    public GameObject panelRegistro;
    public GameObject panelOpciones;
    public GameObject panelUsuarios;
    // Puedes añadir más paneles aquí...

    void Start()
    {
        MostrarSolo(panelInicio);
    }

    public void MostrarInicio()
    {
        MostrarSolo(panelInicio);
    }

    public void MostrarRegistro()
    {
        MostrarSolo(panelRegistro);
    }

    public void MostrarOpciones()
    {
        MostrarSolo(panelOpciones);
    }

    public void MostrarUsuarios()
    {
        MostrarSolo(panelUsuarios);
    }

    private void MostrarSolo(GameObject panelActivo)
    {
        panelInicio.SetActive(panelActivo == panelInicio);
        panelRegistro.SetActive(panelActivo == panelRegistro);
        panelOpciones.SetActive(panelActivo == panelOpciones);
        panelUsuarios.SetActive(panelActivo==panelOpciones);
        // Añade aquí más paneles si agregas más
    }

    // Método para los input fields que hacen abrir el teclado virtual de Meta Quest
    public void AbrirTecladoVirtual()
    {
        // Abre el teclado virtual de Meta Quest
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    // Método para el botón salir, que cierra la aplicación
    public void Salir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}

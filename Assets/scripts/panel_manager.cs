using UnityEngine;

public class PanelManager : MonoBehaviour
{
    [Header("Paneles de UI")]
    public GameObject panelInicio;
    public GameObject panelRegistro;
    public GameObject panelOpciones;
    public GameObject panelUsuarios;
    // Puedes a�adir m�s paneles aqu�...

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
        // A�ade aqu� m�s paneles si agregas m�s
    }

    // M�todo para los input fields que hacen abrir el teclado virtual de Meta Quest
    public void AbrirTecladoVirtual()
    {
        // Abre el teclado virtual de Meta Quest
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    // M�todo para el bot�n salir, que cierra la aplicaci�n
    public void Salir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}

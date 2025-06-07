using UnityEngine;

public class botonCerrarSesion : MonoBehaviour
{
    public PanelManager panelManager;

    //Metodo que se ejecuta al pulsar el botón de cerrar sesión
    public void CerrarSesion()
    {
        UsuarioGlobal.Instance.CerrarSesion();
        // Mostramos el panel de inicio
        panelManager.MostrarInicioTrasCerrarSesion();
    }
}

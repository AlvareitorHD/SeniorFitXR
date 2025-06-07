using UnityEngine;

public class botonCerrarSesion : MonoBehaviour
{
    public PanelManager panelManager;

    //Metodo que se ejecuta al pulsar el bot�n de cerrar sesi�n
    public void CerrarSesion()
    {
        UsuarioGlobal.Instance.CerrarSesion();
        // Mostramos el panel de inicio
        panelManager.MostrarInicioTrasCerrarSesion();
    }
}

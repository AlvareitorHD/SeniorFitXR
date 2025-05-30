using UnityEngine;

public class UsuarioGlobal : MonoBehaviour
{
    public static UsuarioGlobal Instance { get; private set; }
    public Usuario usuarioActual;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persiste entre escenas si es necesario
    }

    public void EstablecerUsuario(Usuario usuario)
    {
        usuarioActual = usuario;
        Debug.Log($"Usuario actual: {usuario.name}");
    }

    public void SumarPuntos(int puntos)
    {
        if (usuarioActual != null)
        {
            usuarioActual.puntosTotales += puntos;
            usuarioActual.puntosSesion += puntos;
        }
    }

    public void AgregarTiempo(float minutos)
    {
        if (usuarioActual != null)
        {
            usuarioActual.tiempoTotalEjercicio += minutos;
        }
    }

    public void AgregarLogro(string logro)
    {
        if (usuarioActual != null && !usuarioActual.logros.Contains(logro))
        {
            usuarioActual.logros.Add(logro);
        }
    }

    public void AgregarReto(string reto)
    {
        if (usuarioActual != null && !usuarioActual.retosCompletados.Contains(reto))
        {
            usuarioActual.retosCompletados.Add(reto);
        }
    }

    public void NuevaSesion()
    {
        if (usuarioActual != null)
        {
            usuarioActual.numeroSesiones++;
            usuarioActual.puntosSesion = 0;
        }
    }

    public void CerrarSesion()
    {
        usuarioActual = null;
        Debug.Log("[UsuarioGlobal] Sesión cerrada. usuarioActual ahora es null.");
    }

}
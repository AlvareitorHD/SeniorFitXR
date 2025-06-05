using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class UsuarioGlobal : MonoBehaviour
{
    public static UsuarioGlobal Instance { get; private set; }

    private Usuario usuarioActual;
    public string serverBaseUrl = "https://pegasus-powerful-imp.ngrok-free.app";
    public string apiEndpoint = "/api/usuarios";

    //Getter para obtener el usuario actual
    public Usuario UsuarioActual
    {
        get { return usuarioActual; }
    }

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
        // Imprimir todos los detalles del usuario actual en la consola para depuración
        Debug.Log($"Usuario actual: {usuarioActual.name}, ID: {usuarioActual.id}, Altura: {usuarioActual.height}, Puntos Totales: {usuarioActual.puntosTotales}, Puntos de Sesión: {usuarioActual.puntosSesion}, Sesiones: {usuarioActual.numeroSesiones}, Tiempo Total de Ejercicio: {usuarioActual.tiempoTotalEjercicio} minutos, Fecha de Registro: {usuarioActual.fechaRegistro}");
        NuevaSesion();
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

    // Método para actualizar datos en el servidor y guardar localmente
    public void ActualizarDatosUsuario()
    {
        if (usuarioActual == null)
        {
            Debug.LogWarning("[UsuarioGlobal] No hay usuario actual para actualizar.");
            return;
        }

        StartCoroutine(EnviarDatosAlServidorYGuardar());
    }

    private IEnumerator EnviarDatosAlServidorYGuardar()
    {
        string url = $"{serverBaseUrl}{apiEndpoint}/{usuarioActual.id}";

        string json = JsonUtility.ToJson(usuarioActual);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "PATCH");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("[UsuarioGlobal] Datos actualizados en el servidor.");
        }
        else
        {
            Debug.LogError("[UsuarioGlobal] Error al actualizar el usuario: " + request.error);
        }

        GuardarUsuarioLocal(usuarioActual);
    }

    private void GuardarUsuarioLocal(Usuario usuario)
    {
        string path = Path.Combine(Application.persistentDataPath, "usuario.json");
        string json = JsonUtility.ToJson(usuario, true);
        File.WriteAllText(path, json);
        Debug.Log($"[UsuarioGlobal] Usuario guardado localmente en {path}");
    }

    // Wrapper para permitir la serialización de diccionarios con JsonUtility
    [System.Serializable]
    private class Wrapper
    {
        public Dictionary<string, object> data;

        public Wrapper(Dictionary<string, object> d)
        {
            data = d;
        }
    }

    public void NuevaSesion()
    {
        if (usuarioActual != null)
        {
            usuarioActual.numeroSesiones++;
            usuarioActual.puntosSesion = 0;
        }
        // Enviar http post a /api/usuarios/{id}/connect
        StartCoroutine(EnviarNuevaSesionAlServidor());
    }

    private IEnumerator EnviarNuevaSesionAlServidor()
    {
        if (usuarioActual == null)
        {
            Debug.LogWarning("[UsuarioGlobal] No hay usuario actual para iniciar sesión.");
            yield break;
        }
        string url = $"{serverBaseUrl}{apiEndpoint}/{usuarioActual.id}/connect";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("[UsuarioGlobal] Nueva sesión iniciada en el servidor.");
        }
        else
        {
            Debug.LogError("[UsuarioGlobal] Error al iniciar nueva sesión: " + request.error);
        }
    }

    public void CerrarSesion()
    {
        Debug.Log("[UsuarioGlobal] Sesión cerrada. usuarioActual ahora es null.");
        // Enviar http post a /api/usuarios/{id}/disconnect
        StartCoroutine(EnviarCerrarSesionAlServidor());
        usuarioActual = null; // Limpiar el usuario actual
    }

    private IEnumerator EnviarCerrarSesionAlServidor()
    {
        if (usuarioActual == null)
        {
            Debug.LogWarning("[UsuarioGlobal] No hay usuario actual para cerrar sesión.");
            yield break;
        }
        string url = $"{serverBaseUrl}{apiEndpoint}/{usuarioActual.id}/disconnect";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("[UsuarioGlobal] Sesión cerrada en el servidor.");
        }
        else
        {
            Debug.LogError("[UsuarioGlobal] Error al cerrar sesión: " + request.error);
        }
    }

}
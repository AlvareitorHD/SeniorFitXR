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

    // Booleano para saber si el servidor está activo
    private bool isServerActive = true;

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

    // Método que se ejecuta tras cargar cualquier escena
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        BuscarComponentesVR();
        if (cabeza != null) lastPosCabeza = cabeza.position;
        if (manoIzquierda != null) lastPosManoIzquierda = manoIzquierda.position;
        if (manoDerecha != null) lastPosManoDerecha = manoDerecha.position;
    }

    // Ahora registramos el evento OnSceneLoaded para que se ejecute al cargar una escena
    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
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

    public void AgregarLogro(Logro logro)
    {
        if (usuarioActual != null)
        {
            usuarioActual.logros.Add(logro);
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
            // Si el servidor no es accesible, desactivamos el booleano isServerActive
            isServerActive = false;
        }

        GuardarUsuarioLocal(usuarioActual);
    }

    private void GuardarUsuarioLocal(Usuario usuario)
    {
        string path = Path.Combine(Application.persistentDataPath, "usuarios.json");
        List<Usuario> usuarios = new List<Usuario>();

        if (File.Exists(path))
        {
            string contenido = File.ReadAllText(path);
            UsuarioList lista = JsonUtility.FromJson<UsuarioList>(contenido);
            usuarios = lista.usuarios;

            // Reemplazar si ya existe el usuario
            int index = usuarios.FindIndex(u => u.id == usuario.id);
            if (index >= 0) usuarios[index] = usuario;
            else usuarios.Add(usuario);
        }
        else
        {
            usuarios.Add(usuario);
        }

        UsuarioList nuevaLista = new UsuarioList { usuarios = usuarios };
        string json = JsonUtility.ToJson(nuevaLista, true);
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

        // Comenzar a enviar ROM periódicamente
        BuscarComponentesVR();
        if (cabeza != null) lastPosCabeza = cabeza.position;
        if (manoIzquierda != null) lastPosManoIzquierda = manoIzquierda.position;
        if (manoDerecha != null) lastPosManoDerecha = manoDerecha.position;
        StartCoroutine(EnviarROMPeriodicamente());
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
            // Si la sesión se inicia correctamente, activamos el booleano isServerActive
            isServerActive = true;
        }
        else
        {
            Debug.LogError("[UsuarioGlobal] Error al iniciar nueva sesión: " + request.error);
            // Si el servidor no es accesible, desactivamos el booleano isServerActive
            isServerActive = false;
        }
    }

    public void CerrarSesion()
    {
        // Terminar el envío de ROM
        StopCoroutine(EnviarROMPeriodicamente());

        Debug.Log("[UsuarioGlobal] Sesión cerrada. usuarioActual ahora es null.");
        
        // Enviar http post a /api/usuarios/{id}/disconnect
        StartCoroutine(EnviarCerrarSesionAlServidor());
        
        // Guardar el usuario actual localmente
        if (usuarioActual != null)
        {
            GuardarUsuarioLocal(usuarioActual);
            Debug.Log("[UsuarioGlobal] Usuario guardado al cerrar sesión.");
        }
        
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

    public void OnApplicationQuit()
    {
        CerrarSesion();
    }

    // PARTE PARA MEDIR LA ACTIVIDAD DEL USUARIO
    // Objetos a asignar desde el Inspector o por script
    public Transform cabeza;
    public Transform manoDerecha;
    public Transform manoIzquierda;

    private Vector3 lastPosCabeza;
    private Vector3 lastPosManoIzquierda;
    private Vector3 lastPosManoDerecha;

    // Máximo movimiento esperado para normalizar (en metros)
    public float maxMovimientoCabeza = 0.2f;  // ejemplo: 20 cm
    public float maxMovimientoMano = 0.5f;    // ejemplo: 50 cm

    private void BuscarComponentesVR()
    {
        // Búsqueda por nombre común para OVRCameraRig o XR Rig
        if (cabeza == null)
        {
            Camera cam = Camera.main;
            if (cam != null) cabeza = cam.transform;
            Debug.Log($"[VR] Cabeza encontrada: {cabeza?.name}");
        }

        if (manoDerecha == null)
        {
            var rightHand = GameObject.Find("RightHandAnchor") ?? GameObject.Find("RightHand Controller") ?? GameObject.Find("RightHand");
            if (rightHand != null) manoDerecha = rightHand.transform;
            Debug.Log($"[VR] Mano derecha encontrada: {manoDerecha?.name}");
        }

        if (manoIzquierda == null)
        {
            var leftHand = GameObject.Find("LeftHandAnchor") ?? GameObject.Find("LeftHand Controller") ?? GameObject.Find("LeftHand");
            if (leftHand != null) manoIzquierda = leftHand.transform;
            Debug.Log($"[VR] Mano izquierda encontrada: {manoIzquierda?.name}");
        }

        if (cabeza == null || manoDerecha == null || manoIzquierda == null)
        {
            Debug.LogWarning("[VR] No se encontraron todas las referencias de cuerpo. Asegúrate de que los objetos estén correctamente nombrados.");
        }
    }

    private float CalcularActividad(Vector3 currentPos, ref Vector3 lastPos, float maxMovimiento)
    {
        float distancia = Vector3.Distance(currentPos, lastPos);
        lastPos = currentPos;

        // Normalizamos entre 0 y 1, con clamp para evitar valores mayores
        float actividad = Mathf.Clamp01(distancia / maxMovimiento);
        return actividad;
    }

    // Envío HTTP de los datos ROM
    private IEnumerator EnviarROMPeriodicamente()
    {
        // Mientras el usuario esté activo y los componentes VR estén asignados
        while (usuarioActual != null && isServerActive)
        {
            if (usuarioActual != null && cabeza != null && manoIzquierda != null && manoDerecha != null)
            {
                float actividadCabeza = CalcularActividad(cabeza.position, ref lastPosCabeza, maxMovimientoCabeza);
                float actividadManoIzq = CalcularActividad(manoIzquierda.position, ref lastPosManoIzquierda, maxMovimientoMano);
                float actividadManoDer = CalcularActividad(manoDerecha.position, ref lastPosManoDerecha, maxMovimientoMano);

                ROMData datosROM = new ROMData(actividadCabeza, actividadManoIzq, actividadManoDer);

                string url = $"{serverBaseUrl}{apiEndpoint}/{usuarioActual.id}/rom";
                string json = JsonUtility.ToJson(datosROM);
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

                using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
                {
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    request.SetRequestHeader("Content-Type", "application/json");

                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        Debug.Log("[UsuarioGlobal] Actividad ROM enviada correctamente.");
                    }
                    else
                    {
                        Debug.LogError("[UsuarioGlobal] Error enviando ROM: " + request.error);
                        Debug.LogError("[UsuarioGlobal] Respuesta servidor: " + request.downloadHandler.text);
                    }
                }
            }
            else
            {
                Debug.LogError("[UsuarioGlobal] No se puede enviar ROM: usuarioActual o componentes VR no están asignados.");
                if (usuarioActual == null)
                {
                    Debug.LogWarning("[UsuarioGlobal] usuarioActual es null.");
                }
                if (cabeza == null || manoIzquierda == null || manoDerecha == null)
                {
                    Debug.LogWarning("[UsuarioGlobal] Algún componente VR no está asignado. Asegúrate de que estén correctamente referenciados.");
                }

            }

                yield return new WaitForSeconds(1f);
        }
    }

    [System.Serializable] // Serializable significa que esta clase puede ser convertida a JSON
    public class ROMData
    {
        public float cabeza;
        public float manoIzquierda;
        public float manoDerecha;
        public string timestamp;

        public ROMData(float cabeza, float manoIzquierda, float manoDerecha)
        {
            this.cabeza = cabeza;
            this.manoIzquierda = manoIzquierda;
            this.manoDerecha = manoDerecha;
            this.timestamp = System.DateTime.UtcNow.ToString("o"); // formato ISO 8601 UTC
        }
    }

}
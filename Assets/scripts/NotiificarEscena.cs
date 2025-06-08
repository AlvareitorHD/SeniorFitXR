using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class EjercicioActualNotifier : MonoBehaviour
{
    public static EjercicioActualNotifier Instance;
    public string serverBaseUrl = "https://pegasus-powerful-imp.ngrok-free.app";

    private string escenaActual = "";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // evita duplicados
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        escenaActual = SceneManager.GetActiveScene().name;
        NotificarCambioEscena(escenaActual);
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        escenaActual = newScene.name;
        // Swtich case para mejorar la legibilidad de los nombres de escena
        switch (escenaActual)
        {
            case "inicio":
                escenaActual = "Panel de inicio";
                break;
            case "pelotas":
                escenaActual = "Baloncesto";
                break;
            case "avion":
                escenaActual = "Avión y globos";
                break;
            case "yoga":
                escenaActual = "Yoga";
                break;
            case "disco":
                escenaActual = "Discoequilibrio";
                break;
            default:
                // Si no es un ejercicio conocido, no cambiar el nombre
                break;
        }
        NotificarCambioEscena(escenaActual);
    }

    private void NotificarCambioEscena(string nombreEscena)
    {
        if (UsuarioGlobal.Instance != null && UsuarioGlobal.Instance.UsuarioActual != null)
        {
            int userId = UsuarioGlobal.Instance.UsuarioActual.id;
            StartCoroutine(EnviarNombreEjercicio(userId, nombreEscena));
        }
    }

    private IEnumerator EnviarNombreEjercicio(int userId, string nombreEscena)
    {
        string url = $"{serverBaseUrl}/api/usuarios/{userId}/ejercicio";
        string json = JsonUtility.ToJson(new EjercicioPayload(nombreEscena));
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error al enviar el ejercicio actual: " + request.error);
        }
    }

    [System.Serializable]
    private class EjercicioPayload
    {
        public string ejercicioActual;

        public EjercicioPayload(string nombre)
        {
            ejercicioActual = nombre;
        }
    }
}

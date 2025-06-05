using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.IO;

public class BotonUsuario : MonoBehaviour
{
    public int usuarioId;
    public string servidorBase = "https://pegasus-powerful-imp.ngrok-free.app";
    public PanelManager panelManager;
    private string RutaLocal => Path.Combine(Application.persistentDataPath, "usuarios.json");


    private void Start()
    {
        panelManager = Object.FindFirstObjectByType<PanelManager>();
        GetComponent<Button>().onClick.AddListener(() => StartCoroutine(CargarUsuarioPorId()));
    }

    IEnumerator CargarUsuarioPorId()
    {
        string url = $"{servidorBase}/api/usuarios/{usuarioId}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Usuario usuario = JsonUtility.FromJson<Usuario>(json);
            UsuarioGlobal.Instance.EstablecerUsuario(usuario);

            Debug.Log($"[BotonUsuario] Usuario {usuario.name} cargado como usuarioActual.");

            if (panelManager != null)
            {
                panelManager.MostrarEjercicios();
            }
            else
            {
                Debug.LogWarning("[BotonUsuario] PanelManager no encontrado en la escena.");
            }
        }
        else
        {
            Debug.LogWarning($"[BotonUsuario] Error al cargar usuario ID {usuarioId} del servidor: {request.error}");
            // Cargar el usuario de memoria local
            if (File.Exists(RutaLocal))
            {
                string json = File.ReadAllText(RutaLocal);
                UsuarioList lista = JsonUtility.FromJson<UsuarioList>(json);
                Usuario usuario = lista.usuarios.Find(u => u.id == usuarioId);
                if (usuario != null)
                {
                    UsuarioGlobal.Instance.EstablecerUsuario(usuario);
                    Debug.Log($"[BotonUsuario] Usuario {usuario.name} cargado desde memoria local.");
                    if (panelManager != null)
                    {
                        panelManager.MostrarEjercicios();
                    }
                }
                else
                {
                    Debug.LogError($"[BotonUsuario] Usuario ID {usuarioId} no encontrado en memoria local.");
                }
            }
            else
            {
                Debug.LogError("[BotonUsuario] No se encontró el archivo de usuarios en memoria local.");
            }

        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class BotonUsuario : MonoBehaviour
{
    public int usuarioId;
    public string servidorBase = "https://pegasus-powerful-imp.ngrok-free.app";
    public PanelManager panelManager;

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
            UsuarioGlobal.Instance.usuarioActual = usuario;

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
            Debug.LogError($"[BotonUsuario] Error al cargar usuario ID {usuarioId}: {request.error}");
        }
    }
}

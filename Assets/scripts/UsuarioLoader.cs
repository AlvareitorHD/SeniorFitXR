using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

public class UsuarioLoader : MonoBehaviour
{
    [Header("UI")]
    public Transform panelContenedor;
    public GameObject botonPrefab;


    [Header("Red")]
    public string serverBaseUrl = "https://pegasus-powerful-imp.ngrok-free.app";
    public string apiEndpoint = "/api/usuarios";

    private string RutaLocal => Path.Combine(Application.persistentDataPath, "usuarios.json");
    private string RutaImagenes => Path.Combine(Application.persistentDataPath, "imagenes");
    private string NombreImagenDefault = "default.png";

    private Sprite spriteDefault;
        
    private void OnEnable()
    {
        Debug.Log("[UsuarioLoader] OnEnable: Inicio de carga de usuarios.");
        if (!Directory.Exists(RutaImagenes)) Directory.CreateDirectory(RutaImagenes);
        StartCoroutine(PrepararYMostrarUsuarios());
    }

    IEnumerator PrepararYMostrarUsuarios()
    {
        yield return StartCoroutine(CargarImagenDefault());
        yield return StartCoroutine(CargarUsuariosUnicaVez());
    }


    // Este método se llama una sola vez al inicio para cargar los usuarios desde el servidor o desde el archivo local.
    // Si el archivo local no existe, se descargan los usuarios del servidor.
    // Si el archivo local existe, se cargan los usuarios desde allí.
    // Si hay un error al descargar del servidor, se usa el archivo local como respaldo.
    // Si hay usuarios en el archivo local, se muestran en el panel.
    // Si no hay usuarios, se muestra un mensaje de que no hay usuarios disponibles.
    IEnumerator CargarUsuariosUnicaVez()
    {
        LimpiarPanel();

        List<Usuario> usuarios = null;
        string url = serverBaseUrl + apiEndpoint;

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = "{\"usuarios\":" + request.downloadHandler.text + "}";
            // Guardar el JSON descargado en el archivo local
            File.WriteAllText(RutaLocal, json);

            UsuarioList lista = JsonUtility.FromJson<UsuarioList>(json);
            usuarios = lista?.usuarios ?? new List<Usuario>();
        }
        else
        {
            if (File.Exists(RutaLocal))
            {
                // Si hay un error al descargar del servidor, se carga desde el archivo local
                string json = File.ReadAllText(RutaLocal);
                UsuarioList lista = JsonUtility.FromJson<UsuarioList>(json);
                usuarios = lista?.usuarios ?? new List<Usuario>();
            }
            else
            {
                usuarios = new List<Usuario>();
            }
        }

        if (usuarios != null && usuarios.Count > 0)
        {
            yield return StartCoroutine(MostrarUsuarios(usuarios));
        }
    }

    void LimpiarPanel()
    {
        foreach (Transform child in panelContenedor)
            Destroy(child.gameObject);
    }

    // Método para mostrar los usuarios en el panel.
    IEnumerator MostrarUsuarios(List<Usuario> usuarios)
    {
        int contador = 0; // Contador para limitar la cantidad de usuarios mostrados por frame

        PanelManager panelManager = Object.FindFirstObjectByType<PanelManager>(); // Reemplazo de método obsoleto

        foreach (Usuario u in usuarios)
        {
            GameObject boton = Instantiate(botonPrefab, panelContenedor);
            TMP_Text texto = boton.GetComponentInChildren<TMP_Text>();
            if (texto != null) texto.text = u.name;

            if (!string.IsNullOrEmpty(u.photoUrl))
            {
                string fullImageUrl = serverBaseUrl + u.photoUrl;
                yield return StartCoroutine(ObtenerImagenLocalODescargar(fullImageUrl, boton));
            }
            else
            {
                AsignarSpriteAlBoton(boton, spriteDefault);
            }

            if (++contador % 5 == 0) // Limitar a 5 usuarios por frame
                yield return null;

            BotonUsuario botonUsuario = boton.GetComponent<BotonUsuario>();
            if (botonUsuario != null)
            {
                botonUsuario.usuarioId = u.id;
                botonUsuario.servidorBase = serverBaseUrl;
                botonUsuario.panelManager = panelManager; // Asignación aquí
            }
        }
    }

    IEnumerator ObtenerImagenLocalODescargar(string url, GameObject boton)
    {
        string nombreArchivo = Path.GetFileName(url);
        string rutaLocal = Path.Combine(RutaImagenes, nombreArchivo);

        if (File.Exists(rutaLocal))
        {
            byte[] datos = File.ReadAllBytes(rutaLocal);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(datos);
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            AsignarSpriteAlBoton(boton, sprite);
            yield break;
        }

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(rutaLocal, bytes);
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            AsignarSpriteAlBoton(boton, sprite);
        }
        else
        {
            Debug.LogWarning("[UsuarioLoader] Error al descargar imagen: " + request.error);
        }
    }

    IEnumerator CargarImagenDefault()
    {
        string url = serverBaseUrl + "/" + NombreImagenDefault;
        string rutaLocal = Path.Combine(RutaImagenes, NombreImagenDefault);

        if (File.Exists(rutaLocal))
        {
            byte[] datos = File.ReadAllBytes(rutaLocal);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(datos);
            spriteDefault = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            yield break;
        }

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D tex = DownloadHandlerTexture.GetContent(request);
            byte[] bytes = tex.EncodeToPNG();
            File.WriteAllBytes(rutaLocal, bytes);
            spriteDefault = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            Debug.LogWarning("[UsuarioLoader] No se pudo descargar la imagen por defecto: " + request.error);
            spriteDefault = null;
        }
    }

    void AsignarSpriteAlBoton(GameObject boton, Sprite sprite)
    {
        Image img = boton.GetComponentInChildren<Image>();
        if (img != null && sprite != null)
        {
            img.sprite = sprite;
            img.preserveAspect = true;
        }
    }
}

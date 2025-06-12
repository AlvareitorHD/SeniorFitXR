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
    public Sprite spriteLocal;

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

        List<Usuario> usuariosLocales = CargarUsuariosLocales();
        List<Usuario> usuariosServidor = null;

        // 1. Descargar usuarios del servidor
        string url = serverBaseUrl + apiEndpoint;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = "{\"usuarios\":" + request.downloadHandler.text + "}";
            UsuarioList listaServidor = JsonUtility.FromJson<UsuarioList>(json);
            usuariosServidor = listaServidor?.usuarios ?? new List<Usuario>();

            // 2. Subir usuarios locales no sincronizados (id negativo)
            foreach (Usuario u in usuariosLocales)
            {
                if (u.id < 0)
                {
                    WWWForm form = new WWWForm();
                    form.AddField("name", u.name);
                    // Redondear a 2 decimales para evitar problemas de precisión
                    int alturaCm = Mathf.RoundToInt(u.height * 100f);
                    form.AddField("height", alturaCm);
                    form.AddField("fechaRegistro", u.fechaRegistro);

                    form.AddField("puntosTotales", u.puntosTotales);
                    form.AddField("puntosSesion", u.puntosSesion);
                    form.AddField("numeroSesiones", u.numeroSesiones);
                    form.AddField("tiempoTotalEjercicio", u.tiempoTotalEjercicio.ToString(System.Globalization.CultureInfo.InvariantCulture));
                    
                    // En el futuro, podría serializar logros si es necesario

                    // Registrar usuario local en el servidor
                    UnityWebRequest post = UnityWebRequest.Post(serverBaseUrl + "/api/register", form);
                    yield return post.SendWebRequest();

                    if (post.result == UnityWebRequest.Result.Success)
                    {
                        Usuario sincronizado = JsonUtility.FromJson<Usuario>(post.downloadHandler.text);
                        u.id = sincronizado.id;
                        u.photoUrl = sincronizado.photoUrl;
                    }
                    else
                    {
                        Debug.LogWarning($"[UsuarioLoader] Falló al sincronizar usuario local {u.name}: {post.error}");
                    }
                }
            }

            // 3. Fusionar y guardar usuarios únicos localmente
            List<Usuario> fusionados = FusionarUsuarios(usuariosLocales, usuariosServidor);
            GuardarUsuariosLocales(fusionados);
            yield return StartCoroutine(MostrarUsuarios(fusionados));
        }
        else
        {
            Debug.LogWarning("[UsuarioLoader] Error al contactar con el servidor. Cargando solo usuarios locales.");
            yield return StartCoroutine(MostrarUsuarios(usuariosLocales));
        }
    }

    List<Usuario> CargarUsuariosLocales()
    {
        if (!File.Exists(RutaLocal)) return new List<Usuario>();

        string json = File.ReadAllText(RutaLocal);
        UsuarioList lista = JsonUtility.FromJson<UsuarioList>(json);
        return lista?.usuarios ?? new List<Usuario>();
    }

    void GuardarUsuariosLocales(List<Usuario> usuarios)
    {
        UsuarioList lista = new UsuarioList { usuarios = usuarios };
        string json = JsonUtility.ToJson(lista, true);
        File.WriteAllText(RutaLocal, json);
    }

    List<Usuario> FusionarUsuarios(List<Usuario> locales, List<Usuario> servidor)
    {
        Dictionary<int, Usuario> mapaServidor = new Dictionary<int, Usuario>();
        foreach (Usuario u in servidor)
        {
            mapaServidor[u.id] = u;
        }

        List<Usuario> fusionados = new List<Usuario>();

        foreach (Usuario local in locales)
        {
            // Si el usuario local ya no existe en el servidor y no es uno con ID negativo (temporal/local), lo eliminamos
            if (local.id > 0 && !mapaServidor.ContainsKey(local.id))
            {
                Debug.Log($"[UsuarioLoader] Eliminando usuario local eliminado del servidor: {local.name} (ID: {local.id})");
                EliminarUsuarioLocal(local);
                continue; // no se añade al resultado final
            }

            // Si aún existe, se mantiene (y puede sobrescribirse luego si viene del servidor)
            fusionados.Add(local);
        }

        // Agregamos usuarios del servidor (reemplazando si ya estaban)
        foreach (Usuario remoto in servidor)
        {
            int index = fusionados.FindIndex(u => u.id == remoto.id);
            if (index >= 0)
                fusionados[index] = remoto; // sobrescribe el existente
            else
                fusionados.Add(remoto);
        }

        return fusionados;
    }

    void EliminarUsuarioLocal(Usuario usuario)
    {
        // Eliminar foto si no es la default
        if (!string.IsNullOrEmpty(usuario.photoUrl) && !usuario.photoUrl.EndsWith(NombreImagenDefault))
        {
            string nombreFoto = Path.GetFileName(usuario.photoUrl);
            string rutaFoto = Path.Combine(RutaImagenes, nombreFoto);

            if (File.Exists(rutaFoto))
            {
                File.Delete(rutaFoto);
                Debug.Log($"[UsuarioLoader] Foto eliminada: {rutaFoto}");
            }
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
            spriteDefault = spriteLocal;
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

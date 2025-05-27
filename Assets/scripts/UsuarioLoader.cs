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
    public string serverBaseUrl = "https://192.168.1.252:3000";
    public string apiEndpoint = "/api/usuarios";

    private string RutaLocal => Path.Combine(Application.persistentDataPath, "usuarios.json");

    private void OnEnable()
    {
        StartCoroutine(CargarUsuariosUnicaVez());
    }

    IEnumerator CargarUsuariosUnicaVez()
    {
        LimpiarPanel();

        List<Usuario> usuarios = null;

        // Intentar cargar desde el servidor primero
        string url = serverBaseUrl + apiEndpoint;
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.certificateHandler = new BypassCertificate();

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = "{\"usuarios\":" + request.downloadHandler.text + "}";
            File.WriteAllText(RutaLocal, json);

            UsuarioList lista = JsonUtility.FromJson<UsuarioList>(json);
            usuarios = lista?.usuarios ?? new List<Usuario>();

            Debug.Log("Usuarios cargados desde servidor.");
        }
        else
        {
            Debug.LogWarning("No se pudo conectar al servidor. Cargando datos locales...");

            if (File.Exists(RutaLocal))
            {
                string json = File.ReadAllText(RutaLocal);
                UsuarioList lista = JsonUtility.FromJson<UsuarioList>(json);
                usuarios = lista?.usuarios ?? new List<Usuario>();
            }
            else
            {
                Debug.LogWarning("No hay datos locales disponibles.");
                usuarios = new List<Usuario>();
            }
        }

        // Mostrar usuarios desde la fuente seleccionada
        if (usuarios != null && usuarios.Count > 0)
            yield return StartCoroutine(MostrarUsuarios(usuarios));
    }

    void LimpiarPanel()
    {
        foreach (Transform child in panelContenedor)
            Destroy(child.gameObject);
    }

    IEnumerator MostrarUsuarios(List<Usuario> usuarios)
    {
        int contador = 0;

        foreach (Usuario u in usuarios)
        {
            GameObject boton = Instantiate(botonPrefab, panelContenedor);
            TMP_Text texto = boton.GetComponentInChildren<TMP_Text>();
            if (texto != null) texto.text = u.name;

            if (!string.IsNullOrEmpty(u.photoUrl))
            {
                string fullImageUrl = serverBaseUrl + u.photoUrl;
                yield return StartCoroutine(DescargarImagen(fullImageUrl, boton));
            }

            if (++contador % 5 == 0)
                yield return null;
        }
    }

    IEnumerator DescargarImagen(string url, GameObject boton)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        request.certificateHandler = new BypassCertificate();
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("Error al descargar imagen: " + request.error);
            yield break;
        }

        Texture2D tex = DownloadHandlerTexture.GetContent(request);
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));

        Image img = boton.GetComponentInChildren<Image>();
        if (img != null)
        {
            img.sprite = sprite;
            img.preserveAspect = true;
        }
    }
}

/*using UnityEngine;
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
    public string apiEndpoint = "/api/usuarios";
    private string serverBaseUrl => ServerDiscovery.Instance.serverUrl;

    private string RutaLocal => Path.Combine(Application.persistentDataPath, "usuarios.json");

    private void OnEnable()
    {
        StartCoroutine(EsperarYObtenerUsuarios());
    }

    IEnumerator EsperarYObtenerUsuarios()
    {
        // Esperar hasta que la IP del servidor est� disponible
        float timeout = 10f;
        float tiempo = 0f;

        while (string.IsNullOrEmpty(serverBaseUrl) && tiempo < timeout)
        {
            yield return new WaitForSeconds(0.5f);
            tiempo += 0.5f;
        }

        if (string.IsNullOrEmpty(serverBaseUrl))
        {
            Debug.LogWarning("No se encontr� el servidor. Cargando datos locales...");
        }

        yield return StartCoroutine(CargarUsuariosUnicaVez());
    }

    IEnumerator CargarUsuariosUnicaVez()
    {
        LimpiarPanel();

        List<Usuario> usuarios = null;

        if (!string.IsNullOrEmpty(serverBaseUrl))
        {
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
                Debug.LogWarning("Fallo al conectar al servidor. Cargando datos locales...");
            }
        }

        // Si no hay conexi�n, cargar datos locales
        if (usuarios == null)
        {
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

        if (usuarios.Count > 0)
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

            if (!string.IsNullOrEmpty(u.photoUrl) && !string.IsNullOrEmpty(serverBaseUrl))
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
*/
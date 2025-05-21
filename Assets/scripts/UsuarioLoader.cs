using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;  // <-- Importante para TextMeshPro

public class UsuarioLoader : MonoBehaviour
{
    [Header("Configuración UI")]
    public Transform panelContenedor;     // Panel donde se instanciarán los botones
    public GameObject botonPrefab;        // Prefab del botón

    [Header("Configuración de Red")]
    public string serverBaseUrl = "https://192.168.1.252:3000"; // IP del servidor Node.js
    public string apiEndpoint = "/api/usuarios";               // Endpoint API

    void Start()
    {
        Debug.Log("Iniciando carga de usuarios...");
        StartCoroutine(CargarUsuarios());
    }

    IEnumerator CargarUsuarios()
    {
        string fullUrl = serverBaseUrl + apiEndpoint;

        UnityWebRequest www = UnityWebRequest.Get(fullUrl);
        www.certificateHandler = new BypassCertificate(); // Ignora errores de certificado
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error al cargar usuarios: " + www.error);
        }
        else
        {
            string rawJson = www.downloadHandler.text;
            Debug.Log("Respuesta JSON cruda: " + rawJson);

            // Adaptar el JSON si es un array plano
            string wrappedJson = "{\"usuarios\":" + rawJson + "}";

            UsuarioList lista = JsonUtility.FromJson<UsuarioList>(wrappedJson);

            foreach (Usuario u in lista.usuarios)
            {
                GameObject boton = Instantiate(botonPrefab, panelContenedor);

                // Asignar nombre con TextMeshPro
                TMP_Text textoNombre = boton.GetComponentInChildren<TMP_Text>();
                if (textoNombre != null)
                {
                    textoNombre.text = u.name;
                }
                else
                {
                    Debug.LogWarning("No se encontró componente TMP_Text en el prefab.");
                }

                // Cargar y asignar imagen solo si hay URL válida
                if (!string.IsNullOrEmpty(u.photoUrl))
                {
                    string fullImageUrl = serverBaseUrl + u.photoUrl;
                    Debug.Log("Cargando imagen desde: " + fullImageUrl);
                    StartCoroutine(CargarImagen(fullImageUrl, boton));
                }
            }

            Debug.Log("Usuarios cargados: " + lista.usuarios.Count);
        }
    }

IEnumerator CargarImagen(string url, GameObject boton)
{
    UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        www.certificateHandler = new BypassCertificate(); // Ignora errores de certificado
        yield return www.SendWebRequest();

    if (www.result == UnityWebRequest.Result.Success)
    {
        Texture2D texture = DownloadHandlerTexture.GetContent(www);

        Image img = boton.GetComponentInChildren<Image>();
        if (img != null)
        {
            // Convertir Texture2D a Sprite
            Sprite sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));
            img.sprite = sprite;
        }
        else
        {
            Debug.LogWarning("No se encontró componente Image en el prefab.");
        }
    }
    else
    {
        Debug.Log("Error al cargar imagen: " + www.error);
    }
}

}

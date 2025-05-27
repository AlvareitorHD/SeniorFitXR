/*using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;

public class UsuarioLoader : MonoBehaviour
{
    [Header("Configuración UI")]
    public Transform panelContenedor;
    public GameObject botonPrefab;

    [Header("Configuración de Red")]
    public string serverBaseUrl = "https://192.168.1.252:3000";
    public string apiEndpoint = "/api/usuarios";

    void Start()
    { 
        
    }

    private void OnEnable()
    {
        ServicePointManager.ServerCertificateValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
        StartCoroutine(CargarUsuarios());
    }

    IEnumerator CargarUsuarios()
    {

        // Primero verificamos si el contenedor está vacío
        if (panelContenedor.childCount > 0)
        {
            foreach (Transform child in panelContenedor)
            {
                Destroy(child.gameObject);
            }
        }

        string url = serverBaseUrl + apiEndpoint;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        string json = null;

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
        {
            json = reader.ReadToEnd();
        }

        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("No se recibió JSON del servidor");
            yield break;
        }

        // Si es un array plano, envolverlo
        string wrappedJson = "{\"usuarios\":" + json + "}";
        UsuarioList lista = JsonUtility.FromJson<UsuarioList>(wrappedJson);

        foreach (Usuario u in lista.usuarios)
        {
            GameObject boton = Instantiate(botonPrefab, panelContenedor);

            // Asignar texto
            TMP_Text texto = boton.GetComponentInChildren<TMP_Text>();
            if (texto != null)
                texto.text = u.name;

            // Cargar imagen
            if (!string.IsNullOrEmpty(u.photoUrl))
            {
                string fullImageUrl = serverBaseUrl + u.photoUrl;
                yield return StartCoroutine(DescargarImagen(fullImageUrl, boton));
            }
        }
    }

    IEnumerator DescargarImagen(string url, GameObject boton)
    {
        HttpWebRequest imageRequest = (HttpWebRequest)WebRequest.Create(url);
        imageRequest.Method = "GET";
        imageRequest.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

        HttpWebResponse imageResponse = (HttpWebResponse)imageRequest.GetResponse();

        using (Stream stream = imageResponse.GetResponseStream())
        using (MemoryStream ms = new MemoryStream())
        {
            stream.CopyTo(ms);
            byte[] data = ms.ToArray();

            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(data);

            Sprite sprite = Sprite.Create(texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f));

            Image img = boton.GetComponentInChildren<Image>();
            if (img != null)
            {
                img.sprite = sprite;
                img.preserveAspect = true;
            }
        }

        yield return null;
    }
}*/
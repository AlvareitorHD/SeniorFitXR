using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Net;
using System.IO;
using System.Text;
using System;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class RegistroUsuario : MonoBehaviour
{
    [Header("Campos de entrada")]
    public TMP_InputField inputNombre;
    public Slider scrollAltura;
    public TextMeshProUGUI textoAlturaPreview;

    [Header("Botón de registro")]
    public Button btnAceptar;
    public Color colorActivo = new Color(0.2f, 0.8f, 0.2f);
    public Color colorInactivo = new Color(0.5f, 0.5f, 0.5f, 1f);

    [Header("Altura")]
    private float alturaSeleccionada = 1.50f;

    [Header("Red")]
    public string serverUrl = "https://192.168.1.252:3000/register";

    void Start()
    {
        inputNombre.text = "Usuario"; // BORRAR DESPUES YA QUE PARA PRUEBAS
        inputNombre.onValueChanged.AddListener(ValidarNombre);
        scrollAltura.onValueChanged.AddListener(v => ActualizarAltura(v));
        ValidarNombre(inputNombre.text);
    }

    public void AceptarRegistro()
    {
        string nombre = inputNombre.text.Trim();
        if (string.IsNullOrEmpty(nombre)) return;

        // Convertir la altura a cm
        int alturaCm = Mathf.RoundToInt(alturaSeleccionada * 100f);
        // Print
        Debug.Log("Altura seleccionada: " + alturaSeleccionada);
        
        // Formulario para enviar por POST
        WWWForm form = new WWWForm();
        form.AddField("name", nombre);
        form.AddField("height", alturaCm);

        StartCoroutine(EnviarFormulario(form));
    }

    IEnumerator EnviarFormulario(WWWForm form)
    {
        UnityWebRequest www = UnityWebRequest.Post(serverUrl, form);
        www.certificateHandler = new BypassCertificate(); // Si es necesario ignorar certificado
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Registro enviado correctamente: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error al registrar usuario: " + www.error);
        }

        inputNombre.text = "";
        scrollAltura.value = 0f;

        // Buscar en la escena el objeto vacio gestorUI y usar su clase PanelManager
        GameObject gestorUI = GameObject.Find("GestorUI");
        if (gestorUI != null)
        {
            PanelManager panelManager = gestorUI.GetComponent<PanelManager>();
            if (panelManager != null)
            {
                panelManager.MostrarInicio();
            }
            else
            {
                Debug.LogError("No se encontró el componente PanelManager en gestorUI.");
            }
        }
        else
        {
            Debug.LogError("No se encontró el objeto gestorUI en la escena.");
        }

        // Crear usuario localmente
        Usuario nuevoUsuario = new Usuario(inputNombre.text.Trim(), alturaSeleccionada);
        GuardarUsuarioLocal(nuevoUsuario);

    }

    private void ActualizarAltura(float valorScrollbar)
    {
        alturaSeleccionada = valorScrollbar;
        textoAlturaPreview.text = $"{alturaSeleccionada:F2} m";
    }

    private void ValidarNombre(string texto)
    {
        bool esValido = !string.IsNullOrWhiteSpace(texto);
        btnAceptar.interactable = esValido;

        ColorBlock colorBlock = btnAceptar.colors;
        colorBlock.normalColor = esValido ? colorActivo : colorInactivo;
        colorBlock.highlightedColor = esValido ? colorActivo : colorInactivo;
        colorBlock.pressedColor = esValido ? colorActivo : colorInactivo;
        colorBlock.selectedColor = esValido ? colorActivo : colorInactivo;
        colorBlock.disabledColor = colorInactivo;
        btnAceptar.colors = colorBlock;
    }

    private void GuardarUsuarioLocal(Usuario usuario)
    {
        string ruta = Path.Combine(Application.persistentDataPath, "usuarios.json");
        UsuarioList lista;

        if (File.Exists(ruta))
        {
            string json = File.ReadAllText(ruta);
            lista = JsonUtility.FromJson<UsuarioList>(json);
        }
        else
        {
            lista = new UsuarioList { usuarios = new List<Usuario>() };
        }

        lista.usuarios.Add(usuario);
        string nuevoJson = JsonUtility.ToJson(lista, true);
        File.WriteAllText(ruta, nuevoJson);

        Debug.Log("Usuario guardado localmente en: " + ruta);
    }

}
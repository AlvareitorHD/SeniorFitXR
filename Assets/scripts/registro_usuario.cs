using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

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
    public string serverUrl = "https://pegasus-powerful-imp.ngrok-free.app/register";

    void Start()
    {
        //inputNombre.text = "Usuario"; // Solo para pruebas
        inputNombre.onValueChanged.AddListener(ValidarNombre);
        scrollAltura.onValueChanged.AddListener(ActualizarAltura);

        ActualizarAltura(scrollAltura.value);
        ValidarNombre(inputNombre.text);
    }

    public void AceptarRegistro()
    {
        string nombre = inputNombre.text.Trim();
        if (string.IsNullOrEmpty(nombre)) return;

        int alturaCm = Mathf.RoundToInt(alturaSeleccionada * 100f);
        Debug.Log($"Nombre: {nombre}, Altura: {alturaCm} cm");

        WWWForm form = new WWWForm();
        form.AddField("name", nombre);
        form.AddField("height", alturaCm);
        // Fecha de registro local
        form.AddField("fechaRegistro", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        StartCoroutine(EnviarFormulario(form, nombre, alturaSeleccionada));
    }

    IEnumerator EnviarFormulario(WWWForm form, string nombre, float altura)
    {
        UnityWebRequest www = UnityWebRequest.Post(serverUrl, form);
        // www.certificateHandler = new BypassCertificate(); // opcional con HTTPS self-signed
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Registro enviado correctamente: " + www.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error al registrar usuario: " + www.error);
        }

        // Guardar usuario local solo si la petición fue exitosa
        Usuario nuevoUsuario = new Usuario(nombre, altura)
        {
            photoUrl = "/default.png", // Aquí puedes asignar una URL de foto si es necesario
            id = 0, // Asigna un ID si es necesario, o maneja esto en el servidor
        };

        GuardarUsuarioLocal(nuevoUsuario);


        // Limpiar entradas
        inputNombre.text = "";
        scrollAltura.value = 1.5f;
        // Cambiar al panel de inicio
        GameObject gestorUI = GameObject.Find("GestorUI");
        if (gestorUI != null)
        {
            PanelManager panelManager = gestorUI.GetComponent<PanelManager>();
            panelManager?.MostrarUsuarios();
        }

    }

    private void ActualizarAltura(float valor)
    {
        alturaSeleccionada = valor;
        textoAlturaPreview.text = $"{alturaSeleccionada:F2} m";
    }

    private void ValidarNombre(string texto)
    {
        bool esValido = !string.IsNullOrWhiteSpace(texto);
        btnAceptar.interactable = esValido;

        ColorBlock colores = btnAceptar.colors;
        colores.normalColor = esValido ? colorActivo : colorInactivo;
        colores.highlightedColor = colores.normalColor;
        colores.pressedColor = colores.normalColor;
        colores.selectedColor = colores.normalColor;
        colores.disabledColor = colorInactivo;
        btnAceptar.colors = colores;
    }

    private void GuardarUsuarioLocal(Usuario usuario)
    {
        string ruta = Path.Combine(Application.persistentDataPath, "usuarios.json");
        UsuarioList lista = new UsuarioList();

        if (File.Exists(ruta))
        {
            string json = File.ReadAllText(ruta);
            lista = JsonUtility.FromJson<UsuarioList>(json);
        }

        // Si no existe la lista, inicializarla
        lista.usuarios ??= new List<Usuario>();
        lista.usuarios.Add(usuario);

        string nuevoJson = JsonUtility.ToJson(lista, true);
        File.WriteAllText(ruta, nuevoJson);

        Debug.Log("Usuario guardado localmente en: " + ruta);
    }
}

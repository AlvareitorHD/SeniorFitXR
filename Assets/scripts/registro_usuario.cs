using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

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
    public string serverUrl = "https://pegasus-powerful-imp.ngrok-free.app/api/register";

    void Start()
    {
        //inputNombre.text = "HOLA"; // Solo para pruebas
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
        yield return www.SendWebRequest();

        Usuario nuevoUsuario;

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Registro enviado correctamente: " + www.downloadHandler.text);

            // Leer respuesta del servidor
            nuevoUsuario = JsonUtility.FromJson<Usuario>(www.downloadHandler.text);
        }
        else
        {
            Debug.LogWarning("No se pudo registrar en el servidor. Se guardará localmente.");

            // Asignar ID temporal negativo basado en timestamp
            int idTemporal = -Mathf.Abs(DateTime.Now.Ticks.GetHashCode());

            nuevoUsuario = new Usuario(nombre, altura)
            {
                id = idTemporal,
                photoUrl = "/default.png", // ruta local
                fechaRegistro = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            };
        }

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

        // Inicializar lista si es null
        lista.usuarios ??= new List<Usuario>();

        // Reemplazar usuario si ya existe uno con el mismo ID
        int index = lista.usuarios.FindIndex(u => u.id == usuario.id);
        if (index >= 0)
            lista.usuarios[index] = usuario;
        else
            lista.usuarios.Add(usuario);

        string nuevoJson = JsonUtility.ToJson(lista, true);
        File.WriteAllText(ruta, nuevoJson);

        Debug.Log("Usuario guardado localmente en: " + ruta);
    }
}

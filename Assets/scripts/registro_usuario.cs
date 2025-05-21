using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RegistroUsuario : MonoBehaviour
{
    [Header("Campos de entrada")]
    public TMP_InputField inputNombre;
    public Slider scrollAltura;
    public TextMeshProUGUI textoAlturaPreview;

    [Header("Botón de registro")]
    public Button btnAceptar;
    public Color colorActivo = new Color(0.2f, 0.8f, 0.2f); // Verde
    public Color colorInactivo = new Color(0.5f, 0.5f, 0.5f, 1f); // Gris

    [Header("Altura")]
    public float alturaMin = 1.00f;
    public float alturaMax = 2.00f;
    private float alturaSeleccionada = 1.00f;

    void Start()
    {
        inputNombre.onValueChanged.AddListener(ValidarNombre);
        scrollAltura.onValueChanged.AddListener(ActualizarAltura);

        ActualizarAltura(scrollAltura.value);
        ValidarNombre(inputNombre.text);
    }

    public void AceptarRegistro()
    {
        string nombre = inputNombre.text.Trim();
        if (string.IsNullOrEmpty(nombre)) return;

        Debug.Log($"Usuario registrado: {nombre} ({alturaSeleccionada:F2} m)");

        Usuario nuevo = new Usuario(nombre, alturaSeleccionada);
        //GestorUsuarios.Registrar(nuevo);

        inputNombre.text = "";
        scrollAltura.value = 1f;
    }

    private void ActualizarAltura(float valorScrollbar)
    {
        alturaSeleccionada = Mathf.Lerp(alturaMin, alturaMax, valorScrollbar);
        textoAlturaPreview.text = $"{alturaSeleccionada:F2} m";
    }

    private void ValidarNombre(string texto)
    {
        bool esValido = !string.IsNullOrWhiteSpace(texto);

        btnAceptar.interactable = esValido;

        // Cambiar el color del botón según validez
        ColorBlock colorBlock = btnAceptar.colors;
        colorBlock.normalColor = esValido ? colorActivo : colorInactivo;
        colorBlock.highlightedColor = esValido ? colorActivo : colorInactivo;
        colorBlock.pressedColor = esValido ? colorActivo : colorInactivo;
        colorBlock.selectedColor = esValido ? colorActivo : colorInactivo;
        colorBlock.disabledColor = colorInactivo; // <- AÑADIDO

        btnAceptar.colors = colorBlock;
    }

}

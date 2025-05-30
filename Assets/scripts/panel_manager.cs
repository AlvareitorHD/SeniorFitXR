using UnityEngine;
using UnityEngine.UI;

public class PanelManager : MonoBehaviour
{
    [Header("Paneles de UI")]
    public GameObject panelInicio;
    public GameObject panelRegistro;
   // public GameObject panelOpciones;
    public GameObject panelUsuarios;
    public GameObject panelEjercicios;
    // Puedes añadir más paneles aquí...

    void Start()
    {
        MostrarSolo(panelInicio);
        panelRegistro.SetActive(false);
        //panelOpciones.SetActive(false);
        panelUsuarios.SetActive(false);
        panelEjercicios.SetActive(false);

    }

    public void MostrarInicio()
    {
        MostrarSolo(panelInicio);
    }

    public void MostrarRegistro()
    {
        MostrarSolo(panelRegistro);
    }

   /* public void MostrarOpciones()
    {
        MostrarSolo(panelOpciones);
    }*/

    public void MostrarUsuarios()
    {
        MostrarSolo(panelUsuarios);
    }

    public void MostrarEjercicios()
    {
        MostrarSolo(panelEjercicios);

        // Poner la foto y nombre del usuario actual en el objeto cabecera -> usuario del panel de ejercicios
        if (UsuarioGlobal.Instance.usuarioActual != null)
        {
            GameObject usuarioPanel = panelEjercicios.transform.Find("cabecera/usuario")?.gameObject;
            if (usuarioPanel == null)
            {
                Debug.LogError("No se encontró el objeto 'usuario' dentro de panelEjercicios.");
                return;
            }

            // Asignar nombre del usuario
            var nombreText = usuarioPanel.transform.Find("nombre_usuario")?.GetComponent<TMPro.TextMeshProUGUI>();
            if (nombreText != null)
            {
                nombreText.text = UsuarioGlobal.Instance.usuarioActual.name;
            }
            else
            {
                Debug.LogWarning("No se encontró el TextMeshProUGUI 'nombre_usuario'.");
            }

            // Cargar y asignar la imagen del usuario
            var fotoImage = usuarioPanel.transform.Find("Foto")?.GetComponent<RawImage>();
            if (fotoImage != null)
            {
                // QUitar "/uploads/" de la URL de la imagen
                string nombreIMG = UsuarioGlobal.Instance.usuarioActual.photoUrl;
                if (nombreIMG.StartsWith("/uploads/"))
                {
                    nombreIMG = nombreIMG.Substring(9);
                }
                string rutaImagen = System.IO.Path.Combine(Application.persistentDataPath, "imagenes", nombreIMG);
                if (System.IO.File.Exists(rutaImagen))
                {
                    byte[] imageData = System.IO.File.ReadAllBytes(rutaImagen);
                    Texture2D texture = new Texture2D(2, 2);
                    texture.LoadImage(imageData);
                    fotoImage.texture = texture;
                }
                else
                {
                    Debug.LogWarning($"No se encontró la imagen del usuario en {rutaImagen}. Usando imagen por defecto.");
                    if(fotoImage.texture != null)
                    {
                        
                        
                    }
                }
            }
            else
            {
                Debug.LogWarning("No se encontró el componente RawImage 'Foto'.");
            }
        }
        else
        {
            Debug.LogWarning("No hay un usuario actual establecido.");
        }
    }


    private void MostrarSolo(GameObject panelActivo)
    {
        panelInicio.SetActive(panelActivo == panelInicio);
        panelRegistro.SetActive(panelActivo == panelRegistro);
        //panelOpciones.SetActive(panelActivo == panelOpciones);
        panelUsuarios.SetActive(panelActivo == panelUsuarios);
        panelEjercicios.SetActive(panelActivo == panelEjercicios);
        // Añade aquí más paneles si agregas más
    }

    // Método para los input fields que hacen abrir el teclado virtual de Meta Quest
    public void AbrirTecladoVirtual()
    {
        // Abre el teclado virtual de Meta Quest
        TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    // Método para el botón salir, que cierra la aplicación
    public void Salir()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}

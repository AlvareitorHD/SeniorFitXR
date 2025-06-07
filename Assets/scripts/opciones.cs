using UnityEngine;
using TMPro;

public class Opciones : MonoBehaviour
{
    // Calidad de Gráficos
    public TMP_Dropdown qualityDropdown; // Asigna el Dropdown en el Inspector
    // 

    void Start()
    {
        // Limpiar y llenar el dropdown con los nombres de los niveles de calidad
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));

        // Establecer el valor actual según el nivel actual
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        // Agregar el listener
        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        Debug.Log("Calidad cambiada a: " + QualitySettings.names[index]);
        
        PlayerPrefs.SetInt("QualityLevel", index); // Guardar el nivel de calidad en PlayerPrefs
        PlayerPrefs.Save(); // Asegurarse de guardar los cambios
    }
}

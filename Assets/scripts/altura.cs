using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class altura : MonoBehaviour

{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.text = "Altura: 1.00 metros";
        slider.onValueChanged.AddListener(v =>
        {
            // Actualiza el texto del slider a "Altura: 0.00 metros"
            text.text = "Altura: " + v.ToString("0.00") + " metros";
        });

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

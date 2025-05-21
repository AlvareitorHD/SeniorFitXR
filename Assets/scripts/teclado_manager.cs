using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class OpenKeyboardOnSelect : MonoBehaviour, ISelectHandler
{
    public TMP_InputField inputField;
    private TouchScreenKeyboard keyboard;

    public void OnSelect(BaseEventData eventData)
    {
        if (keyboard == null || !keyboard.active)
        {
            keyboard = TouchScreenKeyboard.Open(inputField.text, TouchScreenKeyboardType.Default);
        }
    }

    void Update()
    {
        if (keyboard != null && keyboard.active)
        {
            inputField.text = keyboard.text;
        }
    }
}

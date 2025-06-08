using UnityEngine;

public class DanceTile : MonoBehaviour
{
    public Renderer tileRenderer;
    public Color idleColor = Color.white;
    private Color activeColor;
    private bool isActive = false;

    public DanceManager danceManager; // ¡NECESARIO!

    public void Activate(Color color)
    {
        activeColor = color;
        isActive = true;

        // Cambia el color base
        tileRenderer.material.color = activeColor;

        // También aplica emisivo si el material lo soporta
        if (tileRenderer.material.HasProperty("_EmissionColor"))
        {
            tileRenderer.material.EnableKeyword("_EMISSION");
            tileRenderer.material.SetColor("_EmissionColor", activeColor * 2f);
        }
    }

    public void Deactivate()
    {
        isActive = false;
        tileRenderer.material.color = idleColor;

        if (tileRenderer.material.HasProperty("_EmissionColor"))
        {
            tileRenderer.material.SetColor("_EmissionColor", Color.black);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (other.CompareTag("PlayerFoot"))
        {
            if (danceManager != null)
            {
                danceManager.PlayerSteppedCorrectTile(this);
            }
            else
            {
                Debug.LogWarning("DanceManager no asignado en tile.");
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isActive) return;
        if (other.CompareTag("PlayerFoot"))
        {
            if (danceManager != null)
            {
                danceManager.PlayerSteppedCorrectTile(this);
            }
            else
            {
                Debug.LogWarning("DanceManager no asignado en tile.");
            }
        }

    }

    public bool IsActive()
    {
        return isActive;
    }
}

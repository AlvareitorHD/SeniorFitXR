using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private void Awake()
    {
        // Si ya existe otra instancia, destruye este duplicado
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Hace que persista entre escenas
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    public AudioSource audioSource;
    public List<SceneMusic> sceneMusicList;

    private Dictionary<string, AudioClip> musicByScene;

    private void Awake()
    {
        // Singleton: evita duplicados
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Crear el diccionario para acceso rápido
        musicByScene = new Dictionary<string, AudioClip>();
        foreach (var sceneMusic in sceneMusicList)
        {
            musicByScene[sceneMusic.sceneName] = sceneMusic.musicClip;
        }

        // Escuchar cambios de escena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (musicByScene.TryGetValue(scene.name, out AudioClip newClip))
        {
            if (audioSource.clip != newClip)
            {
                audioSource.clip = newClip;
                audioSource.Play();
            }
        }
    }
}

[System.Serializable]
public class SceneMusic
{
    public string sceneName;
    public AudioClip musicClip;
}

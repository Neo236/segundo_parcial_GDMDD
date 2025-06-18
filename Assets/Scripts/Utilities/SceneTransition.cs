using UnityEngine;
using UnityEngine.Serialization;

public class SceneTransition : MonoBehaviour
{
    [Tooltip("El nombre de la escena a cargar. ¡Debe estar en Build Settings!")]
    public SceneField sceneToLoad;

    [Tooltip("El ID del objeto 'SpawnPoint' en la nueva escena.")]
    public SpawnPointID targetSpawnPointID;
    
    [Header("Audio Management")]
    [Tooltip("La música que debe sonar en la nueva escena. Déjalo en None si no quieres cambiar la música.")]
    public AudioClip newMusicTrack;

    [Tooltip("Marca esto si quieres que la música se detenga por completo al entrar (ej. para una zona de silencio).")]
    public bool stopMusicOnEnter = false;

    private static bool _isTransitioning = false; 

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_isTransitioning)
        {
            string currentScene = gameObject.scene.name;
            GameManager.Instance.TransitionToScene(sceneToLoad, targetSpawnPointID, currentScene, newMusicTrack, stopMusicOnEnter);
        }
    }

    public static void StartTransition()
    {
        _isTransitioning = true;
    }

    public static void EndTransition()
    {
        _isTransitioning = false;
    }
}

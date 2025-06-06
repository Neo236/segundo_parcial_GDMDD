using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    [Tooltip("El nombre de la escena a cargar. ¡Debe estar en Build Settings!")]
    public string sceneToLoad;

    [Tooltip("El nombre del objeto 'SpawnPoint' en la nueva escena.")]
    public string targetSpawnPointName;

    private static bool isTransitioning = false; 

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTransitioning)
        {
            string currentScene = gameObject.scene.name;
            GameManager.Instance.TransitionToScene(sceneToLoad, targetSpawnPointName, currentScene);
        }
    }

    public static void StartTransition()
    {
        isTransitioning = true;
    }

    public static void EndTransition()
    {
        isTransitioning = false;
    }
}

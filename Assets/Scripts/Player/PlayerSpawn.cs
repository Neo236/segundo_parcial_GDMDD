using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawn : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive)
        {
            Debug.Log($"Se ha cargado la escena {scene.name}. Buscando punto de spawn '{GameManager.Instance.nextSpawnPointName}'");

            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
            bool foundSpawn = false;

            foreach (GameObject spawnPoint in spawnPoints)
            {
                if (spawnPoint.name == GameManager.Instance.nextSpawnPointName)
                {
                    transform.position = spawnPoint.transform.position;
                    Debug.Log($"Jugador movido a {spawnPoint.name}");
                    foundSpawn = true;
                    break;
                }
            }

            if (!foundSpawn)
            {
                Debug.LogWarning($"No se encontró el punto de spawn llamado '{GameManager.Instance.nextSpawnPointName}' en la escena '{scene.name}'.");
            }
        }
    }
}

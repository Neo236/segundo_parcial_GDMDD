using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerSpawn : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Additive)
        {
            StartCoroutine(FindAndMoveToSpawnPoint(scene));
        }
    }
    
    private IEnumerator FindAndMoveToSpawnPoint(Scene scene)
    {
        yield return new WaitForEndOfFrame();

        if (GameManager.Instance.nextSpawnPointID == null)
        {
            Debug.LogWarning("No se ha definido un punto de spawn para el jugador.");
            yield break;       
        }
        
        Debug.Log($"Buscando punto de spawn ID: {GameManager.Instance.nextSpawnPointID.name}");
        
        SpawnPointIdentifier[] spawnPoints = FindObjectsOfType<SpawnPointIdentifier>();
        bool foundSpawn = false;

        foreach (SpawnPointIdentifier spawnPoint in spawnPoints)
        {
            if (spawnPoint.spawnPointID == GameManager.Instance.nextSpawnPointID)
            {
                transform.position = spawnPoint.transform.position;
                Debug.Log($"Jugador movido a {spawnPoint.spawnPointID.name}");
                foundSpawn = true;
                break;
            }
        }

        if (!foundSpawn)
        {
            Debug.Log($"No se encontró punto de spawn ID: {GameManager.Instance.nextSpawnPointID.name}");
        }
    }
}

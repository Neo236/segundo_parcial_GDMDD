using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private EnemyDistance[] enemies;
    [SerializeField] private string endSceneName = "End"; // Nombre de la escena final

    private bool gameEnded = false;

    void Update()
    {
        if (gameEnded) return;

        bool allDead = true;
        foreach (EnemyDistance enemy in enemies)
        {
            if (enemy != null && !enemy.IsDead)
            {
                allDead = false;
                break;
            }
        }

        if (allDead)
        {
            gameEnded = true;
            Debug.Log("All enemies defeated. Loading end scene...");
            SceneManager.LoadScene(endSceneName);
        }
    }
}

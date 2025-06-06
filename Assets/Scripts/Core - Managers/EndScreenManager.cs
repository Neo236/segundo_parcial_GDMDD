using UnityEngine;
using UnityEngine.SceneManagement;
public class EndScreenManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void RestartGame()
    {
        // Reinicia el juego cargando la escena inicial
        SceneManager.LoadScene("TutorialLevel");
    }
}

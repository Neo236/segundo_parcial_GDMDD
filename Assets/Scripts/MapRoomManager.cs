using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapRoomManager : MonoBehaviour
{
    public static MapRoomManager Instance;

    private MapContainerData[] _rooms;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        _rooms = FindObjectsOfType<MapContainerData>(true);
    }

    public void RevealRoom(string loadedSceneName)
    {
        //string newLoadedScene = SceneManager.GetActiveScene().name;

        for (int i = 0; i < _rooms.Length; i++)
        {
            if (_rooms[i].roomScene.SceneName == loadedSceneName && !_rooms[i].HasBeenRevealed)
            {
                _rooms[i].gameObject.SetActive(true);
                _rooms[i].HasBeenRevealed = true;
                Debug.Log("Revealing room: " + _rooms[i].roomScene.SceneName);
                return;
            }
        }
    }
}

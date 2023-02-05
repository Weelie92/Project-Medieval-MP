using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.Networking;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class NetworkSceneManager : NetworkBehaviour
{
    [SerializeField] private Scene _lobbyScene;
    [SerializeField] private List<Scene> _minigameScenes;
    [SerializeField] private string _currentSceneName;
    [SerializeField] private string _nextSceneName;
    [SerializeField] private string _previousSceneName;
    
    private void Awake()
    {
        if (IsServer)
        {
            _currentSceneName = SceneManager.GetActiveScene().name;
        }
    }

    public void ChanceScene(string sceneName)
    {
        
    }

}

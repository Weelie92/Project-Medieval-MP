using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.Networking;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class SceneNames
{
    public const string Prototype = "Prototype";
    public const string Minigame1 = "MG_PlatformJump";
    public const string Lobby = "Lobby";
   
}

public class NetworkSceneManager : NetworkBehaviour
{
    public static NetworkSceneManager Instance;

    [SerializeField] private Button _startPrototype;
    [SerializeField] private Button _startMinigames;
    [SerializeField] private Button _startLobby;

    [SerializeField] private string _lobbySceneName;
    [SerializeField] private List<string> _minigameSceneNames;
    [SerializeField] private string _currentSceneName;
    [SerializeField] private string _nextSceneName;
    [SerializeField] private string _previousSceneName;

    private void Awake()
    {
        if (_startMinigames == null) return;

        Instance = this;

        _currentSceneName = SceneManager.GetActiveScene().name;

        _startMinigames.onClick.AddListener(ChangeSceneOnClick);
        _startLobby.onClick.AddListener(ChangeSceneOnClickLobby);
        _startPrototype.onClick.AddListener(ChangeSceneOnClickPrototype);
    }

    private void ChangeSceneOnClick()
    {
        if (!IsServer) return;

        ToggleChangingSceneServerRpc();

        int index = Random.Range(0, _minigameSceneNames.Count);
        NetworkManager.Singleton.SceneManager.LoadScene(_minigameSceneNames[index], LoadSceneMode.Single);
    }

    public void ChangeSceneOnClickLobby()
    {
        if (!IsServer) return;

        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClients.Values)
        {
            PlayerController playerController = client.PlayerObject.GetComponent<PlayerController>();

            playerController.isKnockbackable = true;
        }

        NetworkManager.Singleton.SceneManager.LoadScene(_lobbySceneName, LoadSceneMode.Single);
    }

    public void ChangeSceneOnClickPrototype()
    {
        {
        if (!IsServer) return;

        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClients.Values)
            {
            PlayerController playerController = client.PlayerObject.GetComponent<PlayerController>();

            playerController.isKnockbackable = true;
        }

        NetworkManager.Singleton.SceneManager.LoadScene(SceneNames.Prototype, LoadSceneMode.Single);
        }
    }

    [ServerRpc]
    private void ToggleChangingSceneServerRpc()
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClients.Values)
        {
            PlayerController playerController = client.PlayerObject.GetComponent<PlayerController>();

            playerController.isChangingScene = true;
        }

    }
}
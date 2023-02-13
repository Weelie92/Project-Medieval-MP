using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.Networking;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NetworkSceneManager : NetworkBehaviour
{
    [SerializeField] private Button _startMinigames;

    [SerializeField] private SceneAsset _lobbyScene;
    [SerializeField] private List<SceneAsset> _minigameScenes;
    [SerializeField] private string _currentSceneName;
    [SerializeField] private string _nextSceneName;
    [SerializeField] private string _previousSceneName;

    private void Awake()
    {
        if (_startMinigames == null) return;
        
            _currentSceneName = SceneManager.GetActiveScene().name;

            _startMinigames.onClick.AddListener(ChangeSceneOnClick);
        
    }

    private void ChangeSceneOnClick()
    {
        if (!IsServer) return;

        ToggleChangingSceneServerRpc();

        NetworkManager.Singleton.SceneManager.LoadScene(_minigameScenes[Random.Range(0, _minigameScenes.Count)].name, LoadSceneMode.Single);
    }

    [ServerRpc]
    private void ToggleChangingSceneServerRpc()
    {
        foreach (PlayerData playerData in GameObject.FindGameObjectWithTag("GameManager").GetComponent<ConnectedPlayers>()._playerList)
        {
            PlayerController player = NetworkManager.ConnectedClients[playerData.ClientId].PlayerObject.GetComponent<PlayerController>();

            player.isChangingScene = true;
        }
    }

   
}

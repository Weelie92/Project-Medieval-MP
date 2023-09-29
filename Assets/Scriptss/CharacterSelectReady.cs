using System;
using System.Collections;
using System.Collections.Generic;
using PsychoticLab;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour {


    public static CharacterSelectReady Instance { get; private set; }

    public GameObject[] lobbyReadyPlayers;



    public event EventHandler OnReadyChanged;


    private Dictionary<ulong, bool> playerReadyDictionary;


    private void Awake() {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        HandleConnectedClients();
    }


    public void SetPlayerReady() {
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default) {
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds) {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId]) {
                // This player is NOT ready
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady) {
            PmGameLobby.Instance.DeleteLobby();
            ToggleLoadScreenClientRpc();
            Loader.LoadNetwork(Loader.Scene.GameScene);
        }
    }

    [ClientRpc]
    private void ToggleLoadScreenClientRpc()
    {
        LoadingHUD.Instance.ToggleFade(true);
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId) {
        playerReadyDictionary[clientId] = true;

        OnReadyChanged?.Invoke(this, EventArgs.Empty);
    }


    public bool IsPlayerReady(ulong clientId) {
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }

    private void HandleConnectedClients()
    {
        return;

        var connectedClients = NetworkManager.Singleton.ConnectedClientsList;

        for (int i = 0; i < connectedClients.Count; i++)
        {
            GameObject player = lobbyReadyPlayers[i].gameObject;

            player.GetComponent<CharacterRandomizer>().Randomize();
        }
    }

}
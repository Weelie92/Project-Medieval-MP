using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectedPlayers : NetworkBehaviour
{
    public NetworkList<PlayerData> _playerList;

    private void Awake()
    {
        _playerList = new NetworkList<PlayerData>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }
        }
    }
    

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;

            
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        _playerList.Add(new PlayerData(clientId));
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        foreach (PlayerData player in _playerList)
        {
            if (player.ClientId == clientId)
            {
                _playerList.Remove(player);
                break;
            }
        }
    }
}

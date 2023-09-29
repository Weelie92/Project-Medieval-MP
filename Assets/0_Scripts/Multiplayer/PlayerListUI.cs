using System;
using System.Collections;
using System.Collections.Generic;
using QFSW.QC;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class PlayerListUI : MonoBehaviour
{
    public  Lobby lobbyId;

    [SerializeField] TextMeshProUGUI _lobbyName;
    [SerializeField] TextMeshProUGUI _lobbyPlayers;
    [SerializeField] TextMeshProUGUI _lobbyGameMode;

    [SerializeField] GameObject _playerListUI;
    [SerializeField] Transform _playerListContainer;

    [SerializeField] GameObject _playerListEntryPrefab;


    private void Start()
    {

    }

    public void UpdatePlayerList(Lobby lobby)
    {
        foreach (Transform child in _playerListContainer)
        {
            if (child == _playerListEntryPrefab.transform) continue;

            Destroy(child.gameObject);
        }

    }

    public void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in lobby " + lobby.Name + " " + lobby.Data["GameMode"].Value);

    }
}

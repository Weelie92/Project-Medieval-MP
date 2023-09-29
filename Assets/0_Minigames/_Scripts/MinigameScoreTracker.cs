using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class MinigameScoreTracker : NetworkBehaviour
{
    public static MinigameScoreTracker Instance;

    [SerializeField] private UnityEvent onPlayerAddEvent = new UnityEvent();


    [SerializeField] private GameObject _playerScorePrefab;
    [SerializeField] private Transform _playerScoreParent;

    [SerializeField] private NetworkList<ulong> _playerIds;

    [SerializeField] private Dictionary<ulong, int> _playerScores = new Dictionary<ulong, int>();
    private Dictionary<ulong, string> _playerNames = new Dictionary<ulong, string>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

         _playerIds = new NetworkList<ulong>();

    }



    [ServerRpc(RequireOwnership = false)]
    public void AddPlayerServerRpc(ulong clientId)
    {


        if (_playerScores.ContainsKey(clientId)) return;


        _playerScores.Add(clientId, 0);
        _playerIds.Add(clientId);

        foreach (ulong id in _playerIds)
        {
            if (_playerScores.TryGetValue(id, out int score))
            {
                PlayerChangeClientRpc(id, true);
            }
        }

        onPlayerAddEvent.Invoke();

        UpdateScoreUIClientRpc();

    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerNameServerRpc(string playerName, ulong clientId)
    {
        Debug.Log("Player Name: " + playerName + " Client ID: " + clientId);

        SetPlayerNameClientRpc(playerName, clientId);
    }

    [ClientRpc]
    public void SetPlayerNameClientRpc(string playerName, ulong clientId)
    {
        if (!_playerNames.ContainsKey(clientId))
        {
            _playerNames.Add(clientId, playerName);
        }
        else
        {
            _playerNames[clientId] = playerName;
        }
    }



    [ServerRpc(RequireOwnership = false)]
    public void RemovePlayerServerRpc(ulong clientId)
    {
        if (!_playerScores.ContainsKey(clientId)) return;

        _playerScores.Remove(clientId);
        _playerIds.Remove(clientId);

        PlayerChangeClientRpc(clientId, false);
    }

    [ClientRpc]
    private void PlayerChangeClientRpc(ulong clientId, bool added)
    {
        if (_playerScores.ContainsKey(clientId)) return;

        if (added)
        {
            _playerScores.Add(clientId, 0);
        }
        else
        {
            _playerScores.Remove(clientId);
        }

        
    }

    public void IncrementScore(ulong clientId)
    {
        if (_playerScores.ContainsKey(clientId))
        {
            ChangeScoreServerRpc(clientId, 1);
        }
    }

    public void DecrementScore(ulong clientId)
    {
        if (_playerScores.ContainsKey(clientId))
        {
            ChangeScoreServerRpc(clientId, -1);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeScoreServerRpc(ulong clientId, int value)
    {
        ChangeScoreClientRpc(clientId, value);
    }

    [ClientRpc]
    public void ChangeScoreClientRpc(ulong clientId, int value)
    {
        _playerScores[clientId] += value;

        UpdateScoreUI();
    }

    public int GetScore(ulong clientId)
    {
        if (_playerScores.ContainsKey(clientId))
        {
            return _playerScores[clientId];
        }
        else
        {
            return 0;
        }
    }

    public Dictionary<ulong, int> GetPlayerScores()
    {
        return _playerScores;
    }

    [ClientRpc]
    void UpdateScoreUIClientRpc()
    {
        UpdateScoreUI();
    }

    public void UpdateScoreUI()
    {
        if (ScoreBoard.Instance != null) ScoreBoard.Instance.BuildScoreBoard();

        var sortedPlayerScores = _playerScores.OrderByDescending(kvp => kvp.Value);

        ClearScoreboard();

        foreach (var kvp in sortedPlayerScores)
        {
            GameObject playerScore = Instantiate(_playerScorePrefab, _playerScoreParent);

            if (playerScore != null)
            {
                PlayerScoreUI playerScoreUI = playerScore.GetComponent<PlayerScoreUI>();

                if (playerScoreUI != null)
                {
                    string playerName = GetPlayerName(kvp.Key);
                    if (playerName != null)
                    {
                        playerScoreUI.SetPlayerScore(playerName, kvp.Value);
                    }
                    else
                    {
                        Debug.LogError("Player name not found for client ID: " + kvp.Key);
                    }
                }
                else
                {
                    Debug.LogError("PlayerScoreUI component not found on the playerScore object.");
                }
            }
            else
            {
                Debug.LogError("Failed to instantiate playerScore object.");
            }
        }
    }

    public Dictionary<ulong, string> GetPlayerNames()
    {
        return _playerNames;
    }

    public string GetPlayerName(ulong clientId)
    {
        if (_playerNames.TryGetValue(clientId, out string playerName))
        {
            return playerName;
        }
        else
        {
            Debug.LogWarning("Player name not found for client ID: " + clientId);
            return null;
        }
    }


    public void ClearScoreboard()
    {
        _playerScoreParent = GameObject.FindGameObjectWithTag("UI_ScoreBoardBG").transform;

        foreach (Transform child in _playerScoreParent)
        {
            Destroy(child.gameObject);
        }
    }
}
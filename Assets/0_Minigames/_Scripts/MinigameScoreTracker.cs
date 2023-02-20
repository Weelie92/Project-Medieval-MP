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

        for (int i = 0; i < _playerScores.Count; i++)
        {
            KeyValuePair<ulong, int> pair = _playerScores.ElementAt(i);

            ulong tempClientId = pair.Key;

            PlayerChangeClientRpc(tempClientId, true);
        }

        onPlayerAddEvent.Invoke();


        UpdateScoreUIClientRpc();

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
            ChangeScoreClientRpc(clientId, 1);
        }
    }

    public void DecrementScore(ulong clientId)
    {
        if (_playerScores.ContainsKey(clientId))
        {
            ChangeScoreClientRpc(clientId, -1);
        }
    }

    [ClientRpc]
    public void ChangeScoreClientRpc(ulong clientId, int value)
    {
        _playerScores[clientId] += value;

        UpdateScoreUIClientRpc();
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
    public void UpdateScoreUIClientRpc()
    {
        if (ScoreBoard.Instance != null) ScoreBoard.Instance.BuildScoreBoard();

        var sortedPlayerScores = _playerScores.OrderByDescending(kvp => kvp.Value);

        ClearScoreboard();

        foreach (var kvp in sortedPlayerScores)
        {
            GameObject playerScore = Instantiate(_playerScorePrefab, _playerScoreParent);
            playerScore.GetComponent<PlayerScoreUI>().SetPlayerScore(kvp.Key, kvp.Value);
        }
    }

    public void ClearScoreboard()
    {
        foreach (Transform child in _playerScoreParent)
        {
            Destroy(child.gameObject);
        }
    }
}
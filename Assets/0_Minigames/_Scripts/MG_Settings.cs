using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MG_Settings : NetworkBehaviour
{
    [SerializeField] string _minigameName;
    [SerializeField] string _minigameDescription;
    [SerializeField] float _serverStartTimer = 2f;
    [SerializeField] int _timer;

    [Header("Minigame Team Size: Solo = FFA, Two = 2v2v2v2, Four = 4v4, 3 = 1vRest, All = All on team")]
    [SerializeField] private MinigameTeamSize _minigameTeamSize;

    public MG_UI_Timer _timerUI;

    public List<Transform> _spawnPoints;
    private Vector3 _spawnPoint;


    enum MinigameTeamSize { Solo, Two, Four, OneMan, All };

    public  bool ranOnce = false;
    

    private bool _playersSpawned = false;



    // 2
    public IEnumerator SpawnPlayers(List<ulong> clientsCompleted)
    {
        if (_playersSpawned) yield return null;

        // Shuffle the list of spawn points
        _spawnPoints.Shuffle();

        // Spawns the players
        List<Transform> spawnPoints = new List<Transform>(_spawnPoints);

        foreach (var clientId in clientsCompleted)
        {
            NetworkObject player = NetworkManager.ConnectedClients[clientId].PlayerObject;

            if (player == null) continue;

            Transform spawnPoint = spawnPoints[0];
            spawnPoints.RemoveAt(0);

            MovePlayerToSpawnPointClientRpc(spawnPoint.position, clientId);

            yield return new WaitForSeconds(.5f);
        }

        _playersSpawned = true;

        // Sets timer
        SetPlayerTimersClientRpc();

        StartMinigameServerRpc();
    }


    // 3
    [ClientRpc]
    private void MovePlayerToSpawnPointClientRpc(Vector3 spawnPoint, ulong clientId)
    {
        if (NetworkManager.LocalClientId != clientId) return;

        _spawnPoint = spawnPoint;

        NetworkManager.LocalClient.PlayerObject.transform.position = _spawnPoint;
    }

    // 4
    [ClientRpc]
    private void SetPlayerTimersClientRpc()
    {
        if (_timer == 0)
        {
            _timerUI.enabled = false;
        }
        else
        {
            _timerUI.enabled = true;
            _timerUI.SetTimer();
        }
    }

    // 5
    [ServerRpc]
    private void StartMinigameServerRpc()
    {
        GetComponent<IMinigame>().StartSelectedMinigameServerRpc();
    }

    // 7
    [ServerRpc]
    public void StartCountdownServerRpc()
    {
        if (_timer > 0)
        {
            _timer--;

            UpdateCountdownTimerClientRpc(_timer);

            if (_timer == 0)
            {
                CancelInvoke(nameof(StartCountdownServerRpc));
            }
        }
    }

    // 8
    [ClientRpc]
    private void UpdateCountdownTimerClientRpc(int timer)
    {
        _timer = timer;

        _timerUI.timerText.text = _timer.ToString();
    }

    [ServerRpc(RequireOwnership = false)]
    public void KillPlayerServerRpc(ulong clientId)
    {
        if (!IsServer) return;

        if (NetworkManager.ConnectedClients.TryGetValue(clientId, out NetworkClient client))
        {
            PlayerController playerController = client.PlayerObject.GetComponent<PlayerController>();
            playerController.KillClientRpc(clientId);

            int playersAlive = 0;
            ulong winnerPlayer = 99999;

            foreach (NetworkClient player in NetworkManager.ConnectedClientsList)
            {
                if (player.PlayerObject.GetComponent<PlayerController>().isAlive)
                {
                    playersAlive++;

                    winnerPlayer = player.PlayerObject.GetComponent<NetworkObject>().OwnerClientId;
                }
            }

            if (playersAlive == 1)
            {
                // Player won
                MinigameScoreTracker.Instance.IncrementScore(winnerPlayer);

                Debug.Log("Game Over, player " + winnerPlayer + " won!");
            }
        }
    }

}

// Used to shuffle the list of spawn points
public static class ListExtensions
{
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
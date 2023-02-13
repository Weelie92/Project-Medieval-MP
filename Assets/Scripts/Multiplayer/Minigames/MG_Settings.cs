using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MG_Settings : NetworkBehaviour
{
    [SerializeField] string _minigameName;
    [SerializeField] string _minigameDescription;
    [SerializeField] float _serverStartTimer = 2f;
    [SerializeField] int _timer;

    [Header("Minigame Team Size: Solo = FFA, Two = 2v2v2v2, Four = 4v4, 3 = 1vRest, All = All on team")]
    [SerializeField] private MinigameTeamSize _minigameTeamSize;

    [SerializeField] private NetworkList<PlayerData> _players;
    [SerializeField] private MG_UI_Timer _timerUI;

    [SerializeField] private List<Transform> _spawnPoints;
    [SerializeField] private Vector3 _spawnPoint;


    enum MinigameTeamSize { Solo, Two, Four, OneMan, All };

    private void Awake()
    {
        _players = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ConnectedPlayers>()._playerList;
        _timerUI = GameObject.FindGameObjectWithTag("UI_Minigame").GetComponentInChildren<MG_UI_Timer>();
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
    }

    private bool ranOnce = false;
    // 1
    private void OnLoadEventCompleted(string sceneName, LoadSceneMode sceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!IsServer) return;

        if (ranOnce) return;
        ranOnce = true;

        Debug.Log("1");

        StartCoroutine(SpawnPlayers(clientsCompleted));
    }

    private bool _playersSpawned = false;

    // 2
    private IEnumerator SpawnPlayers(List<ulong> clientsCompleted)
    {
        Debug.Log("2");

        if (_playersSpawned) yield return null;


        // Spawns the players

        var spawnPoints = new List<Transform>(_spawnPoints);

        foreach (var clientId in clientsCompleted)
        {
            var player = NetworkManager.ConnectedClients[clientId].PlayerObject;

            if (player == null) continue;

            var index = Random.Range(0, spawnPoints.Count);

            var spawnPoint = spawnPoints[index];


            MovePlayerToSpawnPointClientRpc(spawnPoint.position, clientId);


            spawnPoints.RemoveAt(index);

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

        Debug.Log("3");

        _spawnPoint = spawnPoint;

        NetworkManager.LocalClient.PlayerObject.transform.position = _spawnPoint;
    }

    // 4
    [ClientRpc]
    private void SetPlayerTimersClientRpc()
    {
        Debug.Log("4");
        _timerUI.SetTimer();
    }

    // 5
    [ServerRpc]
    private void StartMinigameServerRpc()
    {
        Debug.Log("5");
        // Start the minigame

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
}

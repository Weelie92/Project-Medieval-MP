using System.Collections;
using System.Collections.Generic;
using Mono.CSharp;
using QFSW.QC;
using Unity.Netcode;
using UnityEngine;

public class MG_Settings : NetworkBehaviour
{
    [SerializeField] string _minigameName;
    [SerializeField] string _minigameDescription;
    [SerializeField] float _serverStartTimer = 2f;
    [SerializeField] int _timer;

    UIMessageSystem UIMessageSystem;
    LoadingHUD LoadingHUDInstance;

    [Header("Minigame Team Size: Solo = FFA, Two = 2v2v2v2, Four = 4v4, 3 = 1vRest, All = All on team")]
    [SerializeField] private MinigameTeamSize _minigameTeamSize;

    public MG_UI_Timer _timerUI;

    public List<Transform> _spawnPoints;
    private Vector3 _spawnPoint;


    private void Awake()
    {
        UIMessageSystem = UIMessageSystem.Instance;
        LoadingHUDInstance = LoadingHUD.Instance;


    }

    private void Start()
    {
        LoadingHUD.Instance.ToggleFade(true, true);

    }

    enum MinigameTeamSize { Solo, Two, Four, OneMan, All };

    public  bool ranOnce = false;
    

    private bool _playersSpawned = false;



    public IEnumerator SpawnPlayers(List<ulong> clientsCompleted)
    {
        List<Transform> spawnPoints = new List<Transform>(_spawnPoints);

        foreach (var clientId in clientsCompleted)
        {
            NetworkObject player = NetworkManager.ConnectedClients[clientId].PlayerObject;

            if (player == null) continue;

            player.GetComponent<PlayerController>().enabled = false;

            if (spawnPoints.Count == 0)
            {
                Debug.LogError("Not enough spawn points!");
                yield break;
            }

            int spawnPointIndex = Random.Range(0, spawnPoints.Count);
            Transform spawnPoint = spawnPoints[spawnPointIndex];
            spawnPoints.RemoveAt(spawnPointIndex);


            MovePlayerToSpawnPointClientRpc(spawnPoint.position + new Vector3(0, 1f, 0), clientId);

            yield return new WaitForSeconds(1f);

            player.GetComponent<PlayerController>().enabled = true;

        }


        StartMinigameServerRpc();
    }


    [ClientRpc]
    private void MovePlayerToSpawnPointClientRpc(Vector3 spawnPointPos, ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
            StartCoroutine(MovePlayerObject(spawnPointPos));
    }



    private IEnumerator MovePlayerObject(Vector3 spawnPoint)
    {
        NetworkObject localPlayerObject = NetworkManager.Singleton.LocalClient.PlayerObject;

        SetPlayerMovement(false);
        
        localPlayerObject.transform.position = spawnPoint;

        UpdatePlayerPositionServerRpc(NetworkManager.Singleton.LocalClientId, spawnPoint);

        yield return new WaitForSeconds(1f);
        SetPlayerMovement(true);
    }

    [ServerRpc]
    private void UpdatePlayerPositionServerRpc(ulong clientId, Vector3 newPosition)
    {
        NetworkObject player = NetworkManager.ConnectedClients[clientId].PlayerObject;
        if (player != null)
        {
            player.transform.position = newPosition;
        }
    }


    private void SetPlayerMovement(bool enabled)
    {
        var playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;

        var playerController = playerObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = enabled;
        }

        var collider = playerObject.GetComponent<CapsuleCollider>();
        if (collider != null)
        {
            collider.enabled = enabled;
        }

        var rigidbody = playerObject.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = !enabled;
        }
    }







    // 5
    [ServerRpc]
    private void StartMinigameServerRpc()
    {
        GetComponent<IMinigame>().StartSelectedMinigameServerRpc();


        ToggleLoadingHUDClientRpc();
    }

    [ClientRpc]
    private void ToggleLoadingHUDClientRpc()
    {
        StartCoroutine(ToggleLoadingHUD());
    }


    IEnumerator ToggleLoadingHUD()
    {
        yield return new WaitForSeconds(1f);
        LoadingHUD.Instance.ToggleFade(false);
    }



    // 7
    [ServerRpc]
    public void StartCountdownServerRpc(int countdownTime)
    {
        StartCountdownClientRpc(countdownTime);
    }

    [ClientRpc]
    private void StartCountdownClientRpc(int countdownTime)
    {
        StartCoroutine(StartCountdown(countdownTime));
    }

    private IEnumerator StartCountdown(int countdownTime)
    {
        int timer = countdownTime;

        for (int i = 0; i < timer; i++)
        {
            _timerUI.timerText.text = timer.ToString();

            yield return new WaitForSeconds(1f);

            if (timer == 0)
            {

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

            if (playersAlive <= 1)
            {
               

                if (playersAlive == 0)
                {
                    // Draw
                    UIMessageSystem.Instance.DisplayMessageServerRpc("Game Over, draw!", 5);
                    PickWinner(winnerPlayer);


                }
                else
                {
                    // Player won
                    UIMessageSystem.Instance.DisplayMessageServerRpc("Game Over, player " + winnerPlayer + " won!", 5);
                    PickWinner(winnerPlayer);

                }
            }
        }
    }

    public void PickWinner(ulong winnerPlayer)
    {
        if (!IsServer) return;

        MinigameScoreTracker.Instance.IncrementScore(winnerPlayer);

        foreach (NetworkClient player in NetworkManager.ConnectedClientsList)
        {
            ulong playerId = player.PlayerObject.GetComponent<NetworkObject>().OwnerClientId;

            if (playerId != winnerPlayer)
            {
                PlayLossSoundClientRpc(playerId);
            }
            else
            {
                PlayVictorySoundClientRpc(playerId);
            }
        }

        StartCoroutine(ChangeToLobby());
    }

    private IEnumerator ChangeToLobby()
    {
        yield return new WaitForSeconds(2f);

        NetworkSceneManager.Instance.ChangeSceneOnClickLobby();
    }

    [ClientRpc]
    private void PlayVictorySoundClientRpc(ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            PlayerController localPlayerController = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();
            localPlayerController.VictorySound();
        }
    }

    [ClientRpc]
    private void PlayLossSoundClientRpc(ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            PlayerController localPlayerController = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<PlayerController>();
            localPlayerController.LossSound();
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
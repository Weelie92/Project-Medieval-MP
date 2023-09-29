using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MG_Race : MG_Settings, IMinigame, ICollideable
{
    public static MG_Race Instance;

    public UIMessageSystem SendMessage;
    public LoadingHUD LoadingHUD;

    [SerializeField] private GameObject _victoryLine;


    public bool isGameStarted = false;
    public bool isPlayerOnBottomGrid = false;


    private void Awake()
    {
        Instance = this;

        SendMessage = UIMessageSystem.Instance;
        LoadingHUD = LoadingHUD.Instance;

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
    }


    // 1
    private void OnLoadEventCompleted(string sceneName, LoadSceneMode sceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        if (!IsServer) return;

        if (ranOnce) return;
        ranOnce = true;


        StartCoroutine(SpawnPlayers(clientsCompleted));
    }


    // 6
    [ServerRpc]
    public void StartSelectedMinigameServerRpc()
    {

        isGameStarted = true;

        //StartCountdownServerRpc(5);
    }




    private HashSet<int> chosenPlatformsTop = new HashSet<int>();
    private HashSet<int> chosenPlatformsBottom = new HashSet<int>();


    private bool playerWon = false;

    public void OnTriggerEnter(Collider other)
    {
        if (IsServer && !playerWon && other.CompareTag("Player"))
        {
            PickWinner(other.GetComponent<NetworkObject>().OwnerClientId);

            playerWon = true;

            SendMessage.DisplayMessageServerRpc("Game Over! " + MinigameScoreTracker.Instance.GetPlayerName(other.GetComponent<NetworkObject>().OwnerClientId) + " won!", 5);
        }
    }
}
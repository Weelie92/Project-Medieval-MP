using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine.Networking;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;


public class SceneNames
{
    public const string Prototype = "Prototype";
    public const string Minigame1 = "MG_PlatformJump";
    public const string Minigame2 = "MG_Race";
    public const string Lobby = "GameScene";
   
}

public class NetworkSceneManager : NetworkBehaviour
{
    public static NetworkSceneManager Instance;
    public LoadingHUD LoadingHUDInstance;

    private string _activeMinigameScene = null;

    [SerializeField] private AudioSource _audioSource;
   

    [SerializeField] private AudioClip _gameSceneMusic;
    [SerializeField] private AudioClip _raceSceneMusic;
    [SerializeField] private AudioClip _platformSceneMusic;

    [SerializeField] private Button _muteButton;

    [SerializeField] private Button _startPrototype;
    [SerializeField] private Button _mg_PlatformJump;
    [SerializeField] private Button _mg_Race;
    [SerializeField] private Button _startLobby;

    [SerializeField] private string _lobbySceneName;
    [SerializeField] private List<string> _minigameSceneNames;
    [SerializeField] private string _currentSceneName;
    [SerializeField] private string _nextSceneName;
    [SerializeField] private string _previousSceneName;



    private void Start()
    {
        _muteButton.onClick.AddListener(ToggleMuteMusic);


        if (_mg_PlatformJump == null) return;

        Instance = this;
        LoadingHUDInstance = LoadingHUD.Instance;

        _currentSceneName = SceneManager.GetActiveScene().name;

        _mg_PlatformJump.onClick.AddListener(ChangeSceneOnClickPlatform);
        _mg_Race.onClick.AddListener(ChangeSceneOnClickRace);

        _startLobby.onClick.AddListener(ChangeSceneOnClickLobby);

        UpdateButtonAvailability();

    }
    public void ToggleMuteMusic()
    {
        _audioSource.volume = _audioSource.volume > 0 ? 0 : 1;
    }

    [ClientRpc]
    private void ChangeMusicClientRpc(string sceneName)
    {
        AudioClip musicClip = null;

        if (sceneName == "Game")
        {
            musicClip = _gameSceneMusic;
        }
        else if (sceneName == "Race")
        {
            musicClip = _raceSceneMusic;
        }
        else if (sceneName == "Platform")
        {
            musicClip = _platformSceneMusic;
        }

        if (musicClip != null)
        {
            _audioSource.clip = musicClip;
            _audioSource.Play();
        }
    }



    private void UpdateButtonAvailability()
    {
        if (_activeMinigameScene == null)
        {
            // In the Lobby, only the minigame buttons should be available.
            _startLobby.interactable = false;
            _mg_PlatformJump.interactable = true;
            _mg_Race.interactable = true;
        }
        else
        {
            // In a Minigame, only the Lobby button should be available.
            _startLobby.interactable = true;
            _mg_PlatformJump.interactable = false;
            _mg_Race.interactable = false;
        }
    }

    private void ChangeSceneOnClickPlatform()
    {
        if (!IsServer || !IsHost) return;

        _activeMinigameScene = Loader.Scene.MG_PlatformJump.ToString();

        PauseMenuUI.Instance.TogglePauseMenu(false);

        NetworkManager.Singleton.SceneManager.LoadScene(Loader.Scene.MG_PlatformJump.ToString(), LoadSceneMode.Additive);

        UpdateButtonAvailability();

        ChangeMusicClientRpc("Platform");
    }

    private void ChangeSceneOnClickRace()
    {
        Debug.Log(IsServer + " " + IsHost);

        if (!IsServer || !IsHost) return;

        _activeMinigameScene = Loader.Scene.MG_Race.ToString();

        PauseMenuUI.Instance.TogglePauseMenu(false);

        NetworkManager.Singleton.SceneManager.LoadScene(Loader.Scene.MG_Race.ToString(), LoadSceneMode.Additive);

        UpdateButtonAvailability();

        ChangeMusicClientRpc("Race");

    }

    public void ChangeSceneOnClickLobby()
    {
        if (!IsServer || !IsHost) return;

        ToggleChangingSceneServerRpc();

        string sceneName1 = "MG_PlatformJump";
        string sceneName2 = "MG_Race";
        UnityEngine.SceneManagement.Scene sceneToUnload1 = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName1);
        UnityEngine.SceneManagement.Scene sceneToUnload2 = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName2);

        if (sceneToUnload1.IsValid())
        {
            NetworkManager.Singleton.SceneManager.UnloadScene(sceneToUnload1);
            _activeMinigameScene = null;

        }

        if (sceneToUnload2.IsValid())
        {
            NetworkManager.Singleton.SceneManager.UnloadScene(sceneToUnload2);
            _activeMinigameScene = null;

        }



        StartCoroutine(MurderAndRevivePlayers());

        PauseMenuUI.Instance.TogglePauseMenu(false);


        UpdateButtonAvailability();

        ChangeMusicClientRpc("Game");

    }

    [ServerRpc]
    private void ToggleChangingSceneServerRpc()
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClients.Values)
        {
            PlayerController playerController = client.PlayerObject.GetComponent<PlayerController>();

            playerController.ReviveServerRpc(playerController.gameObject.GetComponent<NetworkObject>().OwnerClientId);
        }

    }

    private IEnumerator MurderAndRevivePlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerController>().KillClientRpc(player.GetComponent<NetworkObject>().OwnerClientId);
            //player.GetComponent<Rigidbody>().AddForce(new Vector3(UnityEngine.Random.Range(-1,1), 1, UnityEngine.Random.Range(-1, 1)) * 10, ForceMode.VelocityChange);
        }

        yield return new WaitForSeconds(3);

        foreach (GameObject player in players)
        {
            player.GetComponent<PlayerController>().ReviveClientRpc(player.GetComponent<NetworkObject>().OwnerClientId);
        }
    }
}
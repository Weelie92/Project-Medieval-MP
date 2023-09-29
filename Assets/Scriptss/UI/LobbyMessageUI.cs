using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;


    private void Awake() {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start() {
        PmGameMultiplayer.Instance.OnFailedToJoinGame += PmGameMultiplayer_OnFailedToJoinGame;
        PmGameLobby.Instance.OnCreateLobbyStarted += PmGameLobby_OnCreateLobbyStarted;
        PmGameLobby.Instance.OnCreateLobbyFailed += PmGameLobby_OnCreateLobbyFailed;
        PmGameLobby.Instance.OnJoinStarted += PmGameLobby_OnJoinStarted;
        PmGameLobby.Instance.OnJoinFailed += PmGameLobby_OnJoinFailed;
        PmGameLobby.Instance.OnQuickJoinFailed += PmGameLobby_OnQuickJoinFailed;

        Hide();
    }

    private void PmGameLobby_OnQuickJoinFailed(object sender, System.EventArgs e) {
        ShowMessage("Could not find a Lobby to Quick Join!");
    }

    private void PmGameLobby_OnJoinFailed(object sender, System.EventArgs e) {
        ShowMessage("Failed to join Lobby!");
    }

    private void PmGameLobby_OnJoinStarted(object sender, System.EventArgs e) {
        ShowMessage("Joining Lobby...");
    }

    private void PmGameLobby_OnCreateLobbyFailed(object sender, System.EventArgs e) {
        ShowMessage("Failed to create Lobby!");
    }

    private void PmGameLobby_OnCreateLobbyStarted(object sender, System.EventArgs e) {
        ShowMessage("Creating Lobby...");
    }

    private void PmGameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e) {
        
    }

    private void ShowMessage(string message) {
        Show();
        messageText.text = message;
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        PmGameMultiplayer.Instance.OnFailedToJoinGame -= PmGameMultiplayer_OnFailedToJoinGame;
        PmGameLobby.Instance.OnCreateLobbyStarted -= PmGameLobby_OnCreateLobbyStarted;
        PmGameLobby.Instance.OnCreateLobbyFailed -= PmGameLobby_OnCreateLobbyFailed;
        PmGameLobby.Instance.OnJoinStarted -= PmGameLobby_OnJoinStarted;
        PmGameLobby.Instance.OnJoinFailed -= PmGameLobby_OnJoinFailed;
        PmGameLobby.Instance.OnQuickJoinFailed -= PmGameLobby_OnQuickJoinFailed;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour {



    private void Start() {
        PmGameMultiplayer.Instance.OnTryingToJoinGame += PmGameMultiplayer_OnTryingToJoinGame;
        PmGameMultiplayer.Instance.OnFailedToJoinGame += PmGameManager_OnFailedToJoinGame;

        Hide();
    }

    private void PmGameManager_OnFailedToJoinGame(object sender, System.EventArgs e) {
        Hide();
    }

    private void PmGameMultiplayer_OnTryingToJoinGame(object sender, System.EventArgs e) {
        Show();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void OnDestroy() {
        PmGameMultiplayer.Instance.OnTryingToJoinGame -= PmGameMultiplayer_OnTryingToJoinGame;
        PmGameMultiplayer.Instance.OnFailedToJoinGame -= PmGameManager_OnFailedToJoinGame;
    }

}
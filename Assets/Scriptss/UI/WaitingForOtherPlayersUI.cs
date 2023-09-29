using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForOtherPlayersUI : MonoBehaviour {


    private void Start() {
        PmGameManager.Instance.OnLocalPlayerReadyChanged += PmGameManager_OnLocalPlayerReadyChanged;
        PmGameManager.Instance.OnStateChanged += PmGameManager_OnStateChanged;

        Hide();
    }

    private void PmGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (PmGameManager.Instance.IsCountdownToStartActive()) {
            Hide();
        }
    }

    private void PmGameManager_OnLocalPlayerReadyChanged(object sender, System.EventArgs e) {
        //if (PmGameManager.Instance.IsLocalPlayerReady()) {
        //    Show();
        //}
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

}
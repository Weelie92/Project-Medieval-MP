using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MG_PlatformJump : MG_Settings, IMinigame
{
    // 6
    [ServerRpc]
    public void StartSelectedMinigameServerRpc()
    {
        foreach (PlayerData playerData in GameObject.FindGameObjectWithTag("GameManager").GetComponent<ConnectedPlayers>()._playerList)
        {
            PlayerController player = NetworkManager.ConnectedClients[playerData.ClientId].PlayerObject.GetComponent<PlayerController>();

            player.isChangingScene = false;
        }

        InvokeRepeating(nameof(StartCountdownServerRpc), 1f, 1f);
    }

    
}

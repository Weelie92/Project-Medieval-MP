using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneUI : MonoBehaviour
{


    [SerializeField] private Button createGameButton;
    [SerializeField] private Button joinGameButton;


    private void Awake()
    {
        createGameButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            Loader.LoadNetwork(Loader.Scene.GameScene);
        });
        joinGameButton.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
        });
    }

}
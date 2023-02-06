using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    

    [SerializeField] private Button _hostBtn;
    [SerializeField] private Button _clientBtn;


    private void Awake()
    {
        _hostBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();

            GameObject.FindGameObjectWithTag("UI_Network").SetActive(false);

        });

        _clientBtn.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();

            GameObject.FindGameObjectWithTag("UI_Network").SetActive(false);
        });
    }
}

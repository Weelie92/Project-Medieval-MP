using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Canvas _mainMenu;
    [SerializeField] Image _lobbyServerInput;
    [SerializeField] Image _actualMainMenuLobby;


    [SerializeField] Button _mainMenu_StartBtn;
    [SerializeField] Button _mainMenu_CreateLobbyBtn;
    [SerializeField] Button _mainMenu_LobbiesBtn;

    [SerializeField] ScrollRect _mainMenu_ScrollRect;

    private void Start()
    {
        _mainMenu = GetComponent<Canvas>();

        _mainMenu_CreateLobbyBtn.gameObject.SetActive(false);
        _mainMenu_LobbiesBtn.gameObject.SetActive(false);
        _lobbyServerInput.gameObject.SetActive(false);
        _actualMainMenuLobby.gameObject.SetActive(false);

        _mainMenu_ScrollRect.gameObject.SetActive(false);


        _mainMenu_StartBtn.onClick.AddListener(() =>
        {
            _mainMenu_StartBtn.gameObject.SetActive(false);
            _mainMenu_CreateLobbyBtn.gameObject.SetActive(true);
            _mainMenu_LobbiesBtn.gameObject.SetActive(true);
        });

        _mainMenu_CreateLobbyBtn.onClick.AddListener(() =>
        {
            //SceneManager.LoadScene("Lobby");
        });


        _mainMenu_LobbiesBtn.onClick.AddListener(() =>
        {   
            _mainMenu_CreateLobbyBtn.gameObject.SetActive(false);
            _mainMenu_LobbiesBtn.gameObject.SetActive(false);
            _mainMenu_ScrollRect.gameObject.SetActive(true);

        });

        _mainMenu.gameObject.SetActive(true);
    }
}

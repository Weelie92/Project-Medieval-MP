using System.Collections;
using PsychoticLab;
using Unity.Netcode;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance;

    [SerializeField] private GameObject _player;
    [SerializeField] private Canvas _uiCanvas;

    private void Awake()
    {
        Instance = this;
    }

    public void InitializePlayerUI(NetworkObject playerNetworkObject)
    {

        if (playerNetworkObject.IsOwner)
        {
            _player = playerNetworkObject.gameObject;



            Initialize();
        }
        else
        {
            Debug.Log("Player UI not initialized for remote player.");
        }
    }

    public void Initialize()
    {
        _uiCanvas = GetComponent<Canvas>();

        _uiCanvas.enabled = false;
    }

    public void Resume()
    {
        TogglePauseMenu(false);
    }

    public void BugReport()
    {

    }

    public void Randomize()
    {
        
        _player.GetComponent<CharacterRandomizer>().Randomize();
        
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void TogglePauseMenu(bool value = true)
    {
        if (_uiCanvas == null)
        {
            Instance = this;
            _uiCanvas = GetComponent<Canvas>();

        }

        _uiCanvas.enabled = value;

        _player.GetComponent<PlayerController>().isPaused = value;

        Cursor.lockState = !value ? CursorLockMode.Locked : CursorLockMode.None;
    }

    private void OnDestroy()
    {
        
    }
}

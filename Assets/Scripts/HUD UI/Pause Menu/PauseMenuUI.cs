using System.Collections;
using UnityEngine;

public class PauseMenuUI : MonoBehaviour
{
    private GameObject _player;
    private Canvas _uiCanvas;


    private void Awake()
    {
        DontDestroyOnLoad(transform.parent.gameObject);
    }

    public void Initialize()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _uiCanvas = GetComponent<Canvas>();

        _player.GetComponent<PlayerController>().pauseMenuUI = this;
        _uiCanvas.enabled = false;
    }

    public void Resume()
    {
        TogglePauseMenu(false);
    }

    public void BugReport()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }

    public void TogglePauseMenu(bool value = true)
    {
        _uiCanvas.enabled = value;

        _player.GetComponent<PlayerController>().isPaused = value;

        Cursor.lockState = !value ? CursorLockMode.Locked : CursorLockMode.None;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    private GameObject _player;
    
    public void Initialize()
    {
        _player = GameObject.FindGameObjectWithTag("Player");

        _player.GetComponent<PlayerController>().pauseMenuUI = gameObject.GetComponent<PauseMenuUI>();

        gameObject.SetActive(false);
    }

    public void Resume()
    {
        TogglePauseMenu();
        gameObject.SetActive(false);
    }

    public void BugReport()
    {
        
    }

    public void Quit()
    {
        
        Application.Quit();
    }

    public void TogglePauseMenu()
    {
        if (gameObject.activeSelf)
        {
            // Game unpaused


            _player.GetComponent<PlayerAiming>().enabled = true;
            _player.GetComponent<PlayerController>().isPaused = false;

            Cursor.lockState = CursorLockMode.Locked;

            gameObject.SetActive(false);
        }
        else
        {
            // Game paused


            _player.GetComponent<PlayerAiming>().enabled = false;
            _player.GetComponent<PlayerController>().isPaused = true;

            Cursor.lockState = CursorLockMode.None;

            gameObject.SetActive(true);
        }
    }
}

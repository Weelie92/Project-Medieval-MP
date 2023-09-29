using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject hudUIPrefab;
    [SerializeField] private GameObject pmGameManagerPrefab;

    void Awake()
    {
        LoadingHUD.Instance.ToggleFade(true, true);

        SpawnUIIfNotExists();
        SpawnPmGameManagerIfNotExists();
    }

    private void SpawnUIIfNotExists()
    {
        GameObject mainUI = GameObject.FindGameObjectWithTag("MainUI");
        if (mainUI == null)
        {
            Instantiate(hudUIPrefab);
        }
    }

    private void SpawnPmGameManagerIfNotExists()
    {
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        if (gameManager == null)
        {
            Instantiate(pmGameManagerPrefab);
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MG_PlatformJump : MG_Settings, IMinigame, ICollideable
{
    public static MG_PlatformJump Instance;

    UIMessageSystem UIMessageSystem;

    [SerializeField] private GameObject _platformPrefab;
    [SerializeField] private List<GameObject> _platformPrefabs;
    [SerializeField] private GameObject _platformParent;

    public List<Transform> _platformsTop = new List<Transform>();
    public List<Transform> _platformsBottom = new List<Transform>();

    [SerializeField] private BoxCollider _bottomGridTrigger;

    public bool isGameStarted = false;
    public bool isPlayerOnBottomGrid = false;

    [SerializeField] private bool _setPlayerKnockbackBool;

    [SerializeField] private int _gridSize = 7;
    [SerializeField] private float _gridSpacing = 2.0f;

    private void Awake()
    {

        Instance = this;



        UIMessageSystem = UIMessageSystem.Instance;

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
    }

    public bool SetPlayerKnockbackBool
    {
        get => _setPlayerKnockbackBool;
        set => _setPlayerKnockbackBool = value;
    }

    // 1
    private void OnLoadEventCompleted(string sceneName, LoadSceneMode sceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {

        if (!IsServer) return;

        if (ranOnce) return;
        ranOnce = true;


        Vector3 centerTop = new Vector3(0, 25, 2);
        Vector3 centerBottom = new Vector3(0, 20, 2);
        int halfSize = Mathf.FloorToInt(_gridSize / 2);
        Vector3 startPositionTop = centerTop - new Vector3(halfSize * _gridSpacing, 0, halfSize * _gridSpacing);
        Vector3 startPositionBottom = centerBottom - new Vector3(halfSize * _gridSpacing, 0, halfSize * _gridSpacing);

        for (int x = 0; x < _gridSize; x++)
        {
            for (int z = 0; z < _gridSize; z++)
            {
                Vector3 positionTop = startPositionTop + new Vector3(x * _gridSpacing, 0, z * _gridSpacing);
                Vector3 positionBottom = startPositionBottom + new Vector3(x * _gridSpacing, 0, z * _gridSpacing);

                float rand = Random.Range(0f, 1f);
                int index;

                if (rand < 0.9f)
                {
                    index = 0;
                }
                else
                {
                    index = Random.Range(1, _platformPrefabs.Count);
                }

                GameObject platformObjectTop = Instantiate(_platformPrefabs[index], positionTop, Quaternion.identity, _platformParent.transform);
                GameObject platformObjectBottom = Instantiate(_platformPrefabs[index], positionBottom, Quaternion.identity, _platformParent.transform);

                platformObjectBottom.GetComponent<PlatformMain>().isBottomGrid = true;

                _spawnPoints.Add(platformObjectTop.transform);

                _platformsTop.Add(platformObjectTop.transform);

                _platformsBottom.Add(platformObjectBottom.transform);

                platformObjectTop.GetComponent<NetworkObject>().Spawn();
                platformObjectBottom.GetComponent<NetworkObject>().Spawn();
            }
        }

        Debug.Log(clientsCompleted.Count);

        StartCoroutine(SpawnPlayers(clientsCompleted));
    }


    // 6
    [ServerRpc]
    public void StartSelectedMinigameServerRpc()
    {
        Killzone.Instance.isActive = true;

        SetPlayerKnockback(_setPlayerKnockbackBool);

        isGameStarted = true;

        InvokeRepeating(nameof(CrumbleRandomPlatformServerRpc), .5f, .5f);
    }

    private HashSet<int> chosenPlatformsTop = new HashSet<int>();
    private HashSet<int> chosenPlatformsBottom = new HashSet<int>();

    [ServerRpc]
    private void CrumbleRandomPlatformServerRpc()
    {

        if (!IsServer) return;

        int indexTop = 999;
        int indexBottom = 999;

        
            if ((chosenPlatformsTop.Count + chosenPlatformsBottom.Count) == (_platformsTop.Count + _platformsBottom.Count)) return;

            if (!isPlayerOnBottomGrid)
            {
            indexTop = Random.Range(0, _platformsTop.Count);

                chosenPlatformsTop.Add(indexTop);

                if (chosenPlatformsTop.Count == _platformsTop.Count)
                {
                    isPlayerOnBottomGrid = true;
                }

            }
            else
            {

                if (chosenPlatformsTop.Count != _platformsTop.Count)
                {
                indexTop = Random.Range(0, _platformsTop.Count);

                    while (chosenPlatformsTop.Contains(indexTop))
                    {
                    indexTop = Random.Range(0, _platformsTop.Count);
                    }

                    chosenPlatformsTop.Add(indexTop);
                }

                if (chosenPlatformsBottom.Count != _platformsBottom.Count)
                {
                indexBottom = Random.Range(0, _platformsBottom.Count);

                    while (chosenPlatformsBottom.Contains(indexBottom))
                    {
                    indexBottom = Random.Range(0, _platformsBottom.Count);
                    }

                    chosenPlatformsBottom.Add(indexBottom);
                }

                
            }

        if (isPlayerOnBottomGrid && indexBottom != 999)
        {
            Transform platformBottomTransform = _platformsBottom[indexBottom];

            PlatformMain platformBottomMain = platformBottomTransform.GetComponent<PlatformMain>();

            if (platformBottomMain != null)
            {
                platformBottomMain.StartCrumbleServerRpc();
            }
        }

        if (indexTop != 999)
        {
            Transform platformTopTransform = _platformsTop[indexTop];

            PlatformMain platformTopMain = platformTopTransform.GetComponent<PlatformMain>();

            if (platformTopMain != null)
            {
                platformTopMain.StartCrumbleServerRpc();
            }
        }       
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartBottomGridServerRpc()
    {
        isPlayerOnBottomGrid = true;
    }



    public void SetPlayerKnockback(bool shouldSet)
    {
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClients.Values)
        {
            PlayerController playerController = client.PlayerObject.GetComponent<PlayerController>();

            playerController._isBeingKnockedBack = false;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!Killzone.Instance.isActive) return;

        if (other.CompareTag("Player"))
        {
            StartBottomGridServerRpc();
        }
    }
}
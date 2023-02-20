using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Killzone : NetworkBehaviour, ICollideable
{
    public static Killzone Instance;

    private MG_Settings _settings;

    public bool isActive = false;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _settings = FindObjectOfType<MG_Settings>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (other.gameObject.CompareTag("Player"))
        {
            _settings.KillPlayerServerRpc(other.GetComponent<NetworkObject>().OwnerClientId);
            isActive = false;
        }
    }
}
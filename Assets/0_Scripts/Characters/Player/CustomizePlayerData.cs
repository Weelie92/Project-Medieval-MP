using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Networking;


public class CustomizePlayerData : NetworkBehaviour
{
    public List<GameObject> allChildObjects;

    public NetworkList<int> allNetworkChildObjectsIndex;

    
    private void OnEnable()
    {
        allNetworkChildObjectsIndex = new NetworkList<int>();

        FindChildObjects(transform.Find("Modular_Characters"));

        allNetworkChildObjectsIndex.OnListChanged += OnListChangedEvent;
    }

    private void OnDisable()
    {
        allNetworkChildObjectsIndex.OnListChanged -= OnListChangedEvent;
    }

    private void Start()
    {
        UpdatePlayerModel();
    }

    // This is called when the player is spawned
    // This will update each model to be correct
    public void UpdatePlayerModel()
    {
        foreach(GameObject x in allChildObjects)
        {
            x.SetActive(false);
        }

        foreach (int index in allNetworkChildObjectsIndex)
        {
            if (index == -1) continue;

            allChildObjects[index].SetActive(true);
        }

    }

    [ClientRpc]
    public void UpdatePlayerModelClientRpc()
    {
        if (IsLocalPlayer) return;

        foreach (GameObject x in allChildObjects)
        {
            x.SetActive(false);
        }

        foreach (int index in allNetworkChildObjectsIndex)
        {
            if (index == -1) continue;

            allChildObjects[index].SetActive(true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerModelServerRpc()
    {
        foreach (GameObject x in allChildObjects)
        {
            x.SetActive(false);
        }

        foreach (int index in allNetworkChildObjectsIndex)
        {
            if (index == -1) continue;

            allChildObjects[index].SetActive(true);
        }

        UpdatePlayerModelClientRpc();
    }

    // Find all child objects and add them to the list
    void FindChildObjects(Transform parent)
    {
        foreach (Transform child in parent)
        {
            // If the child has children, call this function again
            // Every customization option starts with "Chr_" so we can filter them out
            
            if (child.name.StartsWith("Chr_") && allChildObjects.Contains(child.gameObject) == false)
            {
                allChildObjects.Add(child.gameObject);
            }
            else
            {
                FindChildObjects(child);
            }
        }
    }

    // When the list changes, update the player model
    private void OnListChangedEvent(NetworkListEvent<int> op)
    {
        UpdatePlayerModelServerRpc();
    }


    // Constructs the new players NetworkList
    [ServerRpc(RequireOwnership = false)]
    public void ConstructNetworkListServerRpc(ulong clientId)
    {
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            NetworkClient client = NetworkManager.ConnectedClients[clientId];

            // Grab the NetworkList from the playerobject
            NetworkList<int> list = client.PlayerObject.GetComponent<CustomizePlayerData>().allNetworkChildObjectsIndex;


            list.Clear();

            
            // 26 times, one for each changable slot
            for (int i = 0; i < 26; i++)
            {
                // -1 is used to indicate that the slot is empty
                list.Add(-1);
            }
        }
    }

    // 2nd method that runs on the server, to update the NetworkList, using the index the client sent

    [ServerRpc(RequireOwnership = false)]
    public void SetNetworkListServerRpc(int[] indexes, ulong clientId)
    {
        // Get the correct playerobject, using the clientId
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            NetworkClient client = NetworkManager.ConnectedClients[clientId];

            // Grab the NetworkList from the playerobject
            NetworkList<int> list = client.PlayerObject.GetComponent<CustomizePlayerData>().allNetworkChildObjectsIndex;

            // Loop through the indexes containing the index of the selected object
            foreach (int index in indexes)
            {
                switch (index)
                {
                    // HeadCoverings 
                    case int i when (i >= 0 && i <= 27):
                        list[0] = index;
                        break;
                    // Hair
                    case int i when (i >= 28 && i <= 65):
                        list[1] = index;
                        break;
                    // HeadAttachments
                    case int i when (i >= 66 && i <= 78):
                        list[2] = index;
                        break;
                    // ChestAttachments (empty)
                    case int i when (i == 9999):
                        list[3] = index;
                        break;
                    // BackAttachments
                    case int i when (i >= 79 && i <= 93):
                        list[4] = index;
                        break;
                    // ShoulderAttachRight
                    case int i when (i >= 94 && i <= 114):
                        list[5] = index;
                        break;
                    // ShoulderAttachLeft
                    case int i when (i >= 115 && i <= 135):
                        list[6] = index;
                        break;
                    // ElbowAttachRight
                    case int i when (i >= 136 && i <= 141):
                        list[7] = index;
                        break;
                    // ElbowAttachLeft
                    case int i when (i >= 142 && i <= 147):
                        list[8] = index;
                        break;
                    // HipsAttachment
                    case int i when (i >= 148 && i <= 159):
                        list[9] = index;
                        break;
                    // KneeAttachRight
                    case int i when (i >= 160 && i <= 170):
                        list[10] = index;
                        break;
                    // KneeAttachLeft
                    case int i when (i >= 171 && i <= 181):
                        list[11] = index;
                        break;
                    // ElfEars
                    case int i when (i >= 182 && i <= 184):
                        list[12] = index;
                        break;
                    // Heads
                    case int i when (i >= 185 && i <= 220 || i >= 442 && i <= 477):
                        list[13] = index;
                        break;
                    // Eyebrows
                    case int i when (i >= 221 && i <= 227 || i >= 478 && i <= 487):
                        list[14] = index;
                        break;
                    // Facial Hair
                    case int i when (i >= 488 && i <= 505):
                        list[15] = index;
                        break;
                    // Torso
                    case int i when (i >= 228 && i <= 256 || i >= 506 && i <= 534):
                        list[16] = index;
                        break;
                    // ArmUpperRight
                    case int i when (i >= 257 && i <= 277 || i >= 535 && i <= 555):
                        list[17] = index;
                        break;
                    // ArmUpperLeft
                    case int i when (i >= 278 && i <= 298 || i >= 556 && i <= 576):
                        list[18] = index;
                        break;
                    // ArmLowerRight
                    case int i when (i >= 299 && i <= 317 || i >= 577 && i <= 595):
                        list[19] = index;
                        break;
                    // ArmLowerLeft
                    case int i when (i >= 318 && i <= 336 || i >= 596 && i <= 614):
                        list[20] = index;
                        break;
                    // HandRight
                    case int i when (i >= 337 && i <= 354 || i >= 615 && i <= 632):
                        list[21] = index;
                        break;
                    // HandLeft
                    case int i when (i >= 355 && i <= 372 || i >= 633 && i <= 650):
                        list[22] = index;
                        break;
                    // Hips
                    case int i when (i >= 373 && i <= 401 || i >= 651 && i <= 679):
                        list[23] = index;
                        break;
                    // LegRight
                    case int i when (i >= 402 && i <= 421 || i >= 680 && i <= 699):
                        list[24] = index;
                        break;
                    // LegLeft
                    case int i when (i >= 422 && i <= 441 || i >= 700 && i <= 719):
                        list[25] = index;
                        break;

                }
            }
        }
    }

    // Called when player finish customization, serverRpcParams grabs the information about the client that called the function
    // ActivePartsIndex is the index which contain every picked index from the client that called the function
    
    [ServerRpc(RequireOwnership = false)]
    public void UpdateCharacterServerRpc(int[] activePartsIndex, ServerRpcParams serverRpcParams = default)
    {
        ConstructNetworkListServerRpc(serverRpcParams.Receive.SenderClientId);
        SetNetworkListServerRpc(activePartsIndex, serverRpcParams.Receive.SenderClientId);
    }
}

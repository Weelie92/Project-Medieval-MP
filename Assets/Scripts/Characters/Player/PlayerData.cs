using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public struct PlayerData : INetworkSerializable, IEquatable<PlayerData>
{
    public ulong ClientId;
    //public string PlayerName;
    //public int[] AllNetworkChildObjectsIndex;

    public PlayerData(ulong clientId)
    {
        ClientId = clientId;
        //PlayerName = "";
        //AllNetworkChildObjectsIndex = new int[arrayLength];
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientId);
        //serializer.SerializeValue(ref PlayerName);
        //serializer.SerializeValue(ref AllNetworkChildObjectsIndex);
    }

    public bool Equals(PlayerData other)
    {
        return ClientId == other.ClientId;
    }
}
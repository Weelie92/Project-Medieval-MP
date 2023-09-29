using System.Reflection;
using Unity.Netcode;
using UnityEngine;

public class PlayerDataNetworkList : NetworkList<PlayerData>
{
    //public PlayerDataNetworkList(NetworkBehaviour networkBehaviour) : base(networkBehaviour) { }





    private void SetNetworkBehaviour(NetworkBehaviour networkBehaviour)
    {
        typeof(NetworkList<PlayerData>).GetField("m_NetworkBehaviour", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(this, networkBehaviour);
    }
}

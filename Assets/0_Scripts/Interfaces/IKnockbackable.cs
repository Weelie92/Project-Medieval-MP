using Unity.Netcode;
using UnityEngine;

public interface IKnockbackable
{
    bool CanBeKnockedBack { get; set; }
    void KnockbackClientRpc(Vector3 direction, float force, ulong targetClientId, ClientRpcParams clientRpcParams = default);
}

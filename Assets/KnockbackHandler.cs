using System.Collections;
using QFSW.QC;
using Unity.Netcode;
using UnityEngine;

public class KnockbackHandler : NetworkBehaviour
{
    [ServerRpc(RequireOwnership = false)]
    public void KnockbackServerRpc()
    {
        if (!IsServer) return;

        KnockbackClientRpc();
    }

    [ClientRpc]
    public void KnockbackClientRpc()
    {
        if (GetComponent<PlayerController>()._isBeingKnockedBack) return;

        Knockback();
    }

    private IEnumerator EnableMovementAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay / 2);

        PlayerController playerController = GetComponent<PlayerController>();
        Animator playerAnimator = playerController._playerAnimator;

        playerAnimator.enabled = true;
        playerAnimator.SetTrigger("GetUp");

        yield return new WaitForSeconds(delay / 2);

        playerController.isAlive = true;
        playerController._isBeingKnockedBack = false;
    }

    [Command]
    public void Knockback()
    {
        PlayerController playerController = GetComponent<PlayerController>();
        Animator playerAnimator = playerController._playerAnimator;
        Rigidbody playerRigidbody = playerController._playerRigidbody;

        playerController._isBeingKnockedBack = true;

        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
            playerController._playerAnimator = playerAnimator;
        }

        playerAnimator.enabled = false;
        playerController.EnableRagdoll();
        playerController.DamageTakenSound();
        playerRigidbody.AddForce(Vector3.back, ForceMode.Impulse);

        StartCoroutine(EnableMovementAfterDelay(2f));
    }
}

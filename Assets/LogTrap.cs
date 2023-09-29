using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class LogTrap : NetworkBehaviour
{
    public GameObject log;
    public GameObject hangingPoint;
    public Collider triggerZone; // The Collider set as a trigger
    public Collider triggerLog; // The Collider set as a trigger
    public float forceStrength = 3f; // Adjust this to change the strength of the force

    private NetworkVariable<bool> hasCollided = new NetworkVariable<bool>();

    private void Start()
    {
        hasCollided.Value = false;

        FixedJoint joint = log.AddComponent<FixedJoint>();
        joint.connectedBody = hangingPoint.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsServer && other.CompareTag("Player") && !hasCollided.Value) 
        {
            LogFallClientRpc();
            hasCollided.Value = true;
        } 
        else if (hasCollided.Value) 
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<KnockbackHandler>().KnockbackServerRpc();
            }
        }
    }

    [ClientRpc]
    private void LogFallClientRpc()
    {
        Destroy(log.GetComponent<FixedJoint>());

        triggerZone.enabled = false;
        triggerLog.enabled = true;

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject player in players)
        {
            float distance = Vector3.Distance(player.transform.position, log.transform.position);

            if (distance < closestDistance)
            {
                closestPlayer = player;
                closestDistance = distance;
            }
        }

        if (closestPlayer != null)
        {
            Vector3 direction = (closestPlayer.transform.position - log.transform.position).normalized;
            log.GetComponent<Rigidbody>().AddForce(direction * forceStrength, ForceMode.VelocityChange);
        }

        StartCoroutine(DestroyLogAfterDelay(5f));
    }

    private IEnumerator DestroyLogAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(log);
    }
}

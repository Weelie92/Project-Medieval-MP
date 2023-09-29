using UnityEngine;
using Unity.Netcode;
using TMPro;
using System.Collections;
using QFSW.QC;

public class UIMessageSystem : NetworkBehaviour
{
    public static UIMessageSystem Instance;

    [SerializeField]
    private CanvasGroup messageCanvasGroup;

    [SerializeField]
    private TextMeshProUGUI messageText;

    private void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {


        NetworkObject networkObject = GetComponent<NetworkObject>();

        if (!networkObject.IsSpawned && NetworkManager.Singleton.IsServer)
        {
            // Pause script for 1 second

            yield return new WaitForSeconds(2f);
            networkObject.Spawn();

        }



    }

    [Command]
    [ServerRpc(RequireOwnership = false)]
    public void DisplayMessageServerRpc(string message, int displayTime)
    {
        DisplayMessageClientRpc(message, displayTime);
    }

    [ClientRpc]
    public void DisplayMessageClientRpc(string message, int displayTime)
    {
        StartCoroutine(DisplayMessageCoroutine(message, displayTime));
    }

    private IEnumerator DisplayMessageCoroutine(string message, int displayTime = 5)
    {
        messageText.text = message;

        float fadeInTime = 0.5f;
        for (float t = 0; t < fadeInTime; t += Time.deltaTime)
        {
            messageCanvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeInTime);
            yield return null;
        }
        messageCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(displayTime);

        float fadeOutTime = 0.5f;
        for (float t = 0; t < fadeOutTime; t += Time.deltaTime)
        {
            messageCanvasGroup.alpha = Mathf.Lerp(1f, 0, t / fadeOutTime);
            yield return null;
        }
        messageCanvasGroup.alpha = 0;
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlatformMain : NetworkBehaviour
{
    public GameObject shadowPrefab;
    public GameObject smokePrefab;
    public GameObject flamesPrefab;
    public float crumbleTime = 2.5f;
    public float shadowScale = .7f;

    public bool isCrumbling = false;
    public bool isBottomGrid = false;

    

    [ServerRpc]
    public void StartCrumbleServerRpc()
    {
        if (isCrumbling) return;

        isCrumbling = true;


        GetComponent<AudioManagerPlatform>().PlayBuildupSound();

        if (isBottomGrid)
        {
            StartFlamesCoroutineClientRpc();
        }
        else
        {
            StartShadowCoroutineClientRpc();
        }
    }

    [ClientRpc]
    private void StartFlamesCoroutineClientRpc()
    {
        StartCoroutine(nameof(StartFlamesCoroutine));
    }

    private IEnumerator StartFlamesCoroutine()
    {
        smokePrefab.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        flamesPrefab.SetActive(true);

        yield return new WaitForSeconds(1.0f);

        StartPlatformCrumbledServerRpc();
    }

    [ClientRpc]
    private void StartShadowCoroutineClientRpc()
    {
        StartCoroutine(nameof(StartShadowCoroutine));
    }

    private IEnumerator StartShadowCoroutine()
    {
        shadowPrefab.SetActive(true);

        float timer = 0.0f;

        while (timer < crumbleTime)
        {
            float t = timer / crumbleTime;
            float scale = Mathf.Lerp(0.1f, shadowScale, t);
            shadowPrefab.transform.localScale = Vector3.one * scale;

            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(-timer);

        StartPlatformCrumbledServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void StartPlatformCrumbledServerRpc()
    {
        PlatformCrumbledClientRpc();
    }

    [ClientRpc]
    private void PlatformCrumbledClientRpc()
    {
        StartCoroutine(nameof(PlatformCrumbled));
    }

    

    private IEnumerator PlatformCrumbled()
    {
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;

        GetComponent<AudioManagerPlatform>().PlayExplosionSound();


        SpriteRenderer spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null) spriteRenderer.enabled = false;

        yield return new WaitForSeconds(5);

        ParticleSystem[] particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem ps in particleSystems)
        {
            ps.Stop();
        }

        yield return new WaitForSeconds(5);

        gameObject.SetActive(false);
    }
}

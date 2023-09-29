using UnityEngine;
using Unity.Netcode;
using System.Collections;
using QFSW.QC;

public class LoadingHUD : MonoBehaviour
{
    public static LoadingHUD Instance;

    [SerializeField] private CanvasGroup loadingCanvasGroup;

    private void Awake()
    {
        Instance = this;
        ToggleFade(false, true);
    }


    
    public void ToggleFade(bool loading, bool instant = false)
    {
        if (instant)
        {
            if (loading)
            {
                loadingCanvasGroup.alpha = 1f;
            }
            else
            {
                loadingCanvasGroup.alpha = 0f;
            }
            return;
        }

        if (loading)
        {
            StartCoroutine(FadeCoroutine(0, 1));
        }
        else
        {
            StartCoroutine(FadeCoroutine(1, 0));
        }
    }

    private IEnumerator FadeCoroutine(float startAlpha, float endAlpha)
    {
        float fadeTime = 0.5f;

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            loadingCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t / fadeTime);
            yield return null;
        }

        loadingCanvasGroup.alpha = endAlpha;
    }
}

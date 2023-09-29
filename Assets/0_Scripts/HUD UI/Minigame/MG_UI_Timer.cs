using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;


public class MG_UI_Timer : MonoBehaviour
{
    public static MG_UI_Timer Instance;

    public TextMeshProUGUI timerText;
    
    [SerializeField] float _growTimer = .5f;
    [SerializeField] float _shrinkTimer = 0.3f;
    [SerializeField] Vector3 _oldPos;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            SetTimer();
        }
    }

    #region Set Timer Animation
    public void SetTimer()
    {
        _oldPos = transform.localPosition;
        //_oldPos.x = transform.parent.position.x;

        // Set the initial position and scale of the timer text
        timerText.rectTransform.localScale = Vector3.zero;
        timerText.rectTransform.localPosition = new Vector3(0, 0, 0);

        

        // Animate the scale of the timer text to 1.2 over the animation duration
        StartCoroutine(GrowTimerScale(1.2f, _growTimer));

    }

    private IEnumerator GrowTimerScale(float targetScale, float duration)
    {
        float timer = 0f;
        Vector3 initialScale = timerText.rectTransform.localScale;
        Vector3 targetScaleVector = new Vector3(targetScale, targetScale, targetScale);

        while (timer < duration)
        {
            timerText.rectTransform.localScale = Vector3.Lerp(initialScale, targetScaleVector, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        timerText.rectTransform.localScale = targetScaleVector;

        yield return new WaitForSeconds(1f);

        // Shrink the timer text back to its original scale over 0.3 seconds
        StartCoroutine(ShrinkTimerScale(1f, _shrinkTimer));
    }

    private IEnumerator ShrinkTimerScale(float targetScale, float duration)
    {
        float timer = 0f;
        Vector3 initialScale = timerText.rectTransform.localScale;
        Vector3 targetScaleVector = new Vector3(targetScale, targetScale, targetScale);

        StartCoroutine(MoveTimerText(0.5f));

        while (timer < duration)
        {
            timerText.rectTransform.localScale = Vector3.Lerp(initialScale, targetScaleVector, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        timerText.rectTransform.localScale = targetScaleVector;

        
    }

    private IEnumerator MoveTimerText(float duration)
    {
        float timer = 0f;


        while (timer < duration)
        {
            timerText.transform.localPosition = Vector2.Lerp(new Vector3(0,0,0), _oldPos, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    #endregion
}

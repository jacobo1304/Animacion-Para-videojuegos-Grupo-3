using UnityEngine;
using System.Collections;

[DisallowMultipleComponent]
public class CanvasFadeMessage : MonoBehaviour
{
    [Header("References")]
    [Tooltip("CanvasGroup to fade in/out. If empty, it will use the one on this GameObject.")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Timing")]
    [SerializeField] private float fadeInDuration = 0.2f;
    [SerializeField] private float visibleDuration = 1f;
    [SerializeField] private float fadeOutDuration = 0.35f;
    [SerializeField] private bool useUnscaledTime = false;

    [Header("Behavior")]
    [SerializeField] private bool disableOnHide = false;

    private Coroutine _showRoutine;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (disableOnHide)
            gameObject.SetActive(false);
    }

    public void Show()
    {
        if (canvasGroup == null)
            return;

        if (disableOnHide && !gameObject.activeSelf)
            gameObject.SetActive(true);

        if (_showRoutine != null)
            StopCoroutine(_showRoutine);

        _showRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        yield return FadeTo(1f, fadeInDuration);

        float t = 0f;
        while (t < visibleDuration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }

        yield return FadeTo(0f, fadeOutDuration);

        if (disableOnHide)
            gameObject.SetActive(false);

        _showRoutine = null;
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (duration <= 0f)
        {
            canvasGroup.alpha = targetAlpha;
            yield break;
        }

        float start = canvasGroup.alpha;
        float t = 0f;

        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float k = Mathf.Clamp01(t / duration);
            canvasGroup.alpha = Mathf.Lerp(start, targetAlpha, k);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}

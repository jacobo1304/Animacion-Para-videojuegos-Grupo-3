using System.Collections;
using UnityEngine;

public class AttackCameraZoomController : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float defaultFov = 60f;
    [SerializeField] private bool useUnscaledTime;

    private float currentTargetFov;
    private float currentSmoothTime;
    private float fovVelocity;
    private bool hasDefaultFov;
    private Coroutine zoomRoutine;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = GetComponentInChildren<Camera>();
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }
        }

        if (targetCamera != null)
        {
            defaultFov = targetCamera.fieldOfView;
            hasDefaultFov = true;
            currentTargetFov = defaultFov;
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null || !hasDefaultFov)
        {
            return;
        }

        float delta = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        float newFov = Mathf.SmoothDamp(
            targetCamera.fieldOfView,
            currentTargetFov,
            ref fovVelocity,
            currentSmoothTime,
            Mathf.Infinity,
            delta
        );
        targetCamera.fieldOfView = newFov;
    }

    public void StartZoom(float targetFov, float smoothTime)
    {
        if (targetCamera == null)
        {
            return;
        }

        currentTargetFov = targetFov;
        currentSmoothTime = Mathf.Max(0.01f, smoothTime);
    }

    public void StartZoomPulse(float targetFov, float zoomInTime, float holdTime, float zoomOutTime)
    {
        if (targetCamera == null)
        {
            return;
        }

        if (zoomRoutine != null)
        {
            StopCoroutine(zoomRoutine);
        }

        zoomRoutine = StartCoroutine(ZoomPulseRoutine(targetFov, zoomInTime, holdTime, zoomOutTime));
    }

    public void ResetZoom(float smoothTime)
    {
        if (targetCamera == null || !hasDefaultFov)
        {
            return;
        }

        currentTargetFov = defaultFov;
        currentSmoothTime = Mathf.Max(0.01f, smoothTime);
    }

    private IEnumerator ZoomPulseRoutine(float targetFov, float zoomInTime, float holdTime, float zoomOutTime)
    {
        StartZoom(targetFov, zoomInTime);
        if (holdTime > 0f)
        {
            if (useUnscaledTime)
            {
                yield return new WaitForSecondsRealtime(holdTime);
            }
            else
            {
                yield return new WaitForSeconds(holdTime);
            }
        }

        ResetZoom(zoomOutTime);
        zoomRoutine = null;
    }
}

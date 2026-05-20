using UnityEngine;

public class FloatAnimation : MonoBehaviour
{
    [Header("Float Y Range")]
    [SerializeField] private float fromY = 0.0f;
    [SerializeField] private float toY = 0.5f;

    [Header("Timing")]
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private AnimationCurve ease = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Play Options")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool pingPong = true;

    private Vector3 initialPosition;
    private float time;
    private bool playing = false;

    private void Awake()
    {
        initialPosition = transform.position;
        // If clip was authored with absolute world/local positions, allow using the current y as base
        if (Application.isPlaying && playOnStart)
            Play();
    }

    private void Update()
    {
        if (!playing || duration <= 0f) return;

        time += Time.deltaTime;
        float t = Mathf.Clamp01(time / duration);
        float eased = ease.Evaluate(t);
        float y = Mathf.Lerp(fromY, toY, eased);

        Vector3 pos = transform.position;
        pos.y = initialPosition.y + y;
        transform.position = pos;

        if (time >= duration)
        {
            if (pingPong)
            {
                // swap from/to and restart
                float tmp = fromY; fromY = toY; toY = tmp;
                time = 0f;
            }
            else
            {
                time = 0f; // loop
            }
        }
    }

    public void Play()
    {
        playing = true;
        time = 0f;
    }

    public void Stop()
    {
        playing = false;
    }

    public void ResetToStart()
    {
        time = 0f;
        Vector3 pos = transform.position;
        pos.y = initialPosition.y + fromY;
        transform.position = pos;
    }
}

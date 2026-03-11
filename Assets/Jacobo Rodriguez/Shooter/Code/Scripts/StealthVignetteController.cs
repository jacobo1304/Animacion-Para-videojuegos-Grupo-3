using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Attach to the full-screen UI Image that uses the StealthVignette material.
/// Drag the Character reference in the Inspector.
/// </summary>
[RequireComponent(typeof(Image))]
public class StealthVignetteController : MonoBehaviour
{
    [SerializeField] private Character character;

    [Tooltip("Max vignette intensity when stealth is active (0–1).")]
    [SerializeField] private float stealthIntensity = 0.85f;

    [Tooltip("How fast the vignette fades in / out. Higher = faster.")]
    [SerializeField] private float transitionSpeed = 4f;

    private Material _material;
    private static readonly int IntensityProp = Shader.PropertyToID("_Intensity");

    private void Awake()
    {
        // Instantiate the material so we don't modify the shared asset
        Image img = GetComponent<Image>();
        _material = new Material(img.material);
        img.material = _material;

        _material.SetFloat(IntensityProp, 0f);
    }

    private void Update()
    {
        float target  = character.IsStealth ? stealthIntensity : 0f;
        float current = _material.GetFloat(IntensityProp);
        float next    = Mathf.MoveTowards(current, target, transitionSpeed * Time.deltaTime);
        _material.SetFloat(IntensityProp, next);
    }
}

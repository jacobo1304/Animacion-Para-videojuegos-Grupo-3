using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterStealth : MonoBehaviour, ICharacterComponent
{
    public Character ParentCharacter { get; set; }

    private Animator _animator;
    private static readonly int IsStealthHash = Animator.StringToHash("isStealth");

    [Header("Animation Layers")]
    [Tooltip("Animator layer name used for stealth/crouch blending.")]
    [SerializeField] private string stealthLayerName = "Stealth";

    [Tooltip("Seconds to fade the Stealth layer weight (0 ↔ 1).")]
    [SerializeField] private float stealthLayerFadeTime = 0.12f;

    private int _stealthLayerIndex = -1;
    private Coroutine _stealthLayerFadeRoutine;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator != null)
            _stealthLayerIndex = _animator.GetLayerIndex(stealthLayerName);
    }

    public void OnStealth(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            SetStealth(true);
        else if (ctx.canceled)
            SetStealth(false);
    }

    private void SetStealth(bool value)
    {
        if (ParentCharacter != null)
            ParentCharacter.IsStealth = value;

        if (_animator == null)
            return;

        _animator.SetBool(IsStealthHash, value);

        if (_stealthLayerIndex >= 0)
            FadeStealthLayerWeight(value ? 1f : 0f);
    }

    private void FadeStealthLayerWeight(float targetWeight)
    {
        if (_animator == null)
            return;

        if (_stealthLayerIndex < 0)
            return;

        if (stealthLayerFadeTime <= 0f)
        {
            _animator.SetLayerWeight(_stealthLayerIndex, targetWeight);
            return;
        }

        if (_stealthLayerFadeRoutine != null)
            StopCoroutine(_stealthLayerFadeRoutine);

        _stealthLayerFadeRoutine = StartCoroutine(FadeLayerWeightRoutine(_stealthLayerIndex, targetWeight, stealthLayerFadeTime));
    }

    private System.Collections.IEnumerator FadeLayerWeightRoutine(int layerIndex, float targetWeight, float duration)
    {
        var startWeight = _animator.GetLayerWeight(layerIndex);
        var t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            var a = Mathf.Clamp01(t / duration);
            _animator.SetLayerWeight(layerIndex, Mathf.Lerp(startWeight, targetWeight, a));
            yield return null;
        }

        _animator.SetLayerWeight(layerIndex, targetWeight);
        _stealthLayerFadeRoutine = null;
    }
}

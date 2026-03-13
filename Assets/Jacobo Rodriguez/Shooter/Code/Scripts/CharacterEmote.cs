using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterEmote : MonoBehaviour, ICharacterComponent
{
    public Character ParentCharacter { get; set; }

    [SerializeField] private Animator animator;

    [Header("Animation Layers")]
    [Tooltip("Animator layer name used for emote blending.")]
    [SerializeField] private string emoteLayerName = "Emote";

    [Tooltip("Seconds to fade the Emote layer weight (0 ↔ 1).")]
    [SerializeField] private float emoteLayerFadeTime = 0.12f;

    [Tooltip("Animator state name to restart when re-triggering the emote. If empty, no explicit restart is forced.")]
    [SerializeField] private string emoteStateName = "Emote";

    [Tooltip("Minimum seconds the player must wait before triggering another emote.")]
    [SerializeField] private float timeForEmoteAgain = 2f;

    [Tooltip("Base locomotion state to cross-fade into when the emote is interrupted.")]
    [SerializeField] private string locomotionStateName = "Locomotion";

    [Tooltip("Blend duration (seconds) when forcibly interrupting the emote.")]
    [SerializeField] private float interruptBlendTime = 0.15f;

    private float _lastEmoteTime = -999f;
    private int _emoteLayerIndex = -1;
    private Coroutine _emoteLayerFadeRoutine;
    private CharacterMovement _characterMovement;

    private static readonly int IsEmotingHash = Animator.StringToHash("isEmoting");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _characterMovement = GetComponent<CharacterMovement>();
        if (animator != null)
            _emoteLayerIndex = animator.GetLayerIndex(emoteLayerName);

        if (ParentCharacter != null)
            ParentCharacter.IsEmoting = false;

        if (animator != null)
            animator.SetBool(IsEmotingHash, false);

        SetEmoteLayerWeightImmediate(0f);
    }

    private void Update()
    {
        if (ParentCharacter == null) return;
        if (!ParentCharacter.IsEmoting) return;

        // Combat actions interrupt the emote immediately.
        if (ParentCharacter.IsAiming || ParentCharacter.IsReloading || ParentCharacter.IsFiring)
        {
            InterruptEmote();
            return;
        }

        // Any movement input interrupts the emote.
        if (ParentCharacter.MovementInput.magnitude > 0.1f)
            InterruptEmote(true);
    }

    public void OnEmote(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (ParentCharacter == null) return;
        if (animator == null) return;
        if (ParentCharacter.IsAiming)    return;
        if (ParentCharacter.IsReloading) return;
        if (ParentCharacter.IsFiring)    return;
        if (Time.time < _lastEmoteTime + timeForEmoteAgain) return;

        var wasEmoting = ParentCharacter.IsEmoting;

        // Emoting always forces unequip.
        var equip = GetComponent<CharacterEquip>();
        if (equip != null)
            equip.ForceUnequip();

        // Allows re-triggering a new emote even if one is already playing,
        // as long as the cooldown has elapsed.
        ParentCharacter.IsEmoting = true;
        _lastEmoteTime = Time.time;
        FadeEmoteLayerWeight(1f);
        animator.SetBool(IsEmotingHash, true);

        // If the bool was already true, force a restart so the emote can play again.
        if (wasEmoting && !string.IsNullOrWhiteSpace(emoteStateName))
        {
            var layerToRestart = _emoteLayerIndex >= 0 ? _emoteLayerIndex : 0;
            animator.Play(emoteStateName, layerToRestart, 0f);
        }

        Debug.Log(wasEmoting ? "Emote re-triggered" : "Emote started");
    }

    // Called by Animation Event at the end of the emote clip — clean exit.
    public void EndEmote()
    {
        if (ParentCharacter != null)
            ParentCharacter.IsEmoting = false;
        if (animator != null)
            animator.SetBool(IsEmotingHash, false);
        FadeEmoteLayerWeight(0f);
        Debug.Log("Emote ended");
    }

    // Called internally when an incompatible action becomes active mid-emote.
    private void InterruptEmote(bool interruptedByMovement = false)
    {
        if (ParentCharacter != null)
            ParentCharacter.IsEmoting = false;
        if (animator != null)
            animator.SetBool(IsEmotingHash, false);

        // Important: if movement caused the interruption, immediately push the current input
        // into CharacterMovement so the character moves right away (no second key press needed).
        if (interruptedByMovement && ParentCharacter != null && _characterMovement != null)
            _characterMovement.ApplyMovementInput(ParentCharacter.MovementInput, true);

        FadeEmoteLayerWeight(0f);
        if (animator != null)
        {
            animator.CrossFade(locomotionStateName, interruptBlendTime, 0);
        }
        Debug.Log("Emote interrupted");
    }

    private void FadeEmoteLayerWeight(float targetWeight)
    {
        if (animator == null)
            return;

        if (_emoteLayerIndex < 0)
            return;

        if (emoteLayerFadeTime <= 0f)
        {
            animator.SetLayerWeight(_emoteLayerIndex, targetWeight);
            return;
        }

        if (_emoteLayerFadeRoutine != null)
            StopCoroutine(_emoteLayerFadeRoutine);

        _emoteLayerFadeRoutine = StartCoroutine(FadeLayerWeightRoutine(_emoteLayerIndex, targetWeight, emoteLayerFadeTime));
    }

    private void SetEmoteLayerWeightImmediate(float weight)
    {
        if (animator == null)
            return;

        if (_emoteLayerIndex < 0)
            return;

        if (_emoteLayerFadeRoutine != null)
        {
            StopCoroutine(_emoteLayerFadeRoutine);
            _emoteLayerFadeRoutine = null;
        }

        animator.SetLayerWeight(_emoteLayerIndex, weight);
    }

    private System.Collections.IEnumerator FadeLayerWeightRoutine(int layerIndex, float targetWeight, float duration)
    {
        var startWeight = animator.GetLayerWeight(layerIndex);
        var t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            var a = Mathf.Clamp01(t / duration);
            animator.SetLayerWeight(layerIndex, Mathf.Lerp(startWeight, targetWeight, a));
            yield return null;
        }

        animator.SetLayerWeight(layerIndex, targetWeight);
        _emoteLayerFadeRoutine = null;
    }
}

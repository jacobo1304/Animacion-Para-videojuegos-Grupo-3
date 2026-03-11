using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterEmote : MonoBehaviour, ICharacterComponent
{
    public Character ParentCharacter { get; set; }

    [SerializeField] private Animator animator;

    [Tooltip("Minimum seconds the player must wait before triggering another emote.")]
    [SerializeField] private float timeForEmoteAgain = 2f;

    [Tooltip("Base locomotion state to cross-fade into when the emote is interrupted.")]
    [SerializeField] private string locomotionStateName = "Locomotion";

    [Tooltip("Blend duration (seconds) when forcibly interrupting the emote.")]
    [SerializeField] private float interruptBlendTime = 0.15f;

    private float _lastEmoteTime = -999f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ParentCharacter.IsEmoting = false;
    }

    private void Update()
    {
        if (!ParentCharacter.IsEmoting) return;

        // Combat actions interrupt the emote immediately.
        if (ParentCharacter.IsAiming || ParentCharacter.IsReloading || ParentCharacter.IsFiring)
        {
            InterruptEmote();
            return;
        }

        // Any movement input interrupts the emote.
        if (ParentCharacter.MovementInput.magnitude > 0.1f)
            InterruptEmote();
    }

    public void OnEmote(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (ParentCharacter.IsAiming)    return;
        if (ParentCharacter.IsReloading) return;
        if (ParentCharacter.IsFiring)    return;
        if (Time.time < _lastEmoteTime + timeForEmoteAgain) return;

        // Allows re-triggering a new emote even if one is already playing,
        // as long as the cooldown has elapsed.
        ParentCharacter.IsEmoting = true;
        _lastEmoteTime = Time.time;
        animator.ResetTrigger("Emote");
        animator.SetTrigger("Emote");
        Debug.Log(ParentCharacter.IsEmoting ? "Emote re-triggered" : "Emote started");
    }

    // Called by Animation Event at the end of the emote clip — clean exit.
    public void EndEmote()
    {
        ParentCharacter.IsEmoting = false;
        Debug.Log("Emote ended");
    }

    // Called internally when an incompatible action becomes active mid-emote.
    private void InterruptEmote()
    {
        ParentCharacter.IsEmoting = false;
        animator.ResetTrigger("Emote");
        animator.CrossFade(locomotionStateName, interruptBlendTime, 0);
        Debug.Log("Emote interrupted");
    }
}

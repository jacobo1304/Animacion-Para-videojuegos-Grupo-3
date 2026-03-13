using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class CharacterAim : MonoBehaviour, ICharacterComponent
{
    public Character ParentCharacter { get; set; }

    [Header("Camera")]
    [SerializeField] private CinemachineCamera aimCamera;

    [Header("Aim System")]
    [SerializeField] private FloatDampener aimDampener;
    [SerializeField] private AimConstraint aimConstraint;

    [Header("Debug / Manual Control")]
    [Tooltip("If checked in inspector, character keeps aiming while it remains checked.")]
    [SerializeField] private bool aimWhileChecked;

    private Animator animator;
    private bool isAiming;
    private bool _inputAimHeld;
    private bool _lastAimWhileChecked;
    private CharacterEquip _characterEquip;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        _characterEquip = GetComponent<CharacterEquip>();
        _lastAimWhileChecked = aimWhileChecked;
    }

    public void OnAim(InputAction.CallbackContext ctx)
    {
        if (ParentCharacter == null) return;
        if (!ctx.started && !ctx.canceled || ParentCharacter.IsEmoting) return;

        if (ctx.started && !ParentCharacter.IsEquipped)
            return;

        if (ctx.started)
            _inputAimHeld = true;
        else if (ctx.canceled)
            _inputAimHeld = false;

        EvaluateAimState();
    }

    private void EvaluateAimState()
    {
        if (ParentCharacter == null)
            return;

        bool wantsAim = (_inputAimHeld || aimWhileChecked)
                        && ParentCharacter.IsEquipped
                        && !ParentCharacter.IsEmoting;

        isAiming = wantsAim;

        aimCamera?.gameObject.SetActive(isAiming);

        ParentCharacter.IsAiming = isAiming;
        aimConstraint.enabled = isAiming;
        aimDampener.TargetValue = isAiming ? 1 : 0;

        if (_characterEquip != null)
            _characterEquip.OnAimStateChanged(isAiming);
    }

    public void StopAim()
    {
        isAiming = false;

        if (ParentCharacter != null)
            ParentCharacter.IsAiming = false;

        if (aimCamera != null)
            aimCamera.gameObject.SetActive(false);

        if (aimConstraint != null)
            aimConstraint.enabled = false;

        aimDampener.TargetValue = 0f;

        if (_characterEquip != null)
            _characterEquip.OnAimStateChanged(false);
    }

    private void Update()
    {
        if (aimWhileChecked != _lastAimWhileChecked)
        {
            _lastAimWhileChecked = aimWhileChecked;
            EvaluateAimState();
        }

        if (ParentCharacter != null && !ParentCharacter.IsEquipped && isAiming)
            StopAim();

        aimDampener.Update();
        aimConstraint.weight = aimDampener.CurrentValue;

        animator.SetLayerWeight(1, aimDampener.CurrentValue);
    }
}
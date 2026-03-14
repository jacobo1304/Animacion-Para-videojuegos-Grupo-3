using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class CharacterEquip : MonoBehaviour, ICharacterComponent
{
    private enum PendingEquipAction
    {
        None,
        Equip,
        Unequip
    }

    public Character ParentCharacter { get; set; }

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string equipTriggerName = "Equip";
    [SerializeField] private string unequipTriggerName = "Unequip";

    [Header("Rig Layers")]
    [Tooltip("Rig that constrains the weapon to the hands when equipped.")]
    [SerializeField] private Rig equippedRig;

    [Tooltip("Rig that keeps the weapon in the unequipped pose/holster.")]
    [SerializeField] private Rig unequippedRig;

    [Tooltip("Action rig used while EQUIPPING. Its inner constraints should stay at weight 1.")]
    [SerializeField] private Rig equipActionRig;

    [Tooltip("Action rig used while UNEQUIPPING. Its inner constraints should stay at weight 1.")]
    [SerializeField] private Rig unequipActionRig;

    [Header("Equipped Rig Constraints")]
    [Tooltip("Constraint for normal equipped pose (non-aim).")]
    [SerializeField] private MultiParentConstraint equippedNormalConstraint;

    [Tooltip("Constraint for aiming equipped pose.")]
    [SerializeField] private MultiParentConstraint equippedAimConstraint;

    [Header("Animation Event Action Rig Weights")]
    [Tooltip("During the grab window, action rig layers drive the weapon. Base rig layers stay at 0.")]
    [SerializeField] private bool useActionRigLayers = true;

    [Tooltip("If true, normal front-hold constraints are disabled while a temporary grab constraint is active.")]
    [SerializeField] private bool disableFrontConstraintsWhileGrab = true;

    [Tooltip("If true, release events complete the transition immediately. If false, keep base rig layers at 0 and complete with OnFinishEquip/OnFinishUnequip.")]
    [SerializeField] private bool completeTransitionOnRelease = false;

    [Header("Fallback")]
    [Tooltip("If transition events fail, force base rig layers after this time without changing equip state.")]
    [SerializeField] private bool enableTransitionFallback = true;
    [SerializeField, Min(0.1f)] private float transitionFallbackSeconds = 2f;

    [Header("Debug")]
    [Tooltip("Enable debug logs for equip/unequip actions.")]
    [SerializeField] private bool enableDebug = false;

    private bool _isEquipped;
    private bool _isTransitioning;
    private PendingEquipAction _pendingAction;
    private CharacterAim _characterAim;
    private float _transitionStartTime;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        _characterAim = GetComponent<CharacterAim>();

        if (enableDebug)
            Debug.Log("CharacterEquip Awake: initializing. equippedRig=" + (equippedRig!=null) + ", unequippedRig=" + (unequippedRig!=null));

        if (equippedRig != null && unequippedRig != null)
            _isEquipped = equippedRig.weight >= unequippedRig.weight;
        else if (equippedRig != null)
            _isEquipped = equippedRig.weight > 0.5f;
        else if (unequippedRig != null)
            _isEquipped = !(unequippedRig.weight > 0.5f);

        ApplyRigWeights();
        ClearActionRigs();
    }


    public void OnEquip(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

        ToggleEquip();
    }
    public void OnTryEquipWithClick(InputAction.CallbackContext ctx)
    {
     if (!ctx.performed)
            return;

        TratardeEquiparConClick();
    }
    private void ToggleEquip()
    {

        if (_isTransitioning)
            return;

        if (_isEquipped)
            BeginUnequipTransition();
        else
            BeginEquipTransition();
    }
     void  TratardeEquiparConClick()
    {
        if (!_isEquipped && !_isTransitioning)
            BeginEquipTransition();
    }

    public void ForceUnequip()
    {
        if (!_isEquipped && !_isTransitioning)
            return;

        _isTransitioning = false;
        _pendingAction = PendingEquipAction.None;
        _transitionStartTime = 0f;
        _isEquipped = false;

        if (enableDebug)
            Debug.Log("ForceUnequip: emote/action forced weapon unequip.");

        ClearActionRigs();
        ApplyRigWeights();
    }

    private void BeginEquipTransition()
    {
        // Input intent: equip must become true in all cases (events, finish, fallback).
        _isEquipped = true;
        _isTransitioning = true;
        _pendingAction = PendingEquipAction.Equip;
        _transitionStartTime = Time.time;

        if (enableDebug)
            Debug.Log("BeginEquipTransition: trigger=" + equipTriggerName);

        if (_characterAim != null)
            _characterAim.StopAim();

        // During equip action, disable base rig layers so weapon is moved only by temporary action constraints.
        if (ParentCharacter != null)
            ParentCharacter.IsEquipped = false;
        ApplyTransitionRigWeights();
        RefreshEquippedConstraintWeights();

        if (animator != null && !string.IsNullOrEmpty(equipTriggerName))
        {
            if (!string.IsNullOrEmpty(unequipTriggerName))
                animator.ResetTrigger(unequipTriggerName);
            animator.SetTrigger(equipTriggerName);
        }
        else
        {
            CompleteEquipTransition();
        }
    }

    private void BeginUnequipTransition()
    {
        // Input intent: unequip must become false in all cases (events, finish, fallback).
        _isEquipped = false;
        _isTransitioning = true;
        _pendingAction = PendingEquipAction.Unequip;
        _transitionStartTime = Time.time;

        if (enableDebug)
            Debug.Log("BeginUnequipTransition: trigger=" + unequipTriggerName);

        if (_characterAim != null)
            _characterAim.StopAim();

        // During unequip action, disable base rig layers so weapon is moved only by temporary action constraints.
        if (ParentCharacter != null)
            ParentCharacter.IsEquipped = false;
        ApplyTransitionRigWeights();
        RefreshEquippedConstraintWeights();

        if (animator != null && !string.IsNullOrEmpty(unequipTriggerName))
        {
            if (!string.IsNullOrEmpty(equipTriggerName))
                animator.ResetTrigger(equipTriggerName);
            animator.SetTrigger(unequipTriggerName);
        }
        else
        {
            CompleteUnequipTransition();
        }
    }

    private void CompleteEquipTransition()
    {
        _isEquipped = true;
        _isTransitioning = false;
        _pendingAction = PendingEquipAction.None;
        _transitionStartTime = 0f;
        ClearActionRigs();
        ApplyRigWeights();
    }

    private void CompleteUnequipTransition()
    {
        _isEquipped = false;
        _isTransitioning = false;
        _pendingAction = PendingEquipAction.None;
        _transitionStartTime = 0f;
        ClearActionRigs();
        ApplyRigWeights();
    }

    // Animation Event: call when hand grabs weapon during EQUIP clip
    public void OnGrabEquip()
    {
        LogEventState("OnGrabEquip");

        if (_pendingAction != PendingEquipAction.Equip)
            return;

        SetActionRigWeights(1f, 0f);
    }

    // Animation Event helper: can be used in both clips. It routes based on current pending action.
    public void OnGrab()
    {
        LogEventState("OnGrab (router)");

        if (_pendingAction == PendingEquipAction.Equip)
            OnGrabEquip();
        else if (_pendingAction == PendingEquipAction.Unequip)
            OnGrabUnequip();
    }

    // Animation Event: call when equip hand-off is done and weapon should return to normal equipped constraints
    public void OnReleaseEquip()
    {
        LogEventState("OnReleaseEquip");

        if (_pendingAction != PendingEquipAction.Equip)
            return;

        SetActionRigWeights(0f, 0f);

        // Release event now defines final equipped state.
        _isEquipped = true;
        _isTransitioning = false;
        _pendingAction = PendingEquipAction.None;
        _transitionStartTime = 0f;

        if (ParentCharacter != null)
            ParentCharacter.IsEquipped = true;

        ForceBaseRigWeightsFromState();
        RefreshEquippedConstraintWeights();

        if (completeTransitionOnRelease)
            CompleteEquipTransition();
    }

    // Animation Event: call when hand grabs weapon during UNEQUIP clip
    public void OnGrabUnequip()
    {
        LogEventState("OnGrabUnequip");

        if (_pendingAction != PendingEquipAction.Unequip)
            return;

        SetActionRigWeights(0f, 1f);
    }

    // Animation Event: call when unequip hand-off is done and weapon should return to holster constraints
    public void OnReleaseUnequip()
    {
        LogEventState("OnReleaseUnequip");

        if (_pendingAction != PendingEquipAction.Unequip)
            return;

        SetActionRigWeights(0f, 0f);

        // Release event now defines final unequipped state.
        _isEquipped = false;
        _isTransitioning = false;
        _pendingAction = PendingEquipAction.None;
        _transitionStartTime = 0f;

        if (ParentCharacter != null)
            ParentCharacter.IsEquipped = false;

        ForceBaseRigWeightsFromState();
        RefreshEquippedConstraintWeights();

        if (completeTransitionOnRelease)
            CompleteUnequipTransition();
    }

    // Animation Event helper: can be used in both clips. It routes based on current pending action.
    public void OnRelease()
    {
        LogEventState("OnRelease (router)");

        if (_pendingAction == PendingEquipAction.Equip)
            OnReleaseEquip();
        else if (_pendingAction == PendingEquipAction.Unequip)
            OnReleaseUnequip();
    }

    // Animation Event: place near the end of equip clip to enable normal equipped rig state.
    public void OnFinishEquip()
    {
        LogEventState("OnFinishEquip");

        if (_pendingAction != PendingEquipAction.Equip)
            return;

        CompleteEquipTransition();
    }

    // Animation Event: place near the end of unequip clip to enable normal holster rig state.
    public void OnFinishUnequip()
    {
        LogEventState("OnFinishUnequip");

        if (_pendingAction != PendingEquipAction.Unequip)
            return;

        CompleteUnequipTransition();
    }

    private void SetActionRigWeights(float equipAction, float unequipAction)
    {
        if (useActionRigLayers)
        {
            if (equipActionRig != null)
                equipActionRig.weight = Mathf.Clamp01(equipAction);

            if (unequipActionRig != null)
                unequipActionRig.weight = Mathf.Clamp01(unequipAction);
        }

        if (enableDebug)
        {
            Debug.Log("SetActionRigWeights => equipAction=" + equipAction + ", unequipAction=" + unequipAction +
                      " | equippedRig=" + GetRigWeight(equippedRig) +
                      " unequippedRig=" + GetRigWeight(unequippedRig) +
                      " equipActionRig=" + GetRigWeight(equipActionRig) +
                      " unequipActionRig=" + GetRigWeight(unequipActionRig));
        }

        if (disableFrontConstraintsWhileGrab && IsAnyActionRigActive())
        {
            if (equippedNormalConstraint != null)
                equippedNormalConstraint.weight = 0f;
            if (equippedAimConstraint != null)
                equippedAimConstraint.weight = 0f;
        }
    }

    private void ClearActionRigs()
    {
        SetActionRigWeights(0f, 0f);
    }

    private void ApplyTransitionRigWeights()
    {
        // Keep both base rig layers off while action constraints take control.
        if (equippedRig != null)
            equippedRig.weight = 0f;

        if (unequippedRig != null)
            unequippedRig.weight = 0f;

        // During transitions, action rigs are controlled only by animation events.
        if (useActionRigLayers)
        {
            if (equipActionRig != null && _pendingAction != PendingEquipAction.Equip)
                equipActionRig.weight = 0f;

            if (unequipActionRig != null && _pendingAction != PendingEquipAction.Unequip)
                unequipActionRig.weight = 0f;
        }

        if (enableDebug)
        {
            Debug.Log("ApplyTransitionRigWeights => equippedRig=" + GetRigWeight(equippedRig) +
                      " unequippedRig=" + GetRigWeight(unequippedRig) +
                      " equipActionRig=" + GetRigWeight(equipActionRig) +
                      " unequipActionRig=" + GetRigWeight(unequipActionRig));
        }
    }

    private void ApplyRigWeights()
    {
        if (enableDebug)
        {
            Debug.Log("ApplyRigWeights: setting weights. equippedRigPresent=" + (equippedRig!=null) + " -> " + (_isEquipped?1f:0f) + ", unequippedRigPresent=" + (unequippedRig!=null) + " -> " + (_isEquipped?0f:1f));
        }

        if (ParentCharacter != null)
            ParentCharacter.IsEquipped = _isEquipped;

        ForceBaseRigWeightsFromState();

        ClearActionRigs();

        RefreshEquippedConstraintWeights();

        if (!_isEquipped && _characterAim != null)
            _characterAim.StopAim();
    }

    public void OnAimStateChanged(bool isAiming)
    {
        if (enableDebug)
            Debug.Log("OnAimStateChanged: isAiming=" + isAiming + ", isEquipped=" + _isEquipped);

        RefreshEquippedConstraintWeights();
    }

    private void RefreshEquippedConstraintWeights()
    {
        bool canUseEquippedConstraints = _isEquipped && !_isTransitioning && !IsAnyActionRigActive() && ParentCharacter != null && ParentCharacter.IsEquipped;
        bool aiming = canUseEquippedConstraints && ParentCharacter != null && ParentCharacter.IsAiming;

        if (equippedNormalConstraint != null)
            equippedNormalConstraint.weight = canUseEquippedConstraints && !aiming ? 1f : 0f;

        if (equippedAimConstraint != null)
            equippedAimConstraint.weight = aiming ? 1f : 0f;
    }

    private bool IsAnyActionRigActive()
    {
        if (!useActionRigLayers)
            return false;

        bool equipAction = equipActionRig != null && equipActionRig.weight > 0.001f;
        bool unequipAction = unequipActionRig != null && unequipActionRig.weight > 0.001f;
        return equipAction || unequipAction;
    }

    private void Update()
    {
        if (!enableTransitionFallback || !_isTransitioning)
            return;

        if (Time.time - _transitionStartTime < transitionFallbackSeconds)
            return;

        if (enableDebug)
        {
            Debug.Log("[CharacterEquip Fallback] Transition timeout reached. Forcing base layers from current state without changing _isEquipped. isEquipped=" + _isEquipped +
                      " pending=" + _pendingAction +
                      " elapsed=" + (Time.time - _transitionStartTime).ToString("0.00") + "s");
        }

        _isTransitioning = false;
        _pendingAction = PendingEquipAction.None;
        _transitionStartTime = 0f;

        ClearActionRigs();
        ForceBaseRigWeightsFromState();

        if (ParentCharacter != null)
            ParentCharacter.IsEquipped = _isEquipped;

        RefreshEquippedConstraintWeights();
    }

    private void LateUpdate()
    {
        // Hard-enforce base rig layers from state to avoid runtime/inspector desync.
        if (!_isTransitioning)
            ForceBaseRigWeightsFromState();
    }

    private void ForceBaseRigWeightsFromState()
    {
        if (equippedRig != null)
            equippedRig.weight = _isEquipped ? 1f : 0f;

        if (unequippedRig != null)
            unequippedRig.weight = _isEquipped ? 0f : 1f;

        if (enableDebug)
        {
            Debug.Log("ForceBaseRigWeightsFromState => isEquipped=" + _isEquipped +
                      " | equippedRig=" + GetRigWeight(equippedRig) +
                      " unequippedRig=" + GetRigWeight(unequippedRig));
        }
    }

    private void LogEventState(string eventName)
    {
        if (!enableDebug)
            return;

        Debug.Log("[CharacterEquip Event] " + eventName +
                  " | pending=" + _pendingAction +
                  " transitioning=" + _isTransitioning +
                  " isEquipped=" + _isEquipped +
                  " | equippedRig=" + GetRigWeight(equippedRig) +
                  " unequippedRig=" + GetRigWeight(unequippedRig) +
                  " equipActionRig=" + GetRigWeight(equipActionRig) +
                  " unequipActionRig=" + GetRigWeight(unequipActionRig));
    }

    private static string GetRigWeight(Rig rig)
    {
        return rig == null ? "null" : rig.weight.ToString("0.###");
    }

   
   
}

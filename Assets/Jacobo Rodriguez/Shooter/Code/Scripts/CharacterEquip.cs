using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

public class CharacterEquip : MonoBehaviour, ICharacterComponent
{
    public Character ParentCharacter { get; set; }

    [Header("Rig Layers")]
    [Tooltip("Rig that constrains the weapon to the hands when equipped.")]
    [SerializeField] private Rig equippedRig;

    [Tooltip("Rig that keeps the weapon in the unequipped pose/holster.")]
    [SerializeField] private Rig unequippedRig;

    [Header("Equipped Rig Constraints")]
    [Tooltip("Constraint for normal equipped pose (non-aim).")]
    [SerializeField] private MultiParentConstraint equippedNormalConstraint;

    [Tooltip("Constraint for aiming equipped pose.")]
    [SerializeField] private MultiParentConstraint equippedAimConstraint;

    [Header("Debug")]
    [Tooltip("Enable debug logs for equip/unequip actions.")]
    [SerializeField] private bool enableDebug = false;

    private bool _isEquipped;
    private CharacterAim _characterAim;

    private void Awake()
    {
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
    }


    public void OnEquip(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed)
            return;

        ToggleEquip();
    }

    private void ToggleEquip()
    {
        _isEquipped = !_isEquipped;
        if (enableDebug)
            Debug.Log("ToggleEquip: now equipped=" + _isEquipped);

        ApplyRigWeights();
    }

    public void ForceUnequip()
    {
        if (!_isEquipped)
            return;

        _isEquipped = false;

        if (enableDebug)
            Debug.Log("ForceUnequip: emote/action forced weapon unequip.");

        ApplyRigWeights();
    }

    private void ApplyRigWeights()
    {
        if (enableDebug)
        {
            Debug.Log("ApplyRigWeights: setting weights. equippedRigPresent=" + (equippedRig!=null) + " -> " + (_isEquipped?1f:0f) + ", unequippedRigPresent=" + (unequippedRig!=null) + " -> " + (_isEquipped?0f:1f));
        }

        if (ParentCharacter != null)
            ParentCharacter.IsEquipped = _isEquipped;

        if (equippedRig != null)
            equippedRig.weight = _isEquipped ? 1f : 0f;

        if (unequippedRig != null)
            unequippedRig.weight = _isEquipped ? 0f : 1f;

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
        bool canUseEquippedConstraints = _isEquipped && ParentCharacter != null && ParentCharacter.IsEquipped;
        bool aiming = canUseEquippedConstraints && ParentCharacter != null && ParentCharacter.IsAiming;

        if (equippedNormalConstraint != null)
            equippedNormalConstraint.weight = canUseEquippedConstraints && !aiming ? 1f : 0f;

        if (equippedAimConstraint != null)
            equippedAimConstraint.weight = aiming ? 1f : 0f;
    }
}

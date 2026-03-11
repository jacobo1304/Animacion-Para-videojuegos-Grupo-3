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

    [Header("Weapon")]
    [SerializeField] private Transform weaponPivot;

    [Header("Sockets")]
    [SerializeField] private Transform holderSocket;
    [SerializeField] private Transform rightHandSocket;
    [SerializeField] private Transform leftHandSocket;

    [Header("Gun Points")]
    [SerializeField] private Transform gunGrip;
    [SerializeField] private Transform gunTop;

    private Animator animator;
    private bool isAiming;

    private static readonly Vector3 holderRotation =
        new Vector3(82.1f, -180f, -79.3f);

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
    
        MoveWeaponToHolder();
    }

    public void OnAim(InputAction.CallbackContext ctx)
    {
        if (!ctx.started && !ctx.canceled || ParentCharacter.IsEmoting) return;

        isAiming = ctx.started;

        aimCamera?.gameObject.SetActive(isAiming);

        ParentCharacter.IsAiming = isAiming;
        aimConstraint.enabled = isAiming;
        aimDampener.TargetValue = isAiming ? 1 : 0;

        if (!isAiming)
        {
            MoveWeaponToHolder();
        }
    }

    private void Update()
    {
        aimDampener.Update();
        aimConstraint.weight = aimDampener.CurrentValue;

        animator.SetLayerWeight(1, aimDampener.CurrentValue);
    }

    private void LateUpdate()
    {
        if (isAiming)
        {
            AlignWeaponToHands();
        }
    }

    void MoveWeaponToHolder()
    {
        weaponPivot.SetParent(holderSocket);

        weaponPivot.localPosition = Vector3.zero;

        weaponPivot.localRotation =
            Quaternion.Euler(holderRotation);
    }

    void AlignWeaponToHands()
    {
        weaponPivot.SetParent(null);

        Vector3 gripToPivot = weaponPivot.position - gunGrip.position;

        weaponPivot.position = rightHandSocket.position + gripToPivot;

        Vector3 gunDir =
            (gunTop.position - gunGrip.position).normalized;

        Vector3 handsDir =
            (leftHandSocket.position - rightHandSocket.position).normalized;

        Quaternion rot =
            Quaternion.FromToRotation(gunDir, handsDir);

        weaponPivot.rotation = rot * weaponPivot.rotation;

     
        gripToPivot = weaponPivot.position - gunGrip.position;
        weaponPivot.position = rightHandSocket.position + gripToPivot;
    }
}
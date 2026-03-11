using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterStealth : MonoBehaviour, ICharacterComponent
{
    public Character ParentCharacter { get; set; }

    private Animator _animator;
    private static readonly int IsStealthHash = Animator.StringToHash("isStealth");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
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
        ParentCharacter.IsStealth = value;
        _animator.SetBool(IsStealthHash, value);
    }
}

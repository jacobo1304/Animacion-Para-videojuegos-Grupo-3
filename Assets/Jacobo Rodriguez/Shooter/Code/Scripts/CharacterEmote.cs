using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterEmote : MonoBehaviour,ICharacterComponent
{

    public Character ParentCharacter { get; set; }



    [SerializeField] private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        ParentCharacter.IsEmoting = false;
    }


    public void OnEmote(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed || ParentCharacter.IsAiming || ParentCharacter.IsEmoting) return;
  
        ParentCharacter.IsEmoting = true;
        animator.SetTrigger("Emote");

    }

    public void EndEmote()
    {
        ParentCharacter.IsEmoting = false;
        Debug.Log("Emote ended");
    }
}

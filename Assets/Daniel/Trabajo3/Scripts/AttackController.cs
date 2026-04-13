using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackController : MonoBehaviour
{
   private Animator animator;
   private AttackHitBoxController hitBoxController;
   [SerializeField] private float lightCost = 15f;
   [SerializeField] private float heavyCost = 35f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        hitBoxController = GetComponent<AttackHitBoxController>();
    }

    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(Game.Instance.PlayerOne.DepleteStamina(lightCost))
            {
                animator.SetTrigger("Attack");
            }
        }
        
    }
    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if(Game.Instance.PlayerOne.DepleteStamina(heavyCost))
            {
                animator.SetTrigger("HeavyAttack");
            }
        }
    }


}


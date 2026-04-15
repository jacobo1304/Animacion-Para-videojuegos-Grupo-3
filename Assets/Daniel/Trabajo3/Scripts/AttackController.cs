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

    public void DepleteStamina(float amount)
    {
        Game.Instance.PlayerOne.DepleteStamina(amount);
    }

    public void DepleteStaminaWithParameter(string parameter)
    {
        float motionValue = GetComponent<Animator>().GetFloat(parameter);
        DepleteStamina(motionValue);
        
    }

    public void toggleAttackHitBox(int hitBoxId)
    {
        hitBoxController.ToggleHitBox(hitBoxId);
    }

    public void CleanUpAttackHitBoxes()
    {
        hitBoxController.CleanupHitBoxes();
    }


}


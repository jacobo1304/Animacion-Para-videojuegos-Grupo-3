using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackController : MonoBehaviour
{
   private Animator animator;
   private AttackHitBoxController hitBoxController;
   [SerializeField] private float lightCost = 15f;
   [SerializeField] private float heavyCost = 35f;
    [SerializeField] private MovementCharacter movementCharacter;
    [SerializeField] private float minAttackFacingInput = 0.1f;
    [SerializeField] private float attackRotationSpeed = 12f;
    [SerializeField] private string canMoveParam = "CanMove";
    [SerializeField] private string canAttackParam = "CanAttack";
    [SerializeField] private string lightAttackTrigger = "Attack";
    [SerializeField] private float lightAttackBufferTime = 0.2f;
    [SerializeField] private int maxBufferedLightAttacks = 2;

    private bool isRotatingToAttack;
    private Quaternion targetAttackRotation;
    private int bufferedLightAttackCount;
    private float bufferedLightAttackAt;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        hitBoxController = GetComponent<AttackHitBoxController>();
        if (movementCharacter == null)
        {
            movementCharacter = GetComponent<MovementCharacter>();
        }
    }

    private void Update()
    {
        TryConsumeLightAttackBuffer();
        if (!isRotatingToAttack)
        {
            return;
        }

        float step = attackRotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetAttackRotation, step);
        if (Quaternion.Angle(transform.rotation, targetAttackRotation) <= 0.5f)
        {
            transform.rotation = targetAttackRotation;
            isRotatingToAttack = false;
        }
    }

    public void OnLightAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            QueueOrExecuteLightAttack();
        }
        
    }
    public void OnHeavyAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Game.Instance.PlayerOne.DepleteStamina(heavyCost))
            {
                FaceAttackDirection();
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

    private void FaceAttackDirection()
    {
        if (movementCharacter == null)
        {
            return;
        }

        Vector3 dir = movementCharacter.LastFacingDirection;
        dir.y = 0f;
        if (dir.sqrMagnitude < minAttackFacingInput * minAttackFacingInput)
        {
            return;
        }

        targetAttackRotation = Quaternion.LookRotation(dir, Vector3.up);
        isRotatingToAttack = true;
    }

    private void QueueOrExecuteLightAttack()
    {
        if (CanMoveNow())
        {
            ExecuteLightAttack();
            return;
        }

        if (!CanBufferAttackNow())
        {
            return;
        }

        if (bufferedLightAttackCount < maxBufferedLightAttacks)
        {
            bufferedLightAttackCount++;
        }
        bufferedLightAttackAt = Time.time;
    }

    private void TryConsumeLightAttackBuffer()
    {
        if (bufferedLightAttackCount <= 0)
        {
            return;
        }

        if (Time.time - bufferedLightAttackAt > lightAttackBufferTime)
        {
            bufferedLightAttackCount = 0;
            return;
        }

        if (!CanAttackNow())
        {
            return;
        }

        ExecuteLightAttack();
        bufferedLightAttackCount = Mathf.Max(0, bufferedLightAttackCount - 1);
    }

    private bool CanAttackNow()
    {
        if (animator == null)
        {
            return false;
        }

        return animator.GetBool(canMoveParam) || animator.GetBool(canAttackParam);
    }

    private bool CanMoveNow()
    {
        if (animator == null)
        {
            return false;
        }

        return animator.GetBool(canMoveParam);
    }

    private bool CanBufferAttackNow()
    {
        if (animator == null)
        {
            return false;
        }

        return animator.GetBool(canAttackParam);
    }

    private void ExecuteLightAttack()
    {
        if (!Game.Instance.PlayerOne.DepleteStamina(lightCost))
        {
            return;
        }

        FaceAttackDirection();
        animator.SetTrigger(lightAttackTrigger);
    }


}


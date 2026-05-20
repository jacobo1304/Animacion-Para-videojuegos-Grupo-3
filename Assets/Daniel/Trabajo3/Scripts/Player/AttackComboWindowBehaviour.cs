using UnityEngine;

public class AttackComboWindowBehaviour : StateMachineBehaviour
{
    [SerializeField] private string canAttackParam = "CanAttack";
    [SerializeField] private float windowStartNormalizedTime = 0.6f;
    [SerializeField] private float windowEndNormalizedTime = 0.9f;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(canAttackParam, false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float normalized = stateInfo.normalizedTime % 1f;
        bool inWindow = normalized >= windowStartNormalizedTime && normalized <= windowEndNormalizedTime;
        animator.SetBool(canAttackParam, inWindow);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(canAttackParam, false);
    }
}

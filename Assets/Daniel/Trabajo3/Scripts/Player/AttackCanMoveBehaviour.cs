using UnityEngine;

public class AttackCanMoveBehaviour : StateMachineBehaviour
{
    [SerializeField] private string canMoveParam = "CanMove";

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(canMoveParam, false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(canMoveParam, true);
    }
}

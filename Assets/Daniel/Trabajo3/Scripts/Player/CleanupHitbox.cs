using UnityEngine;
using UnityEngine.Animations;

public class CleanupHitbox : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       animator.gameObject.SendMessage(methodName: "CleanUpAttackHitBoxes");
    }

}
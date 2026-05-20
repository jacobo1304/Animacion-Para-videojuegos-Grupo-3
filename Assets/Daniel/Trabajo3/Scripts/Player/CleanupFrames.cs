using UnityEngine;

public class CleanupFrames : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.SendMessage(methodName:"IFrameEnd");
    }
}

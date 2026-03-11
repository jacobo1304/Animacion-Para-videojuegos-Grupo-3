using UnityEngine;
using System.Linq;

public class AimBasedOnBone : StateMachineBehaviour
{
    [SerializeField] private string aimTargetName;
    [SerializeField, Range(50, 90)] private float rotationThreshold;

    private Transform aimTarget;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        aimTarget = animator.GetComponentsInChildren<Transform>()
            .FirstOrDefault(t => t.name == aimTargetName);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (aimTarget == null) return;

        Transform characterTransform = animator.transform;

        Vector3 aimForward = Vector3.ProjectOnPlane(aimTarget.forward, characterTransform.up).normalized;

        Vector3 characterForward = characterTransform.forward;

        float angle = Vector3.Angle(characterForward, aimForward);


        if(Mathf.Abs(angle) > rotationThreshold)
        {
        animator.SetTrigger(angle > 0 ? "TurnRight" : "TurnLeft");
        }
    }
}

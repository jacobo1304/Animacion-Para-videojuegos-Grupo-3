using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKFromParams : MonoBehaviour
{
    [Header("Targets")]

    public Transform leftHandTarget;
    public Transform leftElBowHint;
    public Transform rightHandTarget;
    public Transform rightElBowHint;
    public Transform lookTarget;

    Animator animator;

    [Header("Pesos manuales")]
    [Range(0, 1)] public float RHPos = 1, RHRot = 1,RHHint =1, LH =1, Look = 0.8f;

    public bool readFromAnimator = true;
    public string pLook = "look_IK";
    private string PRHPos = "RH_IK";
    private string PRHRot = "RH_Rot";
    private string PRHHint = "RH_Hint";

    

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    private void OnAnimatorIK(int layerIndex)
    {
        if (!animator) return;

        float wLook = readFromAnimator ? animator.GetFloat(pLook) : Look;
        float wRHPos = readFromAnimator ? animator.GetFloat(PRHPos) : RHPos;
        float wRHRot = readFromAnimator ? animator.GetFloat(PRHRot) : RHRot;
        float wRHHint = readFromAnimator ? animator.GetFloat(PRHHint) : RHHint;
        float wLH = readFromAnimator ? animator.GetFloat("LH_IK") : LH;

        if (lookTarget)
        {
            animator.SetLookAtWeight(wLook);
            animator.SetLookAtPosition(lookTarget.position);
        }
        else
        {
            animator.SetLookAtWeight(0);
        }

        if (rightHandTarget) {
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, wRHPos);
        animator.SetIKRotationWeight(AvatarIKGoal.RightHand, wRHRot);
        animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
        animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
        }
        else
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
        }

        if (rightElBowHint)
        {
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, wRHHint);
            animator.SetIKHintPosition(AvatarIKHint.RightElbow, rightElBowHint.position);
        }
        else
        {
            animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0);
        }
                 
        

    }

}

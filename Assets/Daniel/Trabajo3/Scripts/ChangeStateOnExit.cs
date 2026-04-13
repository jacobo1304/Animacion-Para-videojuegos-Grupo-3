using UnityEngine;
using UnityEngine.Animations;  

public class ChangeStateOnExit : StateMachineBehaviour
{

    [SerializeField] private string inputStateName;
    [SerializeField] private string outputStateName;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,int layerIndex)
    {
       animator.SetFloat(outputStateName, animator.GetFloat(inputStateName));
    }
}

using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SimpleIK : MonoBehaviour
{
    private Animator animator;
    private float weight;
    public bool enableIK = true;
    public Transform leftHandTarget;
    public Transform lookatTarget;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Pulse(float time = 0.2f)
    {
        StopAllCoroutines();
        StartCoroutine(routine:PulseCR(time));
    }

    private IEnumerator PulseCR(float time)
    {
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            weight = 1f - Mathf.Clamp01(t / time);
            yield return null;
        }
        weight = 0f;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if(!enableIK || !animator) return;

        if (lookatTarget)
        {
            animator.SetLookAtWeight(weight);
            animator.SetLookAtPosition(lookatTarget.position);
        }
        
        if (leftHandTarget)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand,weight);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, weight);

            animator.SetIKPosition(AvatarIKGoal.RightHand, leftHandTarget.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, leftHandTarget.rotation);
        }
    }
    


}

using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class HitTargetFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0.85f, 0f);
    [SerializeField] private SimpleIK ik;

    public void Moveto(Vector3 worldHitPoint, Transform lookAt = null, float pulse = 0.18f)
    {
        if(!target) return;
        target.position = worldHitPoint + offset;
        if (ik)
        {
         if(lookAt) ik.lookatTarget = lookAt;
         ik.Pulse(pulse);   
        }
    }
    
}
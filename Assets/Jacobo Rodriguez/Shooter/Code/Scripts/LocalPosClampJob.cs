using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

[BurstCompile]
public struct LocalPosClampJob : IWeightedAnimationJob
{
  public FloatProperty jobWeight { get; set; }


    public ReadWriteTransformHandle driven;

    public float3 minLocal;
    public float3 maxLocal;


    void IAnimationJob.ProcessRootMotion(AnimationStream stream)
    {
       
    }

    void IAnimationJob.ProcessAnimation(AnimationStream stream)
    {
        float w = jobWeight.Get(stream);

        if (w <= 0f) return;

        float3 p = driven.GetLocalPosition(stream);

        float3 clamped = math.clamp(p, minLocal, maxLocal);

        float3 res = math.lerp(p, clamped, w);

        driven.SetLocalPosition(stream, res);

    }

   
}

[System.Serializable]
public struct LocalPosClampData : IAnimationJobData
{
   [SyncSceneToStream] public Transform constrainedObject;

    public Vector3 minLocal;
    public Vector3 maxLocal;

    public bool IsValid() => constrainedObject != null && 
            minLocal.x <= maxLocal.x &&
            minLocal.y <= maxLocal.y &&
            minLocal.z <= maxLocal.z;

    
    public void SetDefaultValues()
    {
        constrainedObject = null;
        minLocal = new Vector3(-0.05f, -0.05f, -0.05f);
        maxLocal = new Vector3(0.05f, 0.05f, 0.05f);
    }

}

public class LocalPosClampBinder : AnimationJobBinder<LocalPosClampJob, LocalPosClampData>
{
    public override LocalPosClampJob Create(Animator animator, ref LocalPosClampData data, Component component)
    {
       var job = new LocalPosClampJob
       {
           driven = ReadWriteTransformHandle.Bind(animator, data.constrainedObject),
           minLocal = data.minLocal,
           maxLocal = data.maxLocal,
           jobWeight = FloatProperty.Bind(animator, component, "m_Weight")
       };
        return job;
    }

    public override void Destroy(LocalPosClampJob job)
    {
        throw new System.NotImplementedException();
    }
}


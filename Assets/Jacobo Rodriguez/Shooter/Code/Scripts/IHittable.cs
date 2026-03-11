using UnityEngine;

public interface IHittable
{

    void ApplyHit(HitInfo info);

}

public struct HitInfo
{
    public Vector3 point;
    public Vector3 normal;
    public float damage;
}

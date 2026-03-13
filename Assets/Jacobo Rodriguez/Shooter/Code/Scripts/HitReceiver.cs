using UnityEngine;

public class HitReceiver : MonoBehaviour, IHittable
{
    [SerializeField] private Animator _animator;
    [SerializeField] private string hitTriggerName = "Hit";

    public void ApplyHit(HitInfo info)
    {
        if(_animator) _animator.SetTrigger(hitTriggerName);
        Debug.Log($"Hit received at {info.point} with damage {info.damage}");
     
    }


    private void Reset()
    {
        _animator = GetComponent<Animator>();
    }
}



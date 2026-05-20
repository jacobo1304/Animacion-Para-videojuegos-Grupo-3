using UnityEngine;

public class AttackCameraZoomBehaviour : StateMachineBehaviour
{
    [SerializeField] private float zoomFov = 45f;
    [SerializeField] private float zoomInTime = 0.08f;
    [SerializeField] private float zoomHoldTime = 0.06f;
    [SerializeField] private float zoomOutTime = 0.12f;
    [SerializeField] private bool usePulse = true;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AttackCameraZoomController zoom = animator.GetComponentInChildren<AttackCameraZoomController>();
        if (zoom == null)
        {
            zoom = animator.GetComponent<AttackCameraZoomController>();
        }

        if (zoom != null)
        {
            if (usePulse)
            {
                zoom.StartZoomPulse(zoomFov, zoomInTime, zoomHoldTime, zoomOutTime);
            }
            else
            {
                zoom.StartZoom(zoomFov, zoomInTime);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        AttackCameraZoomController zoom = animator.GetComponentInChildren<AttackCameraZoomController>();
        if (zoom == null)
        {
            zoom = animator.GetComponent<AttackCameraZoomController>();
        }

        if (zoom != null)
        {
            zoom.ResetZoom(zoomOutTime);
        }
    }
}

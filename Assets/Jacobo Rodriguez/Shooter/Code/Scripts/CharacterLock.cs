using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class CharacterLock : MonoBehaviour, ICharacterComponent
{

    [SerializeField] private Camera camera;
    [SerializeField] private LayerMask detectionMask;
    [SerializeField] private float detectionRadius;
    [SerializeField] private float detectionAngle;

    [SerializeField] private TextMeshProUGUI lockText;



    private bool autoLockEnabled;

    public Character ParentCharacter { get; set; }

  
    public void OnLock(InputAction.CallbackContext ctx)
    {
        if (!ctx.started) return;

        if (ParentCharacter.LockTarget != null)
        {
            ParentCharacter.LockTarget = null;
            return;

        }

        ParentCharacter.LockTarget = FindBestTarget();

    }

    public void ToggleAutoLock()
    {
        autoLockEnabled = !autoLockEnabled;

        lockText.text = autoLockEnabled ? "Desactivar Auto Lock" : "Activar Auto Lock";

        ParentCharacter.LockTarget = null;
    }


    private void Update()
    {
        if (!autoLockEnabled) return;

        if (ParentCharacter.LockTarget == null)
        {
            ParentCharacter.LockTarget = FindBestTarget();
        }
        else
        {
            ValidateTarget();
        }
    }

    void ValidateTarget()
    {
        Transform target = ParentCharacter.LockTarget;
        if (target == null)
        {
            ParentCharacter.LockTarget = null;
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        Vector3 dir = (target.position - camera.transform.position).normalized;
        float angle = Vector3.Angle(camera.transform.forward, dir);

        if (distance > detectionRadius || angle > detectionAngle)
        {
            ParentCharacter.LockTarget = null;
        }
    }

    Transform FindBestTarget()
    {
        Collider[] detectedObjects =
            Physics.OverlapSphere(transform.position, detectionRadius, detectionMask);

        if (detectedObjects.Length == 0) return null;

        float nearestDistance = detectionRadius;
        Transform closest = null;

        Vector3 cameraForward = camera.transform.forward;

        foreach (Collider obj in detectedObjects)
        {
            Vector3 dir = obj.transform.position - camera.transform.position;
            float angle = Vector3.Angle(cameraForward, dir);

            if (angle > detectionAngle) continue;

            float distance = Vector3.Distance(obj.transform.position, transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                closest = obj.transform;
            }
        }

        return closest;
    }


#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
     Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

#endif
}

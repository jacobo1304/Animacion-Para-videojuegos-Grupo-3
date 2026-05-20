using UnityEngine;

public class EnemyAttackHitBoxBridge : MonoBehaviour
{
    [SerializeField] private AttackHitBoxController hitBoxController;

    private void Awake()
    {
        if (hitBoxController == null)
        {
            hitBoxController = GetComponent<AttackHitBoxController>();
        }
    }

    public void toggleAttackHitBox(int hitBoxId)
    {
        if (hitBoxController != null)
        {
            hitBoxController.ToggleHitBox(hitBoxId);
        }
    }

    public void CleanUpAttackHitBoxes()
    {
        if (hitBoxController != null)
        {
            hitBoxController.CleanupHitBoxes();
        }
    }
}

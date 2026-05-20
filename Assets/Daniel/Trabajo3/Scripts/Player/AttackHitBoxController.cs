using UnityEngine;

public class AttackHitBoxController : MonoBehaviour
{
    [SerializeField] private GameObject [] hitboxes;

    public void ToggleHitBox(int attackId)
    {
        if (hitboxes == null || hitboxes.Length == 0) return;
        if (attackId < 0 || attackId >= hitboxes.Length) return;

        GameObject hitBox = hitboxes[attackId];
        if (hitBox != null)
        {
            hitBox.SetActive(!hitBox.activeSelf);
        }
    }

    public void CleanupHitBoxes()
    {
        foreach (GameObject colliders in hitboxes)
        {
            colliders.SetActive(false);
        }
    }
  
}
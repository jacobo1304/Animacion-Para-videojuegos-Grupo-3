using UnityEngine;

public class AttackHitBoxController : MonoBehaviour
{
    [SerializeField] private GameObject [] hitboxes;

    public void ToggleHitBox(int attackId)
    {
        for(int hitBoxId = 0; hitBoxId < hitboxes.Length; hitBoxId++)
        {
            GameObject hitBox = this.hitboxes[hitBoxId];
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
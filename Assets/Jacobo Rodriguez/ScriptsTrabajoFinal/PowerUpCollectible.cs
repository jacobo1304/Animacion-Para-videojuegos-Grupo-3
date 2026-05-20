using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerUpCollectible : MonoBehaviour
{
    [SerializeField] private PowerUpType powerUpType = PowerUpType.Offensive;
    [SerializeField] private float duration = 6f;
    [SerializeField] private GameObject effectObject;
    [SerializeField] private bool destroyOnPickup = true;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PowerUpManager manager = other.GetComponentInParent<PowerUpManager>();
        if (manager == null)
        {
            return;
        }

        manager.ApplyPowerUp(powerUpType, duration, effectObject);

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}

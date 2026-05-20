using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerUpCollectible : MonoBehaviour
{
    [SerializeField] private PowerUpType powerUpType = PowerUpType.Offensive;
    [SerializeField] private float duration = 6f;
    [SerializeField] private GameObject effectObject;
    [SerializeField] private bool destroyOnPickup = true;
    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioSource audioSource;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PowerUpManager manager = other.GetComponentInParent<PowerUpManager>();
        if (manager == null)
        {
            return;
        }

        if (pickupSound != null)
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            if (audioSource != null)
            {
                audioSource.PlayOneShot(pickupSound);
            }
            else
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }
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

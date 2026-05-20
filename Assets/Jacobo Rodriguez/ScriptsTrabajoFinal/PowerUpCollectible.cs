using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PowerUpCollectible : MonoBehaviour
{
    [SerializeField] private PowerUpType powerUpType = PowerUpType.Offensive;
    [SerializeField] private float duration = 6f;
    [SerializeField] private GameObject effectObject;
    [SerializeField] private bool destroyOnPickup = true;
    [Header("Effect Auto Find")]
    [SerializeField] private bool autoFindEffectOnSpawn = true;
    [SerializeField] private string defensiveEffectName = "Shield FX_ FREE_4";
    [SerializeField] private string offensiveEffectName = "BerserkerParticles";
    [SerializeField] private string defensiveLayerName = "powerup1";
    [SerializeField] private string offensiveLayerName = "powerup2";
    [Header("Audio")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        TryResolveEffectObject();
    }

    private void OnEnable()
    {
        TryResolveEffectObject();
    }

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

        TryResolveEffectObject();

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

    private void TryResolveEffectObject()
    {
        if (!autoFindEffectOnSpawn)
        {
            return;
        }

        string layerName = powerUpType == PowerUpType.Defensive ? defensiveLayerName : offensiveLayerName;
        string effectName = powerUpType == PowerUpType.Defensive ? defensiveEffectName : offensiveEffectName;
        GameObject resolved = FindEffectByLayerAndName(layerName, effectName);
        if (resolved != null)
        {
            effectObject = resolved;
        }
    }

    private GameObject FindEffectByLayerAndName(string layerName, string nameContains)
    {
        if (string.IsNullOrEmpty(layerName) || string.IsNullOrEmpty(nameContains))
        {
            return null;
        }

        int layer = LayerMask.NameToLayer(layerName);
        if (layer < 0)
        {
            return null;
        }

        Transform[] allTransforms = FindObjectsOfType<Transform>(includeInactive: true);
        for (int i = 0; i < allTransforms.Length; i++)
        {
            Transform t = allTransforms[i];
            if (t.gameObject.layer == layer && t.name.Contains(nameContains))
            {
                return t.gameObject;
            }
        }

        return null;
    }
}

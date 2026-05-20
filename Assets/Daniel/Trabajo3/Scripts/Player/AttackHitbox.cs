using UnityEngine;
using Unity.VisualScripting;

public class AttackHitbox : MonoBehaviour, IDamageSender<DamageMessage>
{
    [SerializeField] private DamageMessage damageMessage;
    [SerializeField] private GameObject senderOverride;
    [SerializeField] private PowerUpManager powerUpManager;

    private void Awake()
    {
        if (senderOverride == null)
        {
            senderOverride = transform.root.gameObject;
        }

        if (powerUpManager == null)
        {
            powerUpManager = GetComponentInParent<PowerUpManager>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageReceiver<DamageMessage> receiver))
        {
            SendDamage(receiver);
        }
    }

    public void SendDamage(IDamageReceiver<DamageMessage> receiver)
    {
        DamageMessage message = damageMessage;
        message.sender = senderOverride != null ? senderOverride : transform.root.gameObject;
        float damageMultiplier = powerUpManager != null ? powerUpManager.GetDamageMultiplier() : 1f;
        message.amount *= damageMultiplier;
        receiver.ReceiveDamage(message);
        float extra = 0.02f * (int)message.damageLevel;
        GetComponent<HitStopper>()?.HitStop(duration:0.02f+extra);
    }

}
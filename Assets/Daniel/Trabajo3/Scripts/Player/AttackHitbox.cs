using UnityEngine;
using Unity.VisualScripting;

public class AttackHitbox : MonoBehaviour, IDamageSender<DamageMessage>
{
    [SerializeField] private DamageMessage damageMessage;
    [SerializeField] private GameObject senderOverride;
    private PowerUpManager powerUpManager;

    private void Awake()
    {
        if (senderOverride == null)
        {
            senderOverride = transform.root.gameObject;
        }
        powerUpManager = GetComponentInParent<PowerUpManager>();
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
        DamageMessage msg = damageMessage;
        msg.sender = senderOverride != null ? senderOverride : transform.root.gameObject;
        float damageMultiplier = powerUpManager != null ? powerUpManager.GetDamageMultiplier() : 1f;
        msg.amount *= Mathf.Max(0f, damageMultiplier);
        receiver.ReceiveDamage(msg);
        float extra = 0.02f * (int)msg.damageLevel;
        GetComponent<HitStopper>()?.HitStop(duration:0.02f+extra);
    }

}
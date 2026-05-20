using UnityEngine;
using Unity.VisualScripting;

public class AttackHitbox : MonoBehaviour, IDamageSender<DamageMessage>
{
    [SerializeField] private DamageMessage damageMessage;
    [SerializeField] private GameObject senderOverride;

    private void Awake()
    {
        if (senderOverride == null)
        {
            senderOverride = transform.root.gameObject;
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
        damageMessage.sender = senderOverride != null ? senderOverride : transform.root.gameObject;
        receiver.ReceiveDamage(damageMessage);
        float extra = 0.02f*(int)damageMessage.damageLevel;
        GetComponent<HitStopper>()?.HitStop(duration:0.02f+extra);
    }

}
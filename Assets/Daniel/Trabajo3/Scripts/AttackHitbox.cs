using UnityEngine;
using Unity.VisualScripting;

public class AttackHitbox : MonoBehaviour, IDamageSender<DamageMessage>
{
    [SerializeField] private DamageMessage damageMessage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IDamageReceiver<DamageMessage> receiver))
        {
            SendDamage(receiver);
        }
    }

    public void SendDamage(IDamageReceiver<DamageMessage> receiver)
    {
        receiver.ReceiveDamage(damageMessage);
        float extra = 0.02f*(int)damageMessage.damageLevel;
        GetComponent<HitStopper>()?.HitStop(duration:0.02f+extra);
    }

}
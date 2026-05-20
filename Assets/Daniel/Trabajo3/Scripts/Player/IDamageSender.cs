using UnityEngine;

public interface IDamageSender<TDamage> where TDamage : struct
{
    void SendDamage(IDamageReceiver<TDamage> damage);
}
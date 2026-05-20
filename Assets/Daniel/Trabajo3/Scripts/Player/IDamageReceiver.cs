using UnityEngine;

public interface IDamageReceiver<TDamage> where TDamage : struct
{
    void ReceiveDamage(TDamage damage);
  
}
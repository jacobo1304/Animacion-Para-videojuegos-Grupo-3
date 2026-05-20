using System;
using UnityEngine;
using UnityEngine.Events;

namespace Classes.Clase_7.Scripts
{
    public class DamageHitBox : MonoBehaviour, IDamageReceiver<DamageMessage>
    {
        [Serializable]
        public class AttackQueueEvent : UnityEvent<DamageMessage>
        {
        }

        [SerializeField] private float defenseMultiplier;
        public AttackQueueEvent OnHit;

        public void ReceiveDamage(DamageMessage damage)
        {
            if (damage.sender == transform.root.gameObject) return;
            damage.amount = damage.amount * defenseMultiplier;
            OnHit?.Invoke(damage);
        }
    }
}

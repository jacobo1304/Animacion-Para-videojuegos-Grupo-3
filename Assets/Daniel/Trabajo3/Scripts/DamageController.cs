using System.Collections.Generic;
using System.Linq;
using UnityEngine;


    public class DamageController : MonoBehaviour
    {
        [SerializeField] private bool ignoreDamage;
        private List<DamageMessage> damageList = new List<DamageMessage>();

        private Animator animator;
        private Vector3 lastHitPoint;
        private Vector3 lastImpulse;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public void EnqueueDamage(DamageMessage damage)
        {
            if (ignoreDamage || damageList.Any(dmg => dmg.sender == damage.sender)) return;
            var dirFromAttacker = (transform.position - damage.sender.transform.position).normalized;
            lastHitPoint = transform.position + dirFromAttacker * 0.3f;
            damageList.Add(damage);
        }
    }

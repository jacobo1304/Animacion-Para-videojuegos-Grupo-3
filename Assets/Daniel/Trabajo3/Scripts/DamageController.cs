using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;



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

        private void Update()
        {
            Vector3 damageDirection = Vector3.zero;
            int damageLevel = 0;
            bool isDead = false;
            foreach(DamageMessage message in damageList)
            {
                Game.Instance.PlayerOne.DepleteHealth(message.amount, out isDead);
                damageDirection += (message.sender.transform.position - transform.position).normalized;
                damageLevel = Mathf.Max(damageLevel, (int)message.damageLevel);
            }

            if(damageList.Count == 0 ) return;
            damageDirection = Vector3.ProjectOnPlane(vector:damageDirection.normalized, planeNormal:transform.up);
            float damageAngle = Vector3.SignedAngle(transform.forward, damageDirection, axis:transform.up);

            animator.SetFloat("DamageDirection",(damageAngle/180)*0.5f+0.5f);
            animator.SetInteger(name:"DamageLevel",damageLevel);
            animator.SetTrigger(name: "Damage");

            if(isDead)
            {
                animator.ResetTrigger( name:"Damage");
                animator.SetTrigger(name: "Die");
            }
            
            var attacker = damageList[0].sender.transform;
            damageList.Clear();

        
        }

        public void IFrameStart()
        {
            ignoreDamage = true;
            Debug.Log("DamageController: IFrameStart -> ignoreDamage=true", this);
        }

        public void IFrameEnd()
        {
            ignoreDamage = false;
            Debug.Log("DamageController: IFrameEnd -> ignoreDamage=false", this);
        }
    }
    

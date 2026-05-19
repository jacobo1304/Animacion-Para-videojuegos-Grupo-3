using System;
using Clases.Clase_8.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

namespace Clases.Clase_8.Scripts.States
{
    public class AttackState : State
    {
        private readonly ComboSequence _combo;
        private NavMeshAgent _agent;
        private ComboExecutor _executor;
        private float _cooldownTimer;


        private float _reattackCooldown = 1.0f;

        public AttackState(EnemyAI enemy, ComboSequence combo) : base(enemy)
        {
            _combo = combo;
        }

        public override void Enter()
        {
            _agent = enemy.agent;
            _agent.isStopped = true;
            _agent.ResetPath();

            _executor = enemy.GetComponentInChildren<ComboExecutor>(includeInactive: true);
            if (_executor == null)
            {
                var anim = enemy.GetComponentInChildren<Animator>(includeInactive: true);
                if (anim == null)
                {
                    enemy.ChangeState(new IdleState(enemy));
                    return;
                }

                _executor = anim.gameObject.AddComponent<ComboExecutor>();
                _executor.animator = anim;
            }

            _cooldownTimer = 0f;
            _executor.PlayCombo(_combo);
        }

        public override void Update()
        {
            if (enemy.player == null)
            {
                enemy.ChangeState(new IdleState(enemy));
                return;
            }

            //Rotacion enemy to player when attacking
            Vector3 toPlayer = enemy.player.position - enemy.transform.position;
            toPlayer.y = 0f;

            if(toPlayer.sqrMagnitude > 0.001f)
            {
                var look = Quaternion.LookRotation(toPlayer);
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, look, enemy.rotationSmooth * Time.deltaTime);
            }

            if(!enemy.PlayerInRange(3.0f))
            {
                _executor?.Cancel();
                enemy.ChangeState(new ChaseState(enemy));
                return; 
            }

            if(!_executor.IsBusy())
            {
                _cooldownTimer += Time.deltaTime;
                if(_cooldownTimer >= _reattackCooldown && enemy.PlayerInRange(2.2f))
                {
                    _cooldownTimer = 0f;
                    _executor.PlayCombo(_combo);
                }
                else if (!enemy.PlayerInRange(2.0f))
                {
                    _executor?.Cancel();
                    enemy.ChangeState(new ChaseState(enemy));
                }
            }

        }

        public override void Exit()
        {
            if(_agent != null)
            {
                _agent.isStopped = false;
            }
            _executor?.Cancel();
        }
    }
}

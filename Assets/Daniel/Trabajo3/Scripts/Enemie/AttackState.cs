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

        private const float ReattackCooldown = 3.0f;
        private const float AttackRange = 2.0f;
        private const float ChaseRange = 2.6f;

        public AttackState(EnemyAI enemy, ComboSequence combo) : base(enemy)
        {
            _combo = combo;
        }

        public override void Enter()
        {
            _agent = enemy.agent;
            _agent.isStopped = true;
            _agent.ResetPath();
            enemy.SetAttacking(true);

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
            if (!enemy.CanAttack())
            {
                enemy.ChangeState(new ChaseState(enemy));
                return;
            }

            enemy.MarkAttack();
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

            float dist = Vector3.Distance(enemy.transform.position, enemy.player.position);
            if (dist > ChaseRange)
            {
                _executor?.Cancel();
                enemy.ChangeState(new ChaseState(enemy));
                return;
            }

            if (!_executor.IsBusy())
            {
                _cooldownTimer += Time.deltaTime;
                bool canAttackAgain = _cooldownTimer >= ReattackCooldown && enemy.CanAttack();

                if (dist <= AttackRange && canAttackAgain)
                {
                    _cooldownTimer = 0f;
                    enemy.MarkAttack();
                    _executor.PlayCombo(_combo);
                }
                else if (dist > AttackRange)
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
            enemy.SetAttacking(false);
        }
    }
}

using UnityEngine;
using Clases.Clase_8.Scripts.States;

public class ChaseState : State
{

        private const float ATTACK_RANGE = 2f;
        private const float LOSE_TARGET = 6.0f;
        private float _repathTimer;
        private const float REPATH_EVERY =0.15f;

        public ChaseState(EnemyAI enemy) : base(enemy)
        {
        }

        public override void Enter()
        {
        enemy.agent.isStopped = false;
        enemy.agent.speed = enemy.runSpeed;
        _repathTimer = 0f;
        }

        public override void Update()
        {
        if (enemy.player == null)
        {
            enemy.ChangeState(new IdleState(enemy));
            return;
        }
        float dist = Vector3.Distance(enemy.transform.position, enemy.player.position);
        if (dist <= ATTACK_RANGE && enemy.CanAttack())
        {
            enemy.ChangeState(new AttackState(enemy, enemy.defaultCombo));
            return;
        }

        if(dist> LOSE_TARGET)
        {
            enemy.ChangeState(new IdleState(enemy));
            return;
        }
        _repathTimer += Time.deltaTime;
        if(_repathTimer >= REPATH_EVERY || !enemy.agent.hasPath)
        {
            _repathTimer = 0f;
            enemy.agent.SetDestination(enemy.player.position);
        }
        }

        public override void Exit()
        {

        }
}

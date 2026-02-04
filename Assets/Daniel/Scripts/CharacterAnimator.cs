using UnityEngine;

namespace Clases.scripts
{
    public class CharacterAnimator
    {
        private readonly Animator _animator;
        private readonly int _speedHash = Animator.StringToHash("Speed");
        private readonly int _dieHash = Animator.StringToHash("Die");


        public CharacterAnimator(Animator animator)
        {
            _animator = animator;
        }   

        public void UpdateSpeed(float speed)
        {
            _animator.SetFloat(_speedHash, speed);
        }

        public void UpdateDie(bool isDead)
        {
            _animator.SetBool(_dieHash, isDead);
        }

        public bool GetDie()
        {
            return _animator.GetBool(_dieHash);
        }
    }

}
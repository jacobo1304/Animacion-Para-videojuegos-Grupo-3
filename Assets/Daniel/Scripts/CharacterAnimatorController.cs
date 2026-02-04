using System;
using UnityEngine;

namespace Clases.scripts
{
    public class CharacterAnimatorController : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterInputFactory.InputType _inputType = CharacterInputFactory.InputType.Player;    
        private ICharacterInput _input;
        private CharacterAnimator _characterAnimator;

        private void Awake()
        {
            _input = CharacterInputFactory.CreateInput(_inputType);
            _characterAnimator = new CharacterAnimator(animator);
        }
        private void Update()
        {
            float speed = _input.GetSpeetInput();
            _characterAnimator.UpdateSpeed(speed);

            if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
            {
                bool current = _characterAnimator.GetDie();
                _characterAnimator.UpdateDie(!current);
            }
        }
    }
}
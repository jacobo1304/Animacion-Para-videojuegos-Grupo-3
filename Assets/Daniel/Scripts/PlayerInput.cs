using UnityEngine;
using UnityEngine.Windows;


namespace Clases.scripts
{
    public class PlayerInput : ICharacterInput
    {
        public float GetSpeetInput()
        {
            return new Vector2(
                UnityEngine.Input.GetAxis("Horizontal"),
                UnityEngine.Input.GetAxis("Vertical")
            ).magnitude;
        }
        
    }
}
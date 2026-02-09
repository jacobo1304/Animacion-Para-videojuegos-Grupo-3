using UnityEngine;
using UnityEngine.Windows;

namespace Clases.scripts
{
    public class EnemyInput : ICharacterInput
    {
       public float GetSpeedInput()
        {
            return new Vector2(
                UnityEngine.Input.GetAxis("Horizontal"),
                UnityEngine.Input.GetAxis("Vertical")
            ).magnitude;
        }
        
    }
}
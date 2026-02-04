using UnityEngine;

namespace Clases.scripts
{
    public class Player2Input : ICharacterInput
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

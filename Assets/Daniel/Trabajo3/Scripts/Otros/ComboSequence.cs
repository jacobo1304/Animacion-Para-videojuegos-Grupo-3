using System;
using System.Collections.Generic;
using UnityEngine;

namespace Clases.Clase_8.Scripts
{
    [CreateAssetMenu(fileName = "Combo Sequence")]
    public class ComboSequence : ScriptableObject
    {
        [Serializable]
        public class Step
        {
            public string animatorTrigger = "Attack";
            [UnityEngine.Range(0, 1)] public float chainWindowStart = 0.4f;
            [UnityEngine.Range(0, 1)] public float chainWindowEnd = 0.85f;
            public float minDistance = 0.2f;
            public float maxDistance = 2.2f;
        }

        public List<Step> steps = new List<Step>();
        public float cooldownAfterCombo = 0.6f;
        public bool loopLastStepIfHolding = false;
    }
}

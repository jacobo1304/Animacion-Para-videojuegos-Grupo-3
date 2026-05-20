using UnityEngine;
using System;

[Serializable]
public struct DamageMessage
{
        public enum DamageLevel
        {
                Small,
                Medium,
                Big
        }

        public GameObject sender;
        public float amount;
        public DamageLevel damageLevel;
}

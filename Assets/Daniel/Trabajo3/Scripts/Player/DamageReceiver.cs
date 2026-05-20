using UnityEditor;
using UnityEngine;

public class DamageReceiver : MonoBehaviour,IDamageReceiver<float>
{
    public void ReceiveDamage(float damage)
    {
        Debug.Log("Received damage: " + damage);
    }
}
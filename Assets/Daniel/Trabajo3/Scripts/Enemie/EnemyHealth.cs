using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float startHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    public float StartHealth => startHealth;
    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0f;

    private void Awake()
    {
        if (currentHealth <= 0f)
        {
            currentHealth = startHealth;
        }
        else
        {
            currentHealth = Mathf.Clamp(currentHealth, 0f, startHealth);
        }
    }

    public void ApplyDamage(float amount, out bool died)
    {
        if (IsDead)
        {
            died = true;
            return;
        }

        currentHealth -= Mathf.Max(0f, amount);
        died = currentHealth <= 0f;
    }
}

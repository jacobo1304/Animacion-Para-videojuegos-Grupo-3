using UnityEngine;

public class EnemyStamina : MonoBehaviour
{
    [SerializeField] private float startStamina = 1000f;
    [SerializeField] private float currentStamina = 1000f;
    [SerializeField] private float regenPerSecond = 0f;

    public float CurrentStamina => currentStamina;

    private void Awake()
    {
        if (currentStamina <= 0f)
        {
            currentStamina = startStamina;
        }
        else
        {
            currentStamina = Mathf.Clamp(currentStamina, 0f, startStamina);
        }
    }

    private void Update()
    {
        if (regenPerSecond > 0f)
        {
            currentStamina = Mathf.Min(currentStamina + regenPerSecond * Time.deltaTime, startStamina);
        }
    }

    public void DepleteStamina(float amount)
    {
        currentStamina = Mathf.Max(0f, currentStamina - Mathf.Max(0f, amount));
    }
}

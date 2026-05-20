using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemy;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private bool useNormalizedValues;

    private void Awake()
    {
        if (enemy == null)
        {
            enemy = GetComponentInParent<EnemyHealth>();
        }
    }

    private void OnEnable()
    {
        SyncMaxValues();
        UpdateValues();
    }

    private void Update()
    {
        UpdateValues();
    }

    private void SyncMaxValues()
    {
        if (enemy == null || healthSlider == null)
        {
            return;
        }

        healthSlider.maxValue = useNormalizedValues ? 1f : enemy.StartHealth;
    }

    private void UpdateValues()
    {
        if (enemy == null || healthSlider == null)
        {
            return;
        }

        float value = useNormalizedValues
            ? SafeDivide(enemy.CurrentHealth, enemy.StartHealth)
            : enemy.CurrentHealth;

        healthSlider.value = value;
    }

    private static float SafeDivide(float value, float max)
    {
        if (max <= 0f)
        {
            return 0f;
        }

        return value / max;
    }
}

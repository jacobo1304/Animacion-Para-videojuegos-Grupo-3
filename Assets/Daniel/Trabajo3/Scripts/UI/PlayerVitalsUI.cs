using UnityEngine;
using UnityEngine.UI;

public class PlayerVitalsUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Image healthFill;
    [SerializeField] private bool useNormalizedValues;
    [SerializeField] private Color healthColorHigh = new Color(0.549f, 1f, 0.565f);
    [SerializeField] private Color healthColorMid = new Color(1f, 0.792f, 0.416f);
    [SerializeField] private Color healthColorLow = new Color(1f, 0.243f, 0.173f);

    private CharacterState state;

    private void Awake()
    {
        state = Game.Instance != null ? Game.Instance.PlayerOne : null;
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
        EnsureState();
        if (state == null)
        {
            return;
        }

        if (healthSlider != null)
        {
            healthSlider.maxValue = useNormalizedValues ? 1f : state.StartHealth;
        }

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = useNormalizedValues ? 1f : state.StartStamina;
        }
    }

    private void UpdateValues()
    {
        EnsureState();
        if (state == null)
        {
            return;
        }

        if (healthSlider != null)
        {
            float value = useNormalizedValues
                ? SafeDivide(state.CurrentHealth, state.StartHealth)
                : state.CurrentHealth;
            healthSlider.value = value;
            UpdateHealthFillColor();
        }

        if (staminaSlider != null)
        {
            staminaSlider.value = useNormalizedValues
                ? SafeDivide(state.CurrentStamina, state.StartStamina)
                : state.CurrentStamina;
        }
    }

    private void UpdateHealthFillColor()
    {
        if (healthFill == null)
        {
            return;
        }

        float normalized = SafeDivide(state.CurrentHealth, state.StartHealth);
        if (normalized >= 0.759f)
        {
            healthFill.color = healthColorHigh;
        }
        else if (normalized >= 0.529f)
        {
            healthFill.color = healthColorMid;
        }
        else
        {
            healthFill.color = healthColorLow;
        }
    }

    private void EnsureState()
    {
        if (state == null && Game.Instance != null)
        {
            state = Game.Instance.PlayerOne;
        }
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

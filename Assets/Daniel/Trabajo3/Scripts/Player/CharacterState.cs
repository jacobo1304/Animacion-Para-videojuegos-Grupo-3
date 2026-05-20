using UnityEngine;


public class CharacterState : MonoBehaviour
{
    //Stamina del personaje
    [SerializeField] private float _startStamina = 100f;
    [SerializeField] private float _staminaRegen= 10f;
    [SerializeField] private float _currentStamina = 100f;

   //Vida del personaje
    [SerializeField] private float _startHealth = 100f;
    [SerializeField] private float _currentHealth = 100f;

    public float CurrentStamina => _currentStamina;
    public float CurrentHealth => _currentHealth;
    public float StartStamina => _startStamina;
    public float StartHealth => _startHealth;


    private void Start()
    {
        _currentStamina = _startStamina;
        _currentHealth = _startHealth;
        
    }

    private void Update()
    {
        RegenerateStamina(_staminaRegen*Time.deltaTime);
    }
 
    private void RegenerateStamina(float staminaRegen)
    {
        _currentStamina = Mathf.Min(_currentStamina + staminaRegen, _startStamina);
    }

    public bool DepleteStamina(float staminaDepletion)
    {
        if (CurrentStamina < staminaDepletion)
        {
            return false;
        }

        _currentStamina = Mathf.Max(0f, CurrentStamina - staminaDepletion);
        return true;
    }
    public void DepleteHealth(float healthDepletion, out bool zeroHealth)
    {
        _currentHealth = Mathf.Max(0f, _currentHealth - healthDepletion);
        zeroHealth = false;
        if (_currentHealth <= 0)
        {
            zeroHealth = true;
        }
       
    }




}
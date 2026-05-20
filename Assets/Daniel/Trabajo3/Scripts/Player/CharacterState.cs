using UnityEngine;


public class CharacterState : MonoBehaviour
{
    //Stamina del personaje
   [SerializeField] private float _startStamina = 100000000000f;
   [SerializeField] private float _staminaRegen= 0f;
   [SerializeField] private float _currentStamina = 100f;

   //Vida del personaje
    [SerializeField] private float _startHealth = 100f;
    [SerializeField] private float _currentHealth = 100f;

    public float CurrentStamina => _currentStamina;


    private void Start()
    {
        _currentStamina = _startStamina;
        
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

        _currentStamina = CurrentStamina - staminaDepletion;
        return true;
    }
    public void DepleteHealth(float healthDepletion, out bool zeroHealth)
    {
        _currentHealth -= healthDepletion;
        zeroHealth = false;
        if (_currentHealth <= 0)
        {
            zeroHealth = true;
        }
       
    }




}
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [Header("Character Slots (max 3)")]
    [SerializeField] private GameObject[] characterSlots = new GameObject[3];

    [SerializeField] private int initialCharacterIndex;

    [Header("Character Codes (aligned to slots)")]
    [SerializeField] private string[] characterCodes = new string[3];

    public int CurrentCharacterIndex { get; private set; } = -1;
    public string CurrentCharacterCode { get; private set; } = string.Empty;

    private void Awake()
    {
        ClampSlotsToThree();
        SelectCharacter(initialCharacterIndex);
    }

    public void SelectCharacter0() => SelectCharacter(0);
    public void SelectCharacter1() => SelectCharacter(1);
    public void SelectCharacter2() => SelectCharacter(2);

    public void SelectCharacter(int index)
    {
        if (index < 0 || index >= characterSlots.Length)
            return;

        if (CurrentCharacterIndex == index)
            return;

        GameObject nextCharacter = characterSlots[index];
        if (nextCharacter == null)
            return;

        if (CurrentCharacterIndex >= 0 && CurrentCharacterIndex < characterSlots.Length)
        {
            GameObject currentCharacter = characterSlots[CurrentCharacterIndex];
            if (currentCharacter != null)
                currentCharacter.SetActive(false);
        }

        nextCharacter.SetActive(true);
        CurrentCharacterIndex = index;
        // Update current character code from the aligned codes array
        if (characterCodes != null && index >= 0 && index < characterCodes.Length)
            CurrentCharacterCode = characterCodes[index] ?? string.Empty;
        else
            CurrentCharacterCode = string.Empty;
    }

    private void OnValidate()
    {
        ClampSlotsToThree();
        if (initialCharacterIndex < 0)
            initialCharacterIndex = 0;
        if (initialCharacterIndex > 2)
            initialCharacterIndex = 2;
    }

    private void ClampSlotsToThree()
    {
        if (characterSlots == null)
        {
            characterSlots = new GameObject[3];
            return;
        }

        if (characterSlots.Length > 3)
        {
            var trimmed = new GameObject[3];
            for (int i = 0; i < 3; i++)
                trimmed[i] = characterSlots[i];
            characterSlots = trimmed;
        }
        else if (characterSlots.Length < 3)
        {
            var expanded = new GameObject[3];
            for (int i = 0; i < characterSlots.Length; i++)
                expanded[i] = characterSlots[i];
            characterSlots = expanded;
        }

        // Ensure characterCodes array is also clamped/expanded to length 3
        if (characterCodes == null)
        {
            characterCodes = new string[3];
            return;
        }

        if (characterCodes.Length > 3)
        {
            var trimmedCodes = new string[3];
            for (int i = 0; i < 3; i++)
                trimmedCodes[i] = characterCodes[i];
            characterCodes = trimmedCodes;
        }
        else if (characterCodes.Length < 3)
        {
            var expandedCodes = new string[3];
            for (int i = 0; i < characterCodes.Length; i++)
                expandedCodes[i] = characterCodes[i];
            characterCodes = expandedCodes;
        }
    }

    /// <summary>
    /// Returns the Character component of the currently selected slot, or null.
    /// </summary>
    public Character GetCurrentCharacter()
    {
        if (CurrentCharacterIndex >= 0 && CurrentCharacterIndex < characterSlots.Length)
        {
            var go = characterSlots[CurrentCharacterIndex];
            if (go != null)
                return go.GetComponent<Character>();
        }
        return null;
    }
}

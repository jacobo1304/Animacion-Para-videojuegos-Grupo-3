using UnityEngine;


[DefaultExecutionOrder(-1)]
public class Character : MonoBehaviour
{

    private bool isAiming;
    private bool isCrouching;
    private bool isEmoting;
    private Transform lockTarget;

    public bool IsAiming
    {

        get => isAiming;
        set => isAiming = value;
    }

    public bool IsCrouching
    {
        get => isCrouching;
        set => isCrouching = value;
    }

    public bool IsEmoting
    {
        get => isEmoting;
        set => isEmoting = value;
    }
    public Transform LockTarget { get => lockTarget;  set => lockTarget=value; }

    private void Awake()
    {
        RegisterComponents();
    }


    private void RegisterComponents()
    {
        foreach(ICharacterComponent component in GetComponentsInChildren<ICharacterComponent>())
        {
            component.ParentCharacter = this;
        }


    }
}

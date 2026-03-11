using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterLook : MonoBehaviour, ICharacterComponent
{

    [SerializeField] private Transform target;


    [SerializeField] private FloatDampener horizontalDampener;
    [SerializeField] private FloatDampener verticalDampener;

    [SerializeField] private float horizontalRotationSpeed;
    [SerializeField] private float verticalRotationSpeed;

    [SerializeField] private Vector2 verticalRotationLimits;

    private float verticalRotation;

    public void OnLook(InputAction.CallbackContext ctx)
    {

     Vector2 inputValue = ctx.ReadValue<Vector2>();
        inputValue = inputValue / new Vector2(Screen.width, Screen.height);
        horizontalDampener.TargetValue = inputValue.x;
        verticalDampener.TargetValue = inputValue.y;

    }

    private void Update()
    {
        horizontalDampener.Update();
        verticalDampener.Update();

        ApplyLookRotation();


    }

    private void ApplyLookRotation()
    {
        if (target == null) {
            throw new NullReferenceException("Target transform is not assigned.");
        }
        if(ParentCharacter.LockTarget != null)
        {
            Vector3 lookDirection = (ParentCharacter.LockTarget.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);

            target.rotation = rotation;
            return;
        }


        target.RotateAround(target.position, transform.up, horizontalDampener.CurrentValue * horizontalRotationSpeed * 360 * Time.deltaTime);
        verticalRotation += verticalDampener.CurrentValue * verticalRotationSpeed * 360 * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, verticalRotationLimits.x, verticalRotationLimits.y);
        
        Vector3 euler = target.localEulerAngles;
        euler.x = verticalRotation;
        target.localEulerAngles = euler;


    }


  [field: SerializeField]  public Character ParentCharacter { get; set; }


}

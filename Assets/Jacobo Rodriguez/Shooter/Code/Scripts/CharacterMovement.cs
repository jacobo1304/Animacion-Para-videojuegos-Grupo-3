using UnityEngine;
using UnityEngine.InputSystem;
public class CharacterMovement : MonoBehaviour, ICharacterComponent
{

    private int _speedXHash;
    private int _speedYHash;

    [SerializeField] private FloatDampener speedX;
    [SerializeField] private FloatDampener speedY;

    [SerializeField ]private float angularSpeed;   

    [SerializeReference] private Camera camera;

    private Quaternion targetRotation;

    private Animator _animator;

    private Rigidbody rb;

   [SerializeField] private float moveSpeed = 5f;
    private float currentMoveSpeed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _speedXHash = Animator.StringToHash("SpeedX");
        _speedYHash = Animator.StringToHash("SpeedY");

        currentMoveSpeed = moveSpeed;
    }

    private void MoveCharacter()
    {
        Vector3 moveDirection = new Vector3(speedX.CurrentValue, 0, speedY.CurrentValue);
        moveDirection = Quaternion.Euler(0, camera.transform.eulerAngles.y, 0) * moveDirection;

        transform.position += moveDirection * currentMoveSpeed * Time.deltaTime;
    }

    private void SolveCharacterRotation()
    {
        Vector3 floorNormal = transform.up;
        Vector3 cameraRealFoward = camera.transform.forward;

        float angleInterpolator = Mathf.Abs(Vector3.Dot(cameraRealFoward, floorNormal));
        Vector3 cameraFoward = Vector3.Lerp(cameraRealFoward, camera.transform.up, angleInterpolator).normalized;

        Vector3 characterForward = Vector3.ProjectOnPlane(cameraFoward, floorNormal).normalized;

        Debug.DrawLine(transform.position, transform.position + characterForward * 3, Color.green, 5);
        targetRotation = Quaternion.LookRotation(characterForward, floorNormal);

    }
    public void OnMove(InputAction.CallbackContext ctx)
    {
        if(ParentCharacter.IsEmoting) return;
        Vector2 inputValue = ctx.ReadValue<Vector2>();
        speedX.TargetValue = inputValue.x;
        speedY.TargetValue = inputValue.y;

    }

    private void Update()
    {

        if (ParentCharacter.IsEmoting)
        {
            speedX.TargetValue = 0;
            speedY.TargetValue = 0;
        }

        speedX.Update();
        speedY.Update();


        currentMoveSpeed = ParentCharacter.IsCrouching ? moveSpeed/2 : moveSpeed;
        float animMultiplier = ParentCharacter.IsCrouching ? .8f : 1f;

        MoveCharacter();

        _animator.SetFloat(_speedXHash, speedX.CurrentValue * animMultiplier);
        _animator.SetFloat(_speedYHash, speedY.CurrentValue * animMultiplier);


        SolveCharacterRotation();

        if(!ParentCharacter.IsAiming)
        {
           ApplyCharacterRotation();
        }

    }

    private void ApplyCharacterRotation()
    {
        float motionMagnitud = Mathf.Sqrt(speedX.TargetValue * speedX.TargetValue + speedY.TargetValue * speedY.TargetValue);
        float rotationSpeed = Mathf.SmoothStep(0, .01f, motionMagnitud);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angularSpeed  * rotationSpeed);
    }

    public Character ParentCharacter { get; set; }
}

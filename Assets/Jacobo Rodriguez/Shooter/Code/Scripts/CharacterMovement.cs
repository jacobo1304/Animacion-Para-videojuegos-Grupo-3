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
    private float maxSpeedX;
    private float maxSpeedY;
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

        if (ParentCharacter.IsStealth)
        {
            maxSpeedX = 0.3f;
            maxSpeedY = 0.3f;
            currentMoveSpeed = moveSpeed / 2f;
        }
        else if (ParentCharacter.IsCrouching)
        {
            maxSpeedX = 0.8f;
            maxSpeedY = 0.8f;
            currentMoveSpeed = moveSpeed / 2f;
        }
        else
        {
            maxSpeedX = 1f;
            maxSpeedY = 1f;
            currentMoveSpeed = moveSpeed;
        }

        speedX.TargetValue = Mathf.Clamp(speedX.TargetValue, -maxSpeedX, maxSpeedX);
        speedY.TargetValue = Mathf.Clamp(speedY.TargetValue, -maxSpeedY, maxSpeedY);

        speedX.Update();
        speedY.Update();

        MoveCharacter();

        _animator.SetFloat(_speedXHash, speedX.CurrentValue);
        _animator.SetFloat(_speedYHash, speedY.CurrentValue);


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

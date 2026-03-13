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
    // Commented out: legacy max speed clamping per-axis. We no longer modify animator input values
    // when stealth/crouch is active; stealth/crouch only affect movement speed (currentMoveSpeed).
    // private float maxSpeedX;
    // private float maxSpeedY;
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
        Vector2 inputValue = ctx.ReadValue<Vector2>();
        ParentCharacter.MovementInput = inputValue;

        if (ParentCharacter.IsEmoting) return;
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
            // maxSpeedX = 0.3f;
            // maxSpeedY = 0.3f;
            currentMoveSpeed = moveSpeed / 3f;
        }
        else if (ParentCharacter.IsCrouching)
        {
            // maxSpeedX = 0.8f;
            // maxSpeedY = 0.8f;
            currentMoveSpeed = moveSpeed / 2f;
        }
        else
        {
            // maxSpeedX = 1f;
            // maxSpeedY = 1f;
            currentMoveSpeed = moveSpeed;
        }

        // Commented out: do not modify the input values sent to the animator based on stealth/crouch.
        // We keep the original input values for animation, and only affect actual movement speed above.
        // speedX.TargetValue = Mathf.Clamp(speedX.TargetValue, -maxSpeedX, maxSpeedX);
        // speedY.TargetValue = Mathf.Clamp(speedY.TargetValue, -maxSpeedY, maxSpeedY);

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

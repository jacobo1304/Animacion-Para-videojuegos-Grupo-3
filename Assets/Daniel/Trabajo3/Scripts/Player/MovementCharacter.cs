using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MovementCharacter : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private string canMoveParam = "CanMove";
    [SerializeField] private float deadzoneEnter = 0.15f;
    [SerializeField] private float deadzoneExit = 0.25f;
    [SerializeField] private float animatorDampTime = 0.12f;

    private Rigidbody rb;
    private Vector3 direction;
    private float inputX;
    private float inputY;
    private bool inDeadzoneX = true;
    private bool inDeadzoneY = true;
    private Vector3 lastFacingDirection = Vector3.forward;
    [SerializeField] private Transform facingRoot;

    public Vector3 LastFacingDirection => lastFacingDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (facingRoot == null)
        {
            facingRoot = animator != null ? animator.transform : transform;
        }
    }

    private void OnEnable()
    {
        if (movementAction != null)
        {
            movementAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (movementAction != null)
        {
            movementAction.action.Disable();
        }
    }

    private void Update()
    {
        Vector2 inputValue = Vector2.zero;
        if (movementAction != null)
        {
            inputValue = movementAction.action.ReadValue<Vector2>();
        }

        inputX = inputValue.x;
        inputY = inputValue.y;

        bool canMove = true;
        if (animator != null)
        {
            canMove = animator.GetBool(canMoveParam);
        }

        if (!canMove)
        {
            inputX = 0f;
            inputY = 0f;
        }
        else
        {
            inputX = ApplyHysteresis(inputX, ref inDeadzoneX, deadzoneEnter, deadzoneExit);
            inputY = ApplyHysteresis(inputY, ref inDeadzoneY, deadzoneEnter, deadzoneExit);
        }

        Vector3 inputLocal = new Vector3(inputX, 0f, inputY);
        Vector3 inputNormalized = inputLocal.sqrMagnitude > 1f ? inputLocal.normalized : inputLocal;
        direction = facingRoot.TransformDirection(inputNormalized);
        if (direction.sqrMagnitude > 0.0001f)
        {
            lastFacingDirection = direction.normalized;
        }

        if (animator != null)
        {
            animator.SetFloat("horizontal", inputX, animatorDampTime, Time.deltaTime);
            animator.SetFloat("vertical", inputY, animatorDampTime, Time.deltaTime);

            if (direction.sqrMagnitude > 0f && canMove)
            {
                animator.SetTrigger("Move");
            }
        }
    }

    private static float ApplyHysteresis(float value, ref bool inDeadzone, float enter, float exit)
    {
        float abs = Mathf.Abs(value);

        if (inDeadzone)
        {
            if (abs > exit)
            {
                inDeadzone = false;
            }
            else
            {
                return 0f;
            }
        }
        else
        {
            if (abs < enter)
            {
                inDeadzone = true;
                return 0f;
            }
        }

        return value;
    }

    private void FixedUpdate()
    {
        Vector3 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
}

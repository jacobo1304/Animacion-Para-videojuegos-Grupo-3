using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MovementCharacter : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4.5f;
    [SerializeField] private Animator animator;
    [SerializeField] private InputActionReference movementAction;
    [SerializeField] private string canMoveParam = "CanMove";

    private Rigidbody rb;
    private Vector3 direction;
    private float inputX;
    private float inputY;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
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

        Vector3 input = new Vector3(inputX, 0f, inputY);
        direction = input.sqrMagnitude > 1f ? input.normalized : input;

        if (animator != null)
        {
            animator.SetFloat("horizontal", inputX);
            animator.SetFloat("vertical", inputY);

            if (direction.sqrMagnitude > 0f && canMove)
            {
                animator.SetTrigger("Move");
            }
        }
    }

    private void FixedUpdate()
    {
        Vector3 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }
}

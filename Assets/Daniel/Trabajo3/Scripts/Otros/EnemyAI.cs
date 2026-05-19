using Clases.Clase_8.Scripts;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private State currentState;

    public NavMeshAgent agent;
    public Transform player;
    public Transform[] waypoints;
    public Animator animator;


    [Header("Movement")]
    public float walkSpeed = 2.0f;
    public float runSpeed = 3.5f;
    public float rotationSmooth = 12f;
    public float animSmooth = 10f;


    private int waypointIndex = 0;

    public ComboSequence defaultCombo;

    static class Hash
    {
        public static readonly int SpeedX = Animator.StringToHash("SpeedX");
        public static readonly int SpeedY = Animator.StringToHash("SpeedY");
    }

    private void Start()
    {
        agent.updatePosition = true;
        agent.updateRotation = false;
        animator.applyRootMotion = false;

        ChangeState(new IdleState(enemy:this));
    }

    private void Update()
    {
        currentState?.Update();

        Vector3 desird = agent.desiredVelocity;
        desird.y = 0;

        if(desird.sqrMagnitude > 0.001f)
        {
            Quaternion look = Quaternion.LookRotation(forward:desird, upwards:Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, rotationSmooth * Time.deltaTime);   
        }
         Vector3 dirLocal = desird.sqrMagnitude > 0.001f ? transform.InverseTransformDirection(desird.normalized) : Vector3.zero;

         float denom = Mathf.Max(0.01f, agent.speed);
         float mag01 = Mathf.Clamp01(agent.velocity.magnitude / denom);

         float targetX = dirLocal.x * mag01;
         float targetY = dirLocal.z * mag01;

         float curX = Mathf.Lerp(animator.GetFloat(id:Hash.SpeedX), targetX, t:Time.deltaTime * animSmooth);
         float curY = Mathf.Lerp(animator.GetFloat(id:Hash.SpeedY), targetY, t:Time.deltaTime * animSmooth);

         animator.SetFloat(id:Hash.SpeedX, curX);
         animator.SetFloat(id:Hash.SpeedY, curY);
    }

    public void ChangeState(State newState = null)
    {
        if (newState == null) return;
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

     public void NextWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        waypointIndex = (waypointIndex + 1) % waypoints.Length; 
        agent.SetDestination(waypoints[waypointIndex].position);
    }

    public bool PlayerInRange(float range)
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position,player.position) < range;
    }

   
}


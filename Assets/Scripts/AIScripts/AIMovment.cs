using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;

public class AIMovment : MonoBehaviour
{
    private Rigidbody2D rb2d;

    public UnityEvent<Vector2> OnPointerPosition;
    public UnityEvent OnAttack;
    private Vector2 agentMovement, agentPointer;

    [SerializeField]private float maxSpeed = 2f, Accel = 50f, deAccel = 100f, speed = 0;
    private Vector2 oldinp;

    public Vector2 AgentPointer { get => agentPointer; set => agentPointer = value; }
    public Vector2 AgentMovement { get => agentMovement; set => agentMovement = value; }

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }


    public void PerformAttack()
    {
        OnAttack?.Invoke();
    }

    private void Update()
    {
        //Debug.Log(AgentPointer);
        OnPointerPosition?.Invoke(agentPointer);
    }

    private void FixedUpdate()
    {
        //Debug.Log(agentMovement.magnitude);
        if (AgentMovement.magnitude > 0 && speed >= 0)
        {
            oldinp = AgentMovement;
            speed += Accel * maxSpeed * Time.deltaTime;
        }
        else
            speed -= deAccel * maxSpeed * Time.deltaTime;
        speed = Mathf.Clamp(speed, 0, maxSpeed);
        rb2d.velocity = oldinp * speed;
    }
}

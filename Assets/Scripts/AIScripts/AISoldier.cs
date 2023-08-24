using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.GraphicsBuffer;
using Unity.VisualScripting;

public class AISoldier : MonoBehaviour
{
    [SerializeField] private List<SteeringBehaviour> sbs;
    [SerializeField] private List<AITargeting> detectors;
    [SerializeField] private AIData aiData;
    [SerializeField] private ContextSolver solver;
    [SerializeField] GameObject map;
    [SerializeField] private float attackDist = 0.5f;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private GameObject gameManager;

    AstarAlgorithm pathfind;
    int[,] mapArr;

    public float delayperdetect = 0.05f, aidelay = 0.06f, attackDelay = 1f;

    public Vector2 PointerPos;
    public Vector2 MovementPos;

    bool chase = false;
    private bool isTargetLocation = false;

    private Transform Target;

    public UnityEvent OnAttackPerformed;
    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;

    private Vector2 RoamPos;
    private System.Random rand;
    private float delaytime = 2f;
    private float isAlarmReady = 0f;

    private void Start()
    {
        map = GameObject.FindGameObjectWithTag("Map");
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        mapArr = map.GetComponent<MapGenerator>().GetMapArray();
        pathfind = new AstarAlgorithm(mapArr);
        rand = new System.Random();
        InvokeRepeating("PerformDetection", 0, delayperdetect);
        RoamRoom();
    }

    private void FixedUpdate()
    {
        OnPointerInput?.Invoke(PointerPos);
        OnMovementInput?.Invoke(MovementPos);
    }

    private void Update()
    {
        delaytime -= Time.deltaTime;
        isAlarmReady -= Time.deltaTime;

        if (aiData.currentTarget != null)
        {
            PointerPos = aiData.currentTarget.position;
            if (chase == false)
            {
                chase = true;
                if(isAlarmReady <= 0)
                {
                    gameManager.GetComponent<GameManager>().Target = aiData.currentTarget.gameObject;
                    isAlarmReady = 20f;
                }
                StartCoroutine(ChaseandAttack());
            }
        }
        else if (aiData.GetTargetsCount() > 0)
            aiData.currentTarget = aiData.targets[0];

        else if (isTargetLocation && Target != null)
        {
            if (Vector2.Distance(Target.position, transform.position) > 10f)
                GotoTarget();
            else
                isTargetLocation = false;
        }

        else
            {
            RoamRoom();
            MovementPos = RoamPos;
            PointerPos = RoamPos + (Vector2)transform.position;
            }
    }

    private void RoamRoom()
    {
        if (delaytime > 0)
            return;
        delaytime = 0.5f;
        float x = rand.Next(-1, 2);
        float y = rand.Next(-1, 2);
        RoamPos = new Vector2(x, y);
    }
 
    public void SetTarget(Transform target)
    {
        Target = target;
        isTargetLocation = true;
    }
    private void GotoTarget()
    {
        if (Target == null)
            return;

        List<Vector3> pathList = GetPath(Target);
        var dir = pathList[1] - transform.position;
        MovementPos = dir.normalized;
    }

    public List<Vector3> GetPath(Transform targetPos)
    {
        Vector3 start;
        Vector3 end;
        bool reverse;

        if (Vector3.Distance(transform.position, Vector3.zero) < Vector3.Distance(targetPos.position, Vector3.zero))
        {
            start = transform.position;
            end = targetPos.position;
            reverse = false;
        }
        else
        {
            start = targetPos.position;
            end = transform.position;
            reverse = true;
        }

        List<Vector3> pathList = pathfind.FindPath(start, end);

        if (pathList != null)
            if (reverse)
                pathList.Reverse();

        return pathList;
    }
    private void PerformDetection()
    {
        foreach (AITargeting targeter in detectors)
            targeter.Detect(aiData);

        float[] danger = new float[8];
        float[] interest = new float[8];

        foreach (SteeringBehaviour sb in sbs)
            (danger, interest) = sb.GetSteering(danger, interest, aiData);
    }

    private IEnumerator ChaseandAttack()
    {
        if (aiData.currentTarget == null)
        {
            Debug.Log("No targets");
            MovementPos = Vector3.zero;
            PointerPos = Vector3.right;
            chase = false;
            yield break;
        }
        else
        {
            float dist = Vector2.Distance(aiData.currentTarget.position, transform.position);

            if (dist < attackDist)
            {
                Debug.Log("Attacking Target");
                MovementPos = Vector2.zero;
                OnAttackPerformed?.Invoke();
                yield return new WaitForSeconds(attackDelay);
                StartCoroutine(ChaseandAttack());
            }

            else
            {
                Debug.Log("Approaching Target");
                MovementPos = solver.GetDir(sbs, aiData);
                yield return new WaitForSeconds(attackDelay);
                StartCoroutine(ChaseandAttack());
            }
        }
    }
}
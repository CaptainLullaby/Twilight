using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AIBrute : MonoBehaviour
{
    [SerializeField] private List<SteeringBehaviour> sbs;
    [SerializeField] private List<AITargeting> detectors;
    [SerializeField] private AIData aiData;
    [SerializeField] private ContextSolver solver;
    [SerializeField] private GameObject map;
    [SerializeField] private float attackDist = 0.45f;
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


    private void Start()
    {
        map = GameObject.FindGameObjectWithTag("Map");
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        mapArr = map.GetComponent<MapGenerator>().GetMapArray();
        pathfind = new AstarAlgorithm(mapArr);
        InvokeRepeating("PerformDetection", 0, delayperdetect);
    }

    private void Update()
    {

        if (aiData.currentTarget != null)
        {
            PointerPos = aiData.currentTarget.position;
            if (chase == false)
            {
                chase = true;
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
    }

    private void FixedUpdate()
    {
        OnPointerInput?.Invoke(PointerPos);
        OnMovementInput?.Invoke(MovementPos);
    }

    public void SetTarget(Transform target)
    {
        if (Target == null)
            return;

        Target = target;
        isTargetLocation = true;
    }

    private void GotoTarget()
    {
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

    private void OnDestroy()
    {
        gameManager.GetComponent<GameManager>().Target = GameObject.FindGameObjectWithTag("PlayerChar");
    }
}
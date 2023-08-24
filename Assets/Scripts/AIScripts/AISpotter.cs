using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;

public class AISpotter : MonoBehaviour
{
    [SerializeField] private List<SteeringBehaviour> sbs;
    [SerializeField] private List<AITargeting> detectors;
    [SerializeField] private AIData aiData;
    [SerializeField] private ContextSolver solver;
    [SerializeField] GameObject map;
    [SerializeField] private float attackDist = 0.5f;
    [SerializeField] private GameObject gameManager;

    AstarAlgorithm pathfind;
    int[,] mapArr;

    public float delayperdetect = 0.05f, aidelay = 0.06f, attackDelay = 1f;

    public Vector2 PointerPos;
    public Vector2 MovementPos;

    bool chase = false;
    public bool isTargetLocation = false;
    public Transform Target;
    float backupCall = 0;
    private Vector2 newPos;

    public UnityEvent OnAttackPerformed;
    public UnityEvent<Vector2> OnMovementInput, OnPointerInput;


    private void Start()
    {
        map = GameObject.FindGameObjectWithTag("Map");
        mapArr = map.GetComponent<MapGenerator>().GetMapArray();
        pathfind = new AstarAlgorithm(mapArr);
        InvokeRepeating("PerformDetection", 0, delayperdetect);
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        newPos = RoamAroundtheMap();
    }

    private void Update()
    {
        backupCall -= Time.deltaTime;
        if (aiData.currentTarget != null)
        {
            PointerPos = aiData.currentTarget.position;
            if (chase == false)
            {
                chase = true;
                if (backupCall <= 0)
                {
                    gameManager.GetComponent<GameManager>().backup = true;
                    backupCall = 20f;
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
            if (Vector2.Distance(newPos, transform.position) > 1f)
            {
                List<Vector3> pathList = GetPath(newPos);
                var dir = pathList[1] - transform.position;
                MovementPos = dir.normalized;
            }
            else
                newPos = RoamAroundtheMap();
        }
    }

    private void FixedUpdate()
    {
        OnPointerInput?.Invoke(PointerPos);
        OnMovementInput?.Invoke(MovementPos);
    }

    private Vector2 RoamAroundtheMap()
    {
        System.Random rand = new System.Random();
        var rooms = GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>().GetRooms();
        var room = rooms[rand.Next(0, rooms.Count)];
        return room.center;
    }

    public void SetTarget(Transform target)
    {
        Target = target;
        isTargetLocation = true;
    }

    private void GotoTarget() //Gototowards the target;
    {
        if (Target == null)
            return;

        List<Vector3> pathList = GetPath(Target.position);
        var dir = pathList[1] - transform.position;
        MovementPos = dir.normalized;
    }

    public List<Vector3> GetPath(Vector2 targetPos)
    {
        Vector3 start;
        Vector3 end;
        bool reverse;

        if (Vector3.Distance(transform.position, Vector3.zero) < Vector3.Distance(targetPos, Vector3.zero))
        {
            start = transform.position;
            end = targetPos;
            reverse = false;
        }
        else
        {
            start = targetPos;
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